using AuthApi.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.DAL;

public class AuthDbContext : IdentityDbContext<AppUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options){}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Cook)
            .WithOne(x => x.User)
            .HasForeignKey<CookEntity>().IsRequired();
        
        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Manager)
            .WithOne(x => x.User)
            .HasForeignKey<ManagerEntity>().IsRequired();
        
        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Courier)
            .WithOne(x => x.User)
            .HasForeignKey<CourierEntity>().IsRequired();
        
        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Customer)
            .WithOne(x => x.User)
            .HasForeignKey<CustomerEntity>().IsRequired();
    }
    
    public DbSet<ManagerEntity> Managers { get; set; } 
    public DbSet<CookEntity> Cooks { get; set; } 
    public DbSet<CustomerEntity> Customers { get; set; } 
    public DbSet<CourierEntity> Couriers { get; set; }
}