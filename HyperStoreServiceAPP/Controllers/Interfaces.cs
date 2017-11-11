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
        IHttpActionResult GetProudctsInCart(Guid userId, ProductCartDTO productCartDTO);
        IHttpActionResult GetRecommendedProductForCustomer(Guid userId, Guid PersonId);
        IHttpActionResult AddProductToLiveCart(Guid userId, AddRemoveProduct_CartDTO AddRemoveProduct_CartDTO);
        IHttpActionResult RemoveProductFromLiveCart(Guid userId, AddRemoveProduct_CartDTO AddRemoveProduct_CartDTO);
        IHttpActionResult CompleteTheOrderInLiveCart(Guid userId);
        IHttpActionResult UpdateLiveCart(UpdateLiveCartDTO updateLiveCartDTO);
        IHttpActionResult RetrieveDeficientProductsOfSupplier(Guid userId, Guid PersonId);
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