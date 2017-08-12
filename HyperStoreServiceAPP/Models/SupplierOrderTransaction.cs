using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class SupplierOrderTransaction
    {
        public Guid SupplierOrderTransactionId { get; set; }
        public float PaidAmount { get; set; }
        public bool IsPaymentComplete { get; set; }

        public SupplierOrderTransaction() { }

        [Required]
        public Guid TransactionId;
        public Transaction Transaction;

        [Required]
        public Guid SupplierOrderId;
        public SupplierOrder SupplierOrder;
    }
}