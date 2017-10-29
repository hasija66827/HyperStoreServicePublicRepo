using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{

    #region CustomerPurchaseTrend
    public class CustomerPurchaseTrendDTO
    {
        [Required]
        public Guid? CustomerId { get; set; }
        [Required]
        public int? MonthsCount { get; set; }
    }
    #endregion
    #region ProductConsumption Trend
    public class ProductConsumptionTrendDTO
    {
        [Required]
        public Guid? ProductId { get; set; }
        [Required]
        public int? MonthsCount { get; set; }
    }
    #endregion 
    #region CustomerOrder Controller
    public sealed class DateRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;
            var dateRange = value as IRange<DateTime>;
            return dateRange.LB <= dateRange.UB;
        }
    }
    public enum EntityType
    {
        Customer,
        Supplier
    }
    public class CustomerOrderFilterCriteria
    {
        public Guid? CustomerId { get; set; }
        public string CustomerOrderNo { get; set; }

        [Required]
        [DateRange(ErrorMessage = "{0} is invalid, lb>ub")]
        public IRange<DateTime> OrderDateRange { get; set; }
    }

    public class ProductConsumedDTO
    {
        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public decimal? QuantityConsumed { get; set; }
    }

    public class CustomerBillingSummaryDTO
    {
        [Required]
        public decimal? TotalQuantity { get; set; }

        [Required]
        public int? TotalItems { get; set; }

        [Required]
        public decimal? CartAmount { get; set; }

        [Required]
        public decimal? DiscountAmount { get; set; }

        [Required]
        public decimal? Tax { get; set; }

        [Required]
        public decimal? BillAmount { get; set; }
    }

    public class CustomerOrderDTO
    {
        [Required]
        public List<ProductConsumedDTO> ProductsConsumed { get; set; }

        [Required]
        public CustomerBillingSummaryDTO CustomerBillingSummaryDTO { get; set; }

        [Required]
        public Guid? CustomerId { get; set; }

        [Required]
        public DateTime? DueDate { get; set; }

        [Required]
        public decimal? PayingAmount { get; set; }

        [Required]
        [Range(0d, 100)]
        public decimal? IntrestRate { get; set; }


    }
    #endregion


    #region CustomerTransactionDTO
    public class CustomerTransactionFilterCriteria
    {
        [Required]
        public Guid? CustomerId { get; set; }
    }

    public class CustomerTransactionDTO
    {
        [Required]
        public Guid? CustomerId { get; set; }

        [Required]
        public bool? IsCredit { get; set; }

        [Required]
        [Range(0, 98765432198765)]
        public decimal? TransactionAmount { get; set; }

        public string Description { get; set; }

        [Required]
        public bool? IsCashbackTransaction { get; set; }

        /// <summary>
        /// 1. Updates the wallet balance of the supplier.
        /// 2. Creates a transaction entity associated with the supplier.
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Retruns the newly creaated transaction with the customer included in it.</returns>
        public async Task<CustomerTransaction> CreateNewTransactionAsync(HyperStoreServiceContext db)
        {
            var walletSnapshot = await this.UpdateCustomerWalletBalanceAsync(db);
            if (walletSnapshot == null)
                throw new Exception(String.Format("Customer with id {0} has null wallet balance", this.CustomerId));
            var transaction = this.AddNewTransaction(db, (decimal)walletSnapshot);
            List<CustomerOrder> settleUpOrders;
            //if (transaction.IsCredit == false)
            //TODO: settleUpOrders = SettleUpOrders(transaction, db);
            return transaction;
        }

        /// <summary>
        /// Updates the wallet balance of the customer.
        /// Positive Balance always means that the user owes the customer that much amount.
        /// </summary>
        /// <param name="db"></param>
        /// <returns>The wallet snapshot which was before this function updates it.</returns>
        private async Task<decimal?> UpdateCustomerWalletBalanceAsync(HyperStoreServiceContext db)
        {
            Guid customerId = (Guid)this.CustomerId;
            decimal transactionAmount = (decimal)this.TransactionAmount;
            var customer = await db.Customers.FindAsync(customerId);
            if (customer == null)
                throw new Exception(String.Format("Customer with id {0} not found while updating its wallet balance", this.CustomerId));
            var walletSnapshot = customer.WalletBalance;
            if (IsCredit == true)
                customer.WalletBalance -= transactionAmount;
            else
                customer.WalletBalance += transactionAmount;
            db.Entry(customer).State = EntityState.Modified;
            return walletSnapshot;
        }

        /// <summary>
        /// Creates a transaction entity associated with customer.
        /// </summary>
        /// <param name="walletSnapshot"></param>
        /// <param name="db"></param>
        /// <returns>Retruns the newly creaated transaction with the customer included in it.</returns>
        private CustomerTransaction AddNewTransaction(HyperStoreServiceContext db, decimal walletSnapshot)
        {
            var transaction = new CustomerTransaction
            {
                CustomerTransactionId = Guid.NewGuid(),
                CustomerOrderNo = this.Description,
                IsCredit = (bool)this.IsCredit,
                CustomerId = (Guid)this.CustomerId,
                TransactionAmount = (decimal)this.TransactionAmount,
                TransactionNo = Utility.GenerateCustomerTransactionNo(),
                WalletSnapshot = walletSnapshot,
                TransactionDate = DateTime.Now,
                IsCashbackTransaction = (bool)this.IsCashbackTransaction
            };
            db.CustomerTransactions.Add(transaction);
            return transaction;
        }
    }

    #endregion


    #region Customer Controller
    public class CustomerFilterCriteria
    {
        public IRange<decimal> WalletAmount { get; set; }
        public Guid? CustomerId { get; set; }
    }

    public class CustomerDTO
    {
        public string Address { get; set; }
        public string GSTIN { get; set; }
        [Required]
        [RegularExpression(@"[987]\d{9}")]
        public string MobileNo { get; set; }
        [Required]
        public string Name { get; set; }
    }

    #endregion

}