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
        Task<IHttpActionResult> GetCustomerOrders(CustomerOrderFilterCriteria customerOrderFilterCriteria);
        Task<IHttpActionResult> PlaceCustomerOrder(CustomerOrderDTO orderDetail);
    }

    public interface CustomerOrderDetailInterface
    {
        IQueryable<CustomerOrderProduct> GetCustomerOrderProducts();
        Task<IHttpActionResult> GetCustomerOrderDetail(Guid? id);
    }

    public interface CustomerInterface
    {
        Task<IHttpActionResult> GetCustomers(CustomerFilterCriteria cfc);
        Task<IHttpActionResult> PutCustomer(Guid id, CustomerDTO customerDTO);
        Task<IHttpActionResult> PostCustomer(CustomerDTO customerDTO);
        Task<IHttpActionResult> DeleteCustomer(Guid id);
    }

    public interface CustomerTransactionInterface
    {
        Task<IHttpActionResult> GetTransactions(CustomerTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> PostCustomerTransaction(CustomerTransactionDTO transactionDTO);
    };

    public interface TagInterface
    {
        IQueryable<Tag> GetTags();
        Task<IHttpActionResult> GetTag(Guid id);
        Task<IHttpActionResult> PutTag(Guid id, Tag tag);
        Task<IHttpActionResult> PostTag(TagDTO tagDTO);
        Task<IHttpActionResult> DeleteTag(Guid id);
    }

    public interface ProductInterface
    {
        Task<IHttpActionResult> GetProducts(ProductFilterCriteria filterProductCriteria);
        Task<IHttpActionResult> PutProduct(Guid id, Product product);
        Task<IHttpActionResult> PostProduct(ProductDTO product);
        Task<IHttpActionResult> DeleteProduct(Guid id);
    }

    public interface SupplierOrderDetailInterface
    {
        IQueryable<SupplierOrderProduct> GetSupplierOrderProducts();
        Task<IHttpActionResult> GetSupplierOrderDetail(Guid? supplierOrderId);
    }

    public interface SupplierOrderInterface
    {
        Task<IHttpActionResult> GetSupplierOrders(SupplierOrderFilterCriteria SOFC);
        Task<IHttpActionResult> PostSupplierOrder(SupplierOrderDTO orderDetail);
        Task<IHttpActionResult> DeleteSupplierOrder(Guid id);
    }

    public interface SupplierControllerInterface
    {
        Task<IHttpActionResult> GetSuppliers(SupplierFilterCriteria sfc);
        Task<IHttpActionResult> GetSupplier(Guid id);
        Task<IHttpActionResult> PutSupplier(Guid id, Supplier supplier);
        Task<IHttpActionResult> PostSupplier(SupplierDTO supplierDTO);
        Task<IHttpActionResult> DeleteSupplier(Guid id);
    }

    public interface SupplierTransactionInterface
    {
        Task<IHttpActionResult> GetTransactions(SupplierTransactionFilterCriteria transactionFilterCriteria);
        Task<IHttpActionResult> PostTransaction(SupplierTransactionDTO transactionDTO);
    }

    public interface SupplierOrderTransactionInterface
    {
        Task<IHttpActionResult> GetTransactionsOfSupplierOrder(Guid supplierOrderId);
        Task<IHttpActionResult> GetSupplierOrdersOfTransaction(Guid transactionId);
    }

    public interface CustomerPurchaseTrendInterface
    {
        Task<IHttpActionResult> CustomerPurchaseTrend(CustomerPurchaseTrendDTO parameter);
    }

    public interface ProductTrendInterface
    {
        Task<IHttpActionResult> GetProductTrend(ProductConsumptionTrendDTO parameter);
    }
}