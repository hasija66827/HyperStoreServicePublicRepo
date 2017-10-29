using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{
    public class ProductDTO
    {
        [Required]
        [Range(0d, 100)]
        public decimal? CGSTPer { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal? MRP { get; set; }
        [Required]
        [Range(0d, 100)]
        public decimal? DiscountPer { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32? HSN { get; set; }
        [Required]
        [Range(0d, 100)]
        public decimal? SGSTPer { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public decimal? Threshold { get; set; }
        public List<Guid?> TagIds { get; set; }
    }

    public class ProductFilterCriteria
    {
        public Guid? ProductId { get; set; }
        public List<Guid?> TagIds { get; set; }
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
}