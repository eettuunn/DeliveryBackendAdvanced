using Microsoft.EntityFrameworkCore;
using NotificationAPI.Models;

namespace NotificationAPI;

public class NotificationDbContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; }
    
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }
}