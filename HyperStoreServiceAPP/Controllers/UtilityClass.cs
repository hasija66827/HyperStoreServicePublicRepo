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

    public class CustomerOrderDTO
    {
        [Required]
        public List<ProductConsumed> ProductsConsumed { get; set; }

        [Required]
        public Guid? CustomerId { get; set; }

        [Required]
        public decimal? BillAmount { get; set; }

        [Required]
        public decimal? DiscountedAmount { get; set; }

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

    public class SupplierOrderDTO
    {
        [Required]
        public List<ProductPurchased> ProductsPurchased { get; set; }

        [Required]
        public Guid? SupplierId { get; set; }

        [Required]
        public decimal? BillAmount { get; set; }

        [Required]
        public decimal? PaidAmount { get; set; }

        [Required]
        public DateTime? DueDate { get; set; }

        [Required]
        [Range(0, 100)]
        public float IntrestRate { get; set; }
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
        public decimal? TransactionAmount { get; set; }

        /// <summary>
        /// 1. Updates the wallet balance of the supplier.
        /// 2. Creates a transaction entity associated with the supplier.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<SupplierTransaction> CreateNewTransactionAsync(HyperStoreServiceContext db)
        {
            var walletSnapshot = await this.UpdateSupplierWalletBalanceAsync(db);
            if (walletSnapshot == null)
                throw new Exception(String.Format("Supplier with id {0} not found", this.SupplierId));
            var transaction = this.AddNewTransaction((decimal)walletSnapshot, db);
            //if is credit==false then do settle up the orders.
            return transaction;
        }

        /// <summary>
        /// Updates the wallet balance of the supplier.
        /// Positive Balance always means that the user owes the supplier that much amount.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private async Task<decimal?> UpdateSupplierWalletBalanceAsync(HyperStoreServiceContext db)
        {
            Guid supplierId = (Guid)this.SupplierId;
            decimal transactionAmount = (decimal)this.TransactionAmount;
            bool IsCredit = (bool)this.IsCredit;
            var supplier = await db.Suppliers.FindAsync(supplierId);
            if (supplier == null)
                return null;
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
        /// <returns></returns>
        private SupplierTransaction AddNewTransaction(decimal walletSnapshot, HyperStoreServiceContext db)
        {
            var transaction = new SupplierTransaction
            {
                SupplierTransactionId = Guid.NewGuid(),
                TransactionNo = Utility.GenerateSupplierTransactionNo(),
                TransactionDate = DateTime.Now,
                TransactionAmount = (decimal)this.TransactionAmount,
                IsCredit = (bool)this.IsCredit,
                SupplierId = (Guid)this.SupplierId,
                WalletSnapshot = walletSnapshot
            };
            db.SupplierTransactions.Add(transaction);
            return transaction;
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
    #endregion
}