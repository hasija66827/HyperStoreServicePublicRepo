using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{
    public class SupplierOrderDTO
    {
        [Required]
        public List<ProductPurchased> ProductsPurchased { get; set; }

        [Required]
        public Guid? SupplierId { get; set; }

        [Required]
        public DateTime? DueDate { get; set; }

        public SupplierBillingSummaryDTO SupplierBillingSummaryDTO { get; set; }

        [Required]
        public decimal? PayingAmount { get; set; }

        [Required]
        [Range(0d, 100)]
        public decimal? IntrestRate { get; set; }

        [Required]
        public EntityType? EntityType { get; set; }
    }

    public class ProductPurchased
    {
        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public decimal? QuantityPurchased { get; set; }

        [Required]
        public decimal? PurchasePricePerUnit { get; set; }
    }

    public class SupplierBillingSummaryDTO
    {
        [Required]
        public decimal? BillAmount { get; set; }

        [Required]
        public int? TotalItems { get; set; }

        [Required]
        public decimal? TotalQuantity { get; set; }
    }

    public class SupplierOrderFilterCriteria
    {
        public Guid? SupplierId { get; set; }

        public string SupplierOrderNo { get; set; }

        [Required]
        public EntityType? EntityType { get; set; }

        [Required]
        public bool? PartiallyPaidOrderOnly { get; set; }

        [Required]
        [DateRange(ErrorMessage = "{0} is invalid, lb>ub")]
        public IRange<DateTime> OrderDateRange { get; set; }

        [Required]
        [DateRange(ErrorMessage = "{0} is invalid, lb>ub")]
        public IRange<DateTime> DueDateRange { get; set; }
    }
}