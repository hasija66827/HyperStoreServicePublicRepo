using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HyperStoreServiceAPP;
namespace HyperStoreService.Models
{
    public class SupplierOrder
    {
        public Guid SupplierOrderId { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal PaidAmount { get; set; }
        [Required]
        public string SupplierOrderNo { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}