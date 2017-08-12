using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class SupplierOrderTransaction
    {
        [Required]
        public Guid? SupplierOrderTransactionId { get; set; }
        [Required]
        public float? PaidAmount { get; set; }
        [Required]
        public bool? IsPaymentComplete { get; set; }

        public SupplierOrderTransaction() {
        }

        [Required]
        public Guid? TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        [Required]
        public Guid? SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
    }
}