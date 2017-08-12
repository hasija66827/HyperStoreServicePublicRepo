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
        [Required]
        public Guid? SupplierOrderId { get; set; }
        [Required]
        public string SupplierOrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        [Required]
        public float? BillAmount { get; set; }
        [Required]
        public float? PaidAmount { get; set; }

        public SupplierOrder() {
            this.OrderDate = DateTime.Now;
            this.DueDate = DateTime.Now;
        }

        public void SetDefaultValue() {
            this.SupplierOrderId = Guid.NewGuid();
            this.SupplierOrderNo = Utility.GenerateSupplierOrderNo();
        }

        [Required]
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}