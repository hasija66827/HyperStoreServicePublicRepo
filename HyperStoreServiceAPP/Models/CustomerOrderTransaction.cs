using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class CustomerOrderTransaction
    {
        public Guid CustomerOrderTransactionId { get; set; }
        public CustomerOrderTransaction()
        {
        }

        public Guid TransactionId { get; set; }
        public CustomerTransaction CustomerTransaction { get; set; }

        public Guid CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
    }
}