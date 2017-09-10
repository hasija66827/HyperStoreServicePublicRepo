using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HyperStoreServiceAPP.Controllers
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

    public class CustomerOrderFilterCriteria
    {
        public Guid? CustomerId { get; set; }
        public string CustomerOrderNo { get; set; }

        [Required]
        [DateRange(ErrorMessage = "{0} is invalid, lb>ub")]
        public IRange<DateTime> OrderDateRange { get; set; }
    }

    public class ProductConsumed
    {
        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public decimal? QuantityConsumed { get; set; }
    }

    public class CustomerBillingSummary
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
        public decimal? PayAmount { get; set; }
    }

    public class CustomerOrderDTO
    {
        [Required]
        public List<ProductConsumed> ProductsConsumed { get; set; }

        [Required]
        public CustomerBillingSummary CustomerBillingSummary { get; set; }

        [Required]
        public Guid? CustomerId { get; set; }

        
        [Required]
        public bool? IsPayingNow { get; set; }

        [Required]
        public bool? IsUsingWallet { get; set; }

        [Required]
        public decimal? PayingAmount { get; set; }
    }
    #endregion
    #region Product Controller
    public class ProductDTO
    {
        [Required]
        public decimal? CGSTPer { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal? DisplayPrice { get; set; }
        [Required]
        public decimal? DiscountPer { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32 RefillTime { get; set; }
        public decimal? SGSTPer { get; set; }
        public decimal Threshold { get; set; }
        public List<Guid?> TagIds { get; set; }
    }

    public class IRange<T>
    {
        [Required]
        public T LB { get; set; }
        [Required]
        public T UB { get; set; }
        public IRange(T lb, T ub)
        {
            LB = lb;
            UB = ub;
        }
    }
    public sealed class QuantityRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var quantityRange = value as IRange<decimal?>;
            return (quantityRange.LB >= 0 && quantityRange.LB <= quantityRange.UB);
        }
    }

    public sealed class DiscountPerRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var discountPerRange = value as IRange<decimal?>;
            bool valid = (discountPerRange.LB <= discountPerRange.UB && discountPerRange.LB >= 0 && discountPerRange.UB <= 100);
            return valid;
        }
    }

    public class ProductFilterCriteria
    {
        public Guid? ProductId { get; set; }
        public List<Guid?> TagIds { get; set; }
        [Required]
        public FilterProductQDT FilterProductQDT { get; set; }
    }

    public class FilterProductQDT
    {
        [Required]
        [DiscountPerRange]
        public IRange<decimal?> DiscountPerRange { get; set; }

        [Required]
        [QuantityRange]
        public IRange<decimal?> QuantityRange { get; set; }

        [Required]
        public bool? IncludeDeficientItemsOnly { get; set; }
    }
    #endregion
    #region SupplierOrder Controller
    public class ProductPurchased
    {
        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float? QuantityPurchased { get; set; }

        [Required]
        public decimal? PurchasePricePerUnit { get; set; }
    }

    public class SupplierBillingSummary
    {
        [Required]
        public decimal? BillAmount { get; set; }
        
        [Required]
        public int? TotalItems { get; set; }

        [Required]
        public decimal? TotalQuantity { get; set; }
    }

    public class SupplierOrderDTO
    {
        [Required]
        public List<ProductPurchased> ProductsPurchased { get; set; }

        [Required]
        public Guid? SupplierId { get; set; }
  
        [Required]
        public DateTime? DueDate { get; set; }

        public SupplierBillingSummary SupplierBillingSummary { get; set; }

        [Required]
        public decimal? PayingAmount { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal IntrestRate { get; set; }
    }

    public class SupplierOrderFilterCriteria
    {
        public Guid? SupplierId { get; set; }

        public string SupplierOrderNo { get; set; }

        [Required]
        public bool? PartiallyPaidOrderOnly { get; set; }

        [Required]
        [DateRange(ErrorMessage = "{0} is invalid, lb>ub")]
        public IRange<DateTime> OrderDateRange { get; set; }

        [Required]
        [DateRange(ErrorMessage = "{0} is invalid, lb>ub")]
        public IRange<DateTime> DueDateRange { get; set; }
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
            };
            db.CustomerTransactions.Add(transaction);
            return transaction;
        }
    }

    #endregion
    #region Supplier Transaction
    public class SupplierTransactionFilterCriteria
    {
        [Required]
        public Guid? SupplierId { get; set; }
    }

    public class SupplierTransactionDTO
    {
        [Required]
        public bool? IsCredit { get; set; }

        [Required]
        public Guid? SupplierId { get; set; }

        [Required]
        [Range(0, 98765432198765)]
        public decimal? TransactionAmount { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// 1. Updates the wallet balance of the supplier.
        /// 2. Creates a transaction entity associated with the supplier.
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Retruns the newly creaated transaction with the customer included in it.</returns>
        public async Task<SupplierTransaction> CreateNewTransactionAsync(HyperStoreServiceContext db)
        {
            var walletSnapshot = await this.UpdateSupplierWalletBalanceAsync(db);
            if (walletSnapshot == null)
                throw new Exception(String.Format("Supplier with id {0} ad null wallet balance", this.SupplierId));
            var transaction = this.AddNewTransaction(db, (decimal)walletSnapshot);
            List<SupplierOrder> settleUpOrders;
            if (transaction.IsCredit == false)
                settleUpOrders = SettleUpOrders(transaction, db);
            return transaction;
        }

        /// <summary>
        /// Updates the wallet balance of the supplier.
        /// Positive Balance always means that the user owes the supplier that much amount.
        /// </summary>
        /// <param name="db"></param>
        /// <returns>The wallet snapshot which was before this function updates it.</returns>
        private async Task<decimal?> UpdateSupplierWalletBalanceAsync(HyperStoreServiceContext db)
        {
            Guid supplierId = (Guid)this.SupplierId;
            decimal transactionAmount = (decimal)this.TransactionAmount;
            bool IsCredit = (bool)this.IsCredit;
            var supplier = await db.Suppliers.FindAsync(supplierId);
            if (supplier == null)
                throw new Exception(String.Format("Supplier with id {0} not found while updating its wallet balance", this.SupplierId));
            var walletSnapshot = supplier.WalletBalance;
            if (IsCredit == true)
                supplier.WalletBalance += transactionAmount;
            else
                supplier.WalletBalance -= transactionAmount;
            db.Entry(supplier).State = EntityState.Modified;
            return walletSnapshot;
        }

        /// <summary>
        /// Creates a transaction entity associated with supplier.
        /// </summary>
        /// <param name="walletSnapshot"></param>
        /// <param name="db"></param>
        /// <returns>Retruns the newly creaated transaction with the customer included in it.</returns>
        private SupplierTransaction AddNewTransaction(HyperStoreServiceContext db, decimal walletSnapshot)
        {
            var transaction = new SupplierTransaction
            {
                SupplierTransactionId = Guid.NewGuid(),
                TransactionNo = Utility.GenerateSupplierTransactionNo(),
                TransactionDate = DateTime.Now,
                TransactionAmount = (decimal)this.TransactionAmount,
                SupplierOrderNo = this.Description,
                IsCredit = (bool)this.IsCredit,
                SupplierId = (Guid)this.SupplierId,
                WalletSnapshot = walletSnapshot
            };
            db.SupplierTransactions.Add(transaction);
            return transaction;
        }

        private List<SupplierOrder> SettleUpOrders(SupplierTransaction transaction, HyperStoreServiceContext db)
        {
            List<SupplierOrder> settleUpSupplierOrder = new List<SupplierOrder>();
            if (transaction.IsCredit)
                throw new Exception(String.Format("While settling up the orders transaction {0} cannot be of type credit", transaction.SupplierTransactionId));
            var partiallyPaidOrders = db.SupplierOrders.Where(so => so.SupplierId == transaction.SupplierId &&
                                                                   so.BillAmount - so.SettledPayedAmount > 0)
                                                       .OrderBy(wo => wo.OrderDate);
            var debitTransactionAmount = transaction.TransactionAmount;
            foreach (var partiallyPaidOrder in partiallyPaidOrders)
            {
                if (debitTransactionAmount <= 0)
                    break;
                var remainingAmount = partiallyPaidOrder.BillAmount - partiallyPaidOrder.SettledPayedAmount;
                if (remainingAmount < 0)
                    throw new Exception(string.Format("Supplier OrderNo {0}, Amount remaining to be paid: {1} cannot be less than zero", partiallyPaidOrder.SupplierOrderNo, remainingAmount));
                decimal payingAmountForOrder = Math.Min(remainingAmount, debitTransactionAmount);
                debitTransactionAmount -= payingAmountForOrder;
                var IsOrderSettleUp = SettleUpOrder(partiallyPaidOrder, payingAmountForOrder, db);
                settleUpSupplierOrder.Add(partiallyPaidOrder);
                db.SupplierOrderTransactions.Add(new SupplierOrderTransaction
                {
                    SupplierOrderTransactionId = Guid.NewGuid(),
                    SupplierOrderId = partiallyPaidOrder.SupplierOrderId,
                    TransactionId = transaction.SupplierTransactionId,
                    PaidAmount = payingAmountForOrder,
                    IsPaymentComplete = IsOrderSettleUp
                });
            }
            return settleUpSupplierOrder;
        }

        private bool SettleUpOrder(SupplierOrder supplierOrder, decimal settleUpAmount, HyperStoreServiceContext db)
        {
            supplierOrder.SettledPayedAmount += settleUpAmount;
            db.Entry(supplierOrder).State = EntityState.Modified;
            if (supplierOrder.SettledPayedAmount == supplierOrder.BillAmount)
                return true;
            return false;
        }
    }
    #endregion
    #region Supplier Controller
    public class SupplierFilterCriteria
    {
        public IRange<decimal> WalletAmount { get; set; }
        public Guid? SupplierId { get; set; }
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
    #region TagController
    public class TagDTO
    {
        [Required]
        [StringLength(24)]
        public string TagName { get; set; }
    }
    #endregion
}