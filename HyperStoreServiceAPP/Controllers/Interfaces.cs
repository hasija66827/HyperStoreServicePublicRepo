using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO;
using HyperStoreServiceAPP.DTO.CartManagementDTO;
using HyperStoreServiceAPP.DTO.RecommendedProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace HyperStoreServiceAPP.Controllers
{
    public interface ICart {
        IHttpActionResult CompleteTheOrderInLiveCart(Guid userId);
        IHttpActionResult UpdateLiveCartMetadata(Guid userId, UpdateLiveCartDTO updateLiveCartDTO);
    }

    public interface ICartProduct {
        Task<IHttpActionResult> GetProductsInLiveCart(Guid userId, Guid personId);
        Task<IHttpActionResult> AddProductInLiveCart(Guid userId, PersonProductDTO PersonProductsDTO);
        Task<IHttpActionResult> RemoveProductFromLiveCart(Guid userId, PersonProductDTO PersonProductsDTO);
    }

    public interface IPurchaseHistory {
        IHttpActionResult Get(Guid userId);
        Task<IHttpActionResult> GetRecommendedProduct(Guid userId, Guid PersonId);
        Task<IHttpActionResult> SetReminderForProduct(Guid userId, SetReminderDTO setReminderDTO);
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

    public interface IPriceQuotedBySupplier
    {
        Task<IHttpActionResult> Get(Guid userId, Guid id);
    }
}