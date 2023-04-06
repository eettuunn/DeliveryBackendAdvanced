﻿using delivery_backend_advanced.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace delivery_backend_advanced.Models;

public class AppDbContext : DbContext
{
    public DbSet<DishEntity> Dishes { get; set; } 
    public DbSet<DishBasketEntity> DishesInBasket { get; set; }
    public DbSet<MenuEntity> Menus { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<RatingEntity> Ratings { get; set; }
    public DbSet<RestaurantEntity> Restaurants { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    /*public AppDbContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("host=localhost;port=5432;database=delivery_db;username=postgres;password=root");
    }*/
}