#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
namespace WeedingPlanner.Models;

public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; } 
    public DbSet<Wedding> Weddings {get; set; } 
    public DbSet<Invitation> Invitations { get; set; } 
}