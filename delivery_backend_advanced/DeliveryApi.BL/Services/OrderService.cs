using AutoMapper;
using delivery_backend_advanced.Exceptions;
using delivery_backend_advanced.Models;
using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Entities;
using delivery_backend_advanced.Models.Enums;
using delivery_backend_advanced.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DeliveryApi.BL.Services;

public class OrderService : IOrderService
{
    private readonly BackendDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;

    public OrderService(BackendDbContext context, IMapper mapper, IConfiguration configuration, IMessageProducer messageProducer)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _messageProducer = messageProducer;
    }

    public async Task CreateOrder(CreateOrderDto orderDto, UserInfoDto userInfoDto)
    {
        CheckCreateOrderValidness(orderDto, userInfoDto);
        await CreateCustomerIfNull(userInfoDto);
        
        var orderDishes = await _context
            .DishesInBasket
            .Include(dib => dib.Dish)
            .Where(dib => !dib.IsInOrder && dib.Customer.Id == userInfoDto.id)
            .ToListAsync();
        var customer = await _context
            .Customers
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (orderDishes.Count == 0)
        {
            throw new ConflictException("There are no dishes in basket");
        }
        
        var restaurantEntity = await _context
                                   .Restaurants
                                   .FirstOrDefaultAsync(r => r.Id == orderDto.restaurantId) ??
                               throw new CantFindByIdException("restaurant", orderDto.restaurantId);
        
        OrderEntity newOrder = new OrderEntity()
        {
            Id = new Guid(),
            DeliveryTime = orderDto.deliveryTime ?? DateTime.UtcNow.AddHours(1),
            OrderTime = DateTime.UtcNow,
            Price = orderDishes.Sum(dib => dib.Dish.Price * dib.Amount),
            Address = orderDto.address ?? userInfoDto.address,
            Status = OrderStatus.Created,
            Restaurant = restaurantEntity,
            Dishes = orderDishes,
            Number = await _context.Orders.CountAsync() + 1,
            Customer = customer
        };
        
        orderDishes.Select(dib =>
        {
            dib.IsInOrder = true;
            return dib;
        }).ToList();


        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
        
        SendOrderStatusChangedMessage(newOrder);
    }

    public async Task<OrdersPageDto> GetOrders(OrderQueryModel query, UserRole role, UserInfoDto userInfoDto)
    {
        await CreateCustomerIfNull(userInfoDto);
        
        var orderEn = OrdersRoleDepend(role, query.current, userInfoDto.id);
        
        var page = query.page ?? 1;
        var pageOrderCount = _configuration.GetValue<double>("PageSize");
        var orderCountInRest = await _context.Orders.CountAsync();
        var ordersSkip = (int)((page - 1) * pageOrderCount);
        var ordersTake = (int)Math.Min(orderCountInRest - (page - 1) * pageOrderCount, pageOrderCount);

        var orderList = SortAndFilterOrders(orderEn, query, role);
        
        var pageCount = (int)Math.Ceiling(orderList.Count / pageOrderCount);
        pageCount = pageCount == 0 ? 1 : pageCount;
        orderList = orderList
            .Skip(ordersSkip)
            .Take(ordersTake)
            .ToList();
        if (page > pageCount || page < 1)
        {
            throw new BadRequestException("Incorrect current page");
        }

        
        List<OrderListElementDto> orderDtos = _mapper.Map<List<OrderListElementDto>>(orderList);

        
        var pageInfo = new PaginationDto()
        {
            current = page,
            count = pageCount,
            size = orderList.Count
        };
        OrdersPageDto ordersPage = new OrdersPageDto()
        {
            orders = orderDtos,
            pagination = pageInfo
        };
        
        
        return ordersPage;
    }

    public async Task<OrderDto> GetOrderDetails(Guid orderId, UserInfoDto userInfoDto)
    {
        await CreateCustomerIfNull(userInfoDto);
        
        var orderEntity = await _context
            .Orders
            .Include(order => order.Restaurant)
            .Include(order => order.Dishes)
            .ThenInclude(dish => dish.Dish)
            .FirstOrDefaultAsync(order => order.Id == orderId && order.Customer.Id == userInfoDto.id) ?? throw new CantFindByIdException("order", orderId);
        
        OrderDto orderDto = _mapper.Map<OrderDto>(orderEntity);
        orderDto.dishes = _mapper.Map<List<DishInOrderDto>>(orderEntity.Dishes.Select(d => d.Dish));
        for (int i = 0; i < orderEntity.Dishes.Count; i++)
        {
            orderDto.dishes[i].amount = orderEntity.Dishes[i].Amount;
        }
        //todo: no dishes 
        return orderDto;

    }

    public async Task CancelOrder(Guid orderId, UserInfoDto userInfoDto)
    {
        await CreateCustomerIfNull(userInfoDto);
        
        var orderEntity = await _context
            .Orders
            .FirstOrDefaultAsync(order => order.Id == orderId && order.Customer.Id == userInfoDto.id) ?? throw new CantFindByIdException("order", orderId);
        if (orderEntity.Status != OrderStatus.Created)
        {
            throw new ConflictException("You cant cancel order, that is already taken by cook");
        }
        orderEntity.Status = OrderStatus.Canceled;

        await _context.SaveChangesAsync();
        
        SendOrderStatusChangedMessage(orderEntity);
    }

    public async Task RepeatOrder(RepeatOrderDto repOrder, Guid orderId, UserInfoDto userInfoDto)
    {
        var customer = await CreateCustomerIfNull(userInfoDto);
        
        if (repOrder.deliveryTime < DateTime.UtcNow.AddHours(1) && repOrder.deliveryTime != null)
        {
            throw new BadRequestException("delivery time must be more, then now + 1 hour");
        }
        
        var prevOrder = await _context
            .Orders
            .Include(order => order.Dishes)
            .Include(order => order.Restaurant)
            .FirstOrDefaultAsync(order => order.Id == orderId && order.Customer.Id == userInfoDto.id) ?? throw new CantFindByIdException("order", orderId);

        OrderEntity newOrder = new OrderEntity()
        {
            Id = new Guid(),
            DeliveryTime = repOrder.deliveryTime ?? DateTime.UtcNow.AddHours(1),
            OrderTime = DateTime.UtcNow,
            Price = prevOrder.Price,
            Address = repOrder.address,
            Status = OrderStatus.Created,
            Number = await _context.Orders.CountAsync() + 1,
            Restaurant = prevOrder.Restaurant,
            Dishes = prevOrder.Dishes,
            Customer = customer
        };

        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
        
        SendOrderStatusChangedMessage(newOrder);
    }



    private IEnumerable<OrderEntity> OrdersRoleDepend(UserRole role, bool current, Guid userId)
    {
        IEnumerable<OrderEntity> orders = Enumerable.Empty<OrderEntity>();
        switch (role)
        {
            case UserRole.Customer:
                orders = _context
                    .Orders
                    .Where(order => (current == false || order.Status != OrderStatus.Canceled && order.Status != OrderStatus.Delivered) && order.Customer.Id == userId)
                    .AsEnumerable();
                break;
            case UserRole.Manager:
                orders = _context
                    .Orders
                    .Include(order => order.Restaurant)
                    .ThenInclude(r => r.Managers)
                    .Where(order => order.Restaurant.Managers.Any(m => m.Id == userId))
                    .AsEnumerable();
                break;
            case UserRole.Cook:
                orders = _context
                    .Orders
                    .Where(
                        order => !current &&
                                 order.Status == OrderStatus.Created || current && order.Cook.Id == userId)
                    .AsEnumerable();
                break;
            case UserRole.Courier:
                orders = _context
                    .Orders
                    .Where(
                        order => !current  &&
                                 order.Status == OrderStatus.Delivery || current && order.Courier.Id == userId)
                    .AsEnumerable();
                break;
        }

        return orders;
    }
    
    private List<OrderEntity> SortAndFilterOrders(IEnumerable<OrderEntity> orders, OrderQueryModel query, UserRole role)
    {
        if (query.sort != null)
        {
            switch (query.sort)
            {
                case OrderSort.CreateAsc:
                    orders = orders.OrderBy(order => order.OrderTime).ToList();
                    break;
                case OrderSort.CreateDesc:
                    orders = orders.OrderByDescending(order => order.OrderTime).ToList();
                    break;
                case OrderSort.DeliveryAsc:
                    orders = orders.OrderBy(order => order.DeliveryTime).ToList();
                    break;
                case OrderSort.DeliveryDesc:
                    orders = orders.OrderByDescending(order => order.DeliveryTime).ToList();
                    break;
            }
        }
        
        if (query.search != null)
        {
            orders = orders.Where(order => order.Number.ToString().Contains(query.search.ToString())).ToList();
        }

        if (query.statuses.Count != 0 && role == UserRole.Manager)
        {
            orders = orders.Where(order => query.statuses.Contains(order.Status)).ToList();
        }

        return orders.ToList();
    }

    private void CheckCreateOrderValidness(CreateOrderDto orderDto, UserInfoDto userInfoDto)
    {
        if (orderDto.address == null && userInfoDto.address == null)
        {
            throw new BadRequestException("You need to write address in request body or in your profile");
        }
        if (orderDto.deliveryTime < DateTime.UtcNow.AddHours(1) && orderDto.deliveryTime != null)
        {
            throw new BadRequestException("delivery time must be more, then now + 1 hour");
        }
    }
    
    private async Task<Customer> CreateCustomerIfNull(UserInfoDto userInfoDto)
    {
        var customer = await _context
            .Customers
            .FirstOrDefaultAsync(c => c.Id == userInfoDto.id);
        if (customer == null)
        {
            var newCustomer = new Customer()
            {
                Id = userInfoDto.id,
                Address = userInfoDto.address
            };
            
            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();
            customer = newCustomer;
        }
        customer.Address = userInfoDto.address ?? customer.Address;

        return customer;
    }
    
    private void SendOrderStatusChangedMessage(OrderEntity orderEntity)
    {
        var orderStatusMessage = new OrderStatusMessage
        {
            orderId = orderEntity.Id,
            newStatus = orderEntity.Status,
            address = orderEntity.Address,
            number = orderEntity.Number
        };
        
        _messageProducer.SendMessage(orderStatusMessage);
    }
}