using AUserRegister.Models;
using Microsoft.EntityFrameworkCore;

namespace AUserRegister.Persistence;
 
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
}