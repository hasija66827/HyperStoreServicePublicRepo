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
        Task<IHttpActionResult> Get(CustomerOrderFilterCriteria customerOrderFilterCriteria);
        Task<IHttpActionResult> Post(CustomerOrderDTO orderDetail);
    }

    public interface ICustomerOrderDetail
    {
        Task<IHttpActionResult> Get(Guid? id);
    }

    public interface ICustomer
    {
        Task<IHttpActionResult> Get(CustomerFilterCriteria cfc);
        Task<IHttpActionResult> Put(Guid id, CustomerDTO customerDTO);
        Task<IHttpActionResult> Post(CustomerDTO customerDTO);
        Task<IHttpActionResult> GetWalletBalanceRange();
    }

    public interface ICustomerTransaction
    {
        Task<IHttpActionResult> Get(CustomerTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> Post(CustomerTransactionDTO transactionDTO);
    };

    public interface ITag
    {
        IQueryable<Tag> Get();
        Task<IHttpActionResult> Post(TagDTO tagDTO);
    }

    public interface IProduct
    {
        Task<IHttpActionResult> Get(ProductFilterCriteria filterProductCriteria);
        Task<IHttpActionResult> Put(Guid id, Product product);
        Task<IHttpActionResult> Post(ProductDTO product);
    }

    public interface ISupplierOrderProduct
    {
        Task<IHttpActionResult> Get(Guid? supplierOrderId);
    }

    public interface ISupplierOrder
    {
        Task<IHttpActionResult> Get(SupplierOrderFilterCriteria SOFC);
        Task<IHttpActionResult> Post(SupplierOrderDTO orderDetail);
    }

    public interface ISupplier
    {
        Task<IHttpActionResult> Get(SupplierFilterCriteria sfc);
        Task<IHttpActionResult> Put(Guid id, SupplierDTO supplierDTO);
        Task<IHttpActionResult> Post(SupplierDTO supplierDTO);
        Task<IHttpActionResult> GetWalletBalanceRange();
    }

    public interface ISupplierTransaction
    {
        Task<IHttpActionResult> Get(SupplierTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> Post(SupplierTransactionDTO transactionDTO);
    }

    public interface ICustomerPurchaseTrend
    {
        Task<IHttpActionResult> Get(CustomerPurchaseTrendDTO parameter);
    }

    public interface IProductTrend
    {
        Task<IHttpActionResult> Get(ProductConsumptionTrendDTO parameter);
    }

    public interface IRecommendedProduct
    {
        Task<IHttpActionResult> Get(Guid id);
    }
}