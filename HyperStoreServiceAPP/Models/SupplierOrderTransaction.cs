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
        //It is amount paid from the transaction amount.
        //It is null, if the transaction was credit transaction o.w. it is less than equal to transaction amount.
        public decimal? PaidAmount { get; set; }

        public bool IsPaymentComplete { get; set; }

        public SupplierOrderTransaction()
        {
        }

        public Guid TransactionId { get; set; }
        public SupplierTransaction SupplierTransaction { get; set; }

        public Guid SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
    }
}