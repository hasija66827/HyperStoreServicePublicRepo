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
    
        public HyperStoreServiceContext() : base("name=HyperStoreServiceContext")
        {
        }

        public System.Data.Entity.DbSet<Product> Products { get; set; }

        public System.Data.Entity.DbSet<ProductTag> ProductTags { get; set; }

        public System.Data.Entity.DbSet<Supplier> Suppliers { get; set; }

        public System.Data.Entity.DbSet<Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<CustomerOrder> CustomerOrders { get; set; }

        public System.Data.Entity.DbSet<SupplierOrder> SupplierOrders { get; set; }

        public System.Data.Entity.DbSet<CustomerOrderProduct> CustomerOrderProducts { get; set; }

        public System.Data.Entity.DbSet<SupplierOrderProduct> SupplierOrderProducts { get; set; }

        public System.Data.Entity.DbSet<CustomerTransaction> CustomerTransactions { get; set; }

        public System.Data.Entity.DbSet<SupplierTransaction> SupplierTransactions { get; set; }

        public System.Data.Entity.DbSet<CustomerOrderTransaction> CustomerOrderTransactions { get; set; }

        public System.Data.Entity.DbSet<SupplierOrderTransaction> SupplierOrderTransactions { get; set; }

        public System.Data.Entity.DbSet<DeficientStockHit> DeficientStockHits { get; set; }

        public System.Data.Entity.DbSet<Tag> Tags { get; set; }
    }
}
