using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace HyperStoreServiceAPP.Controllers
{
    public interface ICustomerOrder
    {
        Task<IHttpActionResult> Get(Guid userId, CustomerOrderFilterCriteria customerOrderFilterCriteria);
        Task<IHttpActionResult> Post(Guid userId, CustomerOrderDTO orderDetail);
    }

    public interface ICustomerOrderDetail
    {
        Task<IHttpActionResult> Get(Guid userId, Guid? id);
    }

    public interface ICustomer
    {
        Task<IHttpActionResult> Get(Guid userId, CustomerFilterCriteria cfc);
        Task<IHttpActionResult> Put(Guid userId, Guid id, CustomerDTO customerDTO);
        Task<IHttpActionResult> Post(Guid userId, CustomerDTO customerDTO);
        Task<IHttpActionResult> GetWalletBalanceRange(Guid userId);
    }

    public interface ICustomerTransaction
    {
        Task<IHttpActionResult> Get(Guid userId, CustomerTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> Post(Guid userId, CustomerTransactionDTO transactionDTO);
    };

    public interface ITag
    {
        IQueryable<Tag> Get(Guid userId);
        Task<IHttpActionResult> Post(Guid userId, TagDTO tagDTO);
    }

    public interface IProduct
    {
        Task<IHttpActionResult> Get(Guid userId, ProductFilterCriteria filterProductCriteria);
        Task<IHttpActionResult> Put(Guid userId, Guid id, Product product);
        Task<IHttpActionResult> Post(Guid userId, ProductDTO product);
        Task<IHttpActionResult> GetProductMetadata(Guid userId);
    }

    public interface ISupplierOrderProduct
    {
        Task<IHttpActionResult> Get(Guid userId, Guid? supplierOrderId);
    }

    public interface ISupplierOrder
    {
        Task<IHttpActionResult> Get(Guid userId, SupplierOrderFilterCriteria SOFC);
        Task<IHttpActionResult> Post(Guid userId, SupplierOrderDTO orderDetail);
    }

    public interface ISupplier
    {
        Task<IHttpActionResult> Get(Guid userId, SupplierFilterCriteria sfc);
        Task<IHttpActionResult> Put(Guid userId, Guid id, SupplierDTO supplierDTO);
        Task<IHttpActionResult> Post(Guid userId, SupplierDTO supplierDTO);
        Task<IHttpActionResult> GetWalletBalanceRange(Guid userId);
    }

    public interface ISupplierTransaction
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