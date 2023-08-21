using Microsoft.EntityFrameworkCore;
using Order.API.Models.Entities;

namespace Order.API.Models
{
    public class OrderAPIDbContext : DbContext
    {
        public OrderAPIDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Entities.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Order>(m =>
            {
                m.ToTable("Orders").HasKey(o=>o.OrderId);
                m.Property(m => m.OrderId).HasColumnName("OrderId");
                m.Property(m => m.BuyerId).HasColumnName("BuyerId");
                m.Property(m => m.CreatedDate).HasColumnName("CreatedDate");
                m.Property(m => m.OrderStatus).HasColumnName("OrderStatus");
                m.Property(m => m.TotalPrice).HasColumnName("TotalPrice");
                m.HasMany(o => o.OrderItems);
            });

            modelBuilder.Entity<OrderItem>(m =>
            {
                m.ToTable("OrderItems").HasKey(o => o.Id);
                m.Property(m => m.Id).HasColumnName("Id ");
                m.Property(m => m.OrderId).HasColumnName("OrderId");
                m.Property(m => m.Count).HasColumnName("Count");
                m.Property(m => m.Price).HasColumnName("Price");
                m.Property(m => m.ProductId).HasColumnName("ProductId");
                m.HasOne(o => o.Order);
            });


        }
    }
}
