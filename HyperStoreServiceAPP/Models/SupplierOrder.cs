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
        public decimal PayedAmount { get; set; }
        public decimal PayedAmountIncTx { get; set; }
        public decimal InterestRate { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalQuantity { get; set; }
        [Required]
        public string SupplierOrderNo { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}