using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO;
using HyperStoreServiceAPP.DTO.CartManagementDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace HyperStoreServiceAPP.Controllers
{
    public interface ICart {
        IHttpActionResult GetProductsInCart(Guid userId, Guid? supplierId);
        Task<IHttpActionResult> AddProductToCart(Guid userId, CartDTO cartDTO);
        Task<IHttpActionResult> AddLeftOverDeficientProductsToCart(Guid userId);
        Task<IHttpActionResult> RemoveProductFromCart(Guid userId, Guid productId);
        Task<IHttpActionResult> PurchaseProductInCart(Guid userId, Guid productId);
        Task<IHttpActionResult> UnPurchaseProductInCart(Guid userId, Guid productId);
        Task<IHttpActionResult> EmptyShoppingCart(Guid userId, Guid supplierId);
        Task<IHttpActionResult> EmptyAllShoppingCart(Guid userId);
    }

    public interface ITag
    {
        IQueryable<Tag> Get(Guid userId);
        Task<IHttpActionResult> Post(Guid userId, TagDTO tagDTO);
    }

    public interface IProduct
    {
        Task<IHttpActionResult> Get(Guid userId, ProductFilterCriteria filterProductCriteria);
        IHttpActionResult GetTotalRecordsCount(Guid userId);
        Task<IHttpActionResult> Post(Guid userId, ProductDTO product);
        Task<IHttpActionResult> GetProductMetadata(Guid userId);
    }

    public interface IOrderProduct
    {
        Task<IHttpActionResult> Get(Guid userId, Guid? supplierOrderId);
    }

    public interface IOrder
    {
        Task<IHttpActionResult> Get(Guid userId, SupplierOrderFilterCriteria SOFC);
        IHttpActionResult GetTotalRecordsCount(Guid userId);
        Task<IHttpActionResult> Post(Guid userId, SupplierOrderDTO orderDetail);
    }

    public interface IPerson
    {
        Task<IHttpActionResult> Get(Guid userId, SupplierFilterCriteria sfc);
        IHttpActionResult GetTotalRecordsCount(Guid userId);
        Task<IHttpActionResult> Put(Guid userId, Guid id, SupplierDTO supplierDTO);
        Task<IHttpActionResult> Post(Guid userId, SupplierDTO supplierDTO);
        Task<IHttpActionResult> GetWalletBalanceRange(Guid userId);
    }

    public interface ITransaction
    {
        Task<IHttpActionResult> Get(Guid userId, SupplierTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> Post(Guid userId, SupplierTransactionDTO transactionDTO);
    }

    public interface ICustomerPurchaseTrend
    {
        Task<IHttpActionResult> Get(Guid userId, CustomerPurchaseTrendDTO parameter);
    }

    public interface IProductTrend
    {
        Task<IHttpActionResult> Get(Guid userId, ProductConsumptionTrendDTO parameter);
    }

    public interface IRecommendedProduct
    {
        Task<IHttpActionResult> Get(Guid userId, Guid id);
    }
    public interface IPriceQuotedBySupplier
    {
        Task<IHttpActionResult> Get(Guid userId, Guid id);
    }
}