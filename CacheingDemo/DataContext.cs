using CacheingDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacheingDemo
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) 
        {
            
        }

        public DbSet<Driver> Drivers { get; set; }
    }
}
