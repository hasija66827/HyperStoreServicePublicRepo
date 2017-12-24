using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class HyperStoreServiceContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public HyperStoreServiceContext() : base("name=9970377274DBConnection")
        {
        }

        public HyperStoreServiceContext(string name) : base(name)
        {
        }

        public System.Data.Entity.DbSet<Product> Products { get; set; }

        public System.Data.Entity.DbSet<ProductTag> ProductTags { get; set; }

        public System.Data.Entity.DbSet<Person> Persons { get; set; }
      
        public System.Data.Entity.DbSet<Order> Orders { get; set; }
      
        public System.Data.Entity.DbSet<OrderProduct> OrderProducts { get; set; }
 
        public System.Data.Entity.DbSet<Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<OrderTransaction> OrderTransactions { get; set; }

        public System.Data.Entity.DbSet<DeficientStockHit> DeficientStockHits { get; set; }

        public System.Data.Entity.DbSet<Tag> Tags { get; set; }

        public System.Data.Entity.DbSet<PurchaseHistory> PurchaseHistory { get; set; }

        public System.Data.Entity.DbSet<Cart>Carts { get; set; }

        public System.Data.Entity.DbSet<CartProduct> CartProducts { get; set; }

        public System.Data.Entity.DbSet<PaymentOption> PaymentOptions{ get; set; }

    }
}
