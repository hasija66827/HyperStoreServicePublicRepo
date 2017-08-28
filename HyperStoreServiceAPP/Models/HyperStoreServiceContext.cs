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

        public System.Data.Entity.DbSet<HyperStoreService.Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.ProductTag> ProductTags { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.Supplier> Suppliers { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.CustomerOrder> CustomerOrders { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.SupplierOrder> SupplierOrders { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.CustomerOrderProduct> CustomerOrderProducts { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.SupplierOrderProduct> SupplierOrderProducts { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.SupplierTransaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.SupplierOrderTransaction> SupplierOrderTransactions { get; set; }

        public System.Data.Entity.DbSet<HyperStoreService.Models.Tag> Tags { get; set; }
    }
}
