using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class SupplierOrder
    {

        public Guid SupplierOrderId { get; set; }
        public string SupplierOrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public float BillAmount { get; set; }
        public float PaidAmount { get; set; }

        public SupplierOrder() { }

        [Required]
        public Nullable<Guid> SupplierId;
        public Supplier Supplier;

        public List<SupplierOrderProduct> SupplierOrderProducts { get; set; }
        public List<SupplierOrderTransaction> SupplierOrderTransactions { get; set; }
    }
}