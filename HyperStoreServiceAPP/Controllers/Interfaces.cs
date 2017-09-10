using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace HyperStoreServiceAPP.Controllers
{
    public interface CustomerOrderInterface
    {
        Task<IHttpActionResult> Get(CustomerOrderFilterCriteria customerOrderFilterCriteria);
        Task<IHttpActionResult> Post(CustomerOrderDTO orderDetail);
    }

    public interface CustomerOrderDetailInterface
    {
        Task<IHttpActionResult> Get(Guid? id);
    }

    public interface CustomerInterface
    {
        Task<IHttpActionResult> Get(CustomerFilterCriteria cfc);
        Task<IHttpActionResult> Put(Guid id, CustomerDTO customerDTO);
        Task<IHttpActionResult> Post(CustomerDTO customerDTO);
    }

    public interface CustomerTransactionInterface
    {
        Task<IHttpActionResult> Get(CustomerTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> Post(CustomerTransactionDTO transactionDTO);
    };

    public interface TagInterface
    {
        IQueryable<Tag> Get();
        Task<IHttpActionResult> Post(TagDTO tagDTO);
    }

    public interface ProductInterface
    {
        Task<IHttpActionResult> Get(ProductFilterCriteria filterProductCriteria);
        Task<IHttpActionResult> Put(Guid id, Product product);
        Task<IHttpActionResult> Post(ProductDTO product);
    }

    public interface SupplierOrderDetailInterface
    {
         Task<IHttpActionResult> Get(Guid? supplierOrderId);
    }

    public interface SupplierOrderInterface
    {
        Task<IHttpActionResult> Get(SupplierOrderFilterCriteria SOFC);
        Task<IHttpActionResult> Post(SupplierOrderDTO orderDetail);
    }

    public interface SupplierControllerInterface
    {
        Task<IHttpActionResult> Get(SupplierFilterCriteria sfc);
        Task<IHttpActionResult> Put(Guid id, SupplierDTO supplierDTO);
        Task<IHttpActionResult> Post(SupplierDTO supplierDTO);
    }

    public interface SupplierTransactionInterface
    {
        Task<IHttpActionResult> Get(SupplierTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> Post(SupplierTransactionDTO transactionDTO);
    }

    public interface CustomerPurchaseTrendInterface
    {
        Task<IHttpActionResult> Get(CustomerPurchaseTrendDTO parameter);
    }

    public interface ProductTrendInterface
    {
        Task<IHttpActionResult> Get(ProductConsumptionTrendDTO parameter);
    }
}