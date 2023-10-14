using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models;
using Electronic.Domain.Models.Catalog;
using Electronic.Domain.Models.Core;
using Electronic.Domain.Models.Inventory;
using Electronic.Domain.Models.New;
using Electronic.Domain.Models.Order;
using Electronic.Domain.Models.Payment;
using Electronic.Domain.Models.Review;
using Electronic.Domain.Models.ShoppingCart;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.DatabaseContext;

public class ElectronicDatabaseContext : DbContext
{
    public ElectronicDatabaseContext(DbContextOptions<ElectronicDatabaseContext> options) : base (options)
    {
        
    }

    #region Advertisement

    public DbSet<Advertisement> Advertisements { get; set; }

    #endregion

    #region Core

    public DbSet<District> Districts { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<StateOrProvince> StateOrProvinces { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<Address> Addresses { get; set; }

    #endregion

    #region Catalog

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductAttribute> ProductAttributes { get; set; }
    public DbSet<ProductAttributeGroup> ProductAttributeGroups { get; set; }
    public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
    public DbSet<ProductOption> ProductOptions { get; set; }
    public DbSet<ProductOptionGroup> ProductOptionGroups { get; set; }
    public DbSet<ProductOptionValue> ProductOptionValues { get; set; }
    public DbSet<ProductMedia> ProductMedias { get; set; }
    public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; }
    public DbSet<ProductLink> ProductLinks { get; set; }

    #endregion

    #region Inventory

    public DbSet<Stock> Stocks { get; set; }
    public DbSet<StockHistory> StockHistories { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    #endregion

    #region New

    public DbSet<NewItem> NewItems { get; set; }
    public DbSet<NewCategory> NewCategories { get; set; }

    #endregion

    #region Order

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    
    #endregion

    #region Payment

    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentProvider> PaymentProviders { get; set; }

    #endregion

    #region Review

    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewReply> ReviewReplies { get; set; }

    #endregion

    #region Shopping Cart

    public DbSet<Cart> Carts { get; set; }

    #endregion
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElectronicDatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}