using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HyperStoreService.Models;
using System.ComponentModel.DataAnnotations;

namespace HyperStoreServiceAPP.Controllers
{
    public partial class SupplierOrdersController : ApiController, SupplierOrderInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpGet]
        [ResponseType(typeof(List<SupplierOrder>))]
        public async Task<IHttpActionResult> Get(SupplierOrderFilterCriteria SOFC)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (SOFC == null)
                return BadRequest("A filter criteria object should not be null for retrieving list of supplier orders");
            if (SOFC.DueDateRange.LB < SOFC.OrderDateRange.LB)
                return BadRequest(String.Format("Order DueDate {0} Cannot be less than OrdeDate {1}",
                    SOFC.DueDateRange.LB, SOFC.OrderDateRange.LB));
            List<SupplierOrder> result;

            try
            {
                var query = db.SupplierOrders.Where(order => order.OrderDate >= SOFC.OrderDateRange.LB.Date &&
                                                             order.OrderDate <= SOFC.OrderDateRange.UB.Date &&
                                                             order.DueDate >= SOFC.DueDateRange.LB.Date &&
                                                             order.DueDate <= SOFC.DueDateRange.UB.Date
                                                             )
                                                             .Include(so => so.Supplier);
                if (SOFC.SupplierId != null)
                {
                    query = query.Where(order => order.SupplierId == SOFC.SupplierId);
                }

                if (SOFC.SupplierOrderNo != null)
                {
                    query = query.Where(order => order.SupplierOrderNo == SOFC.SupplierOrderNo);
                }

                if (SOFC.PartiallyPaidOrderOnly == true)
                {
                    query = query.Where(order => order.SettledPayedAmount < order.BillAmount);
                }

                result = await query.OrderByDescending(order => order.OrderDate).ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
            return Ok(result);
        }

        // POST: api/SupplierOrders
        /// <summary>
        /// 1. Increments the wallet balance of the supplier with creating a transaction entity associated with the supplier.
        /// 2. Creates a new supplier order.
        /// 3. Creates a new supplierorder transaction.
        /// 4. Updates the stock of each product purchased in orderDetail.
        /// 5. Creates supplierorderproduct entity for each product purchased in orderDetail.
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns>Using wallet amount</returns>
        [ResponseType(typeof(decimal))]
        [HttpPost]
        public async Task<IHttpActionResult> Post(SupplierOrderDTO orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (orderDetail == null)
                throw new Exception("OrderDetails should not have been null while placing the supplier order");
            if (orderDetail.DueDate < DateTime.Now)
                return BadRequest(String.Format("DueDate {0} cannot be before current Date {1}", orderDetail.DueDate, DateTime.Now.Date));
            var payingAmount = orderDetail.PayingAmount;
            var billAmount = orderDetail.SupplierBillingSummary.BillAmount;
            if (payingAmount > billAmount)
                return BadRequest(String.Format("Paying Amount {0} should be less than Bill Amount {1}", payingAmount, billAmount));
            //TODO: Verify bill amount.
            try
            {
                var supplierOrder = await CreateNewSupplierOrderAsync(orderDetail);
                SupplierTransactionDTO transactionDTO = new SupplierTransactionDTO
                {
                    IsCredit = true,
                    SupplierId = orderDetail.SupplierId,
                    TransactionAmount = billAmount - payingAmount,
                    Description = supplierOrder.SupplierOrderNo,
                };
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);

                var supplierOrderTransaction = CreateNewSupplierOrderTransaction(supplierOrder, transaction);

                var products = await UpdateStockOfProductsAsync(orderDetail.ProductsPurchased);

                AddIntoSupplierOrderProduct(orderDetail.ProductsPurchased, supplierOrder.SupplierOrderId);

                await db.SaveChangesAsync();
                return Ok(transactionDTO.TransactionAmount);
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierOrderExists(Guid? id)
        {
            return db.SupplierOrders.Count(e => e.SupplierOrderId == id) > 0;
        }
    }

    public partial class SupplierOrdersController : ApiController, SupplierOrderInterface
    {
        private SupplierOrderTransaction CreateNewSupplierOrderTransaction(SupplierOrder supplierOrder, SupplierTransaction transaction)
        {
            var supplierOrderTransaction = new SupplierOrderTransaction
            {
                SupplierOrderTransactionId = Guid.NewGuid(),
                TransactionId = transaction.SupplierTransactionId,
                SupplierOrderId = supplierOrder.SupplierOrderId,
                IsPaymentComplete = supplierOrder.BillAmount == supplierOrder.SettledPayedAmount ? true : false,
                PaidAmount = null
            };
            db.SupplierOrderTransactions.Add(supplierOrderTransaction);
            return supplierOrderTransaction;
        }

        private async Task<SupplierOrder> CreateNewSupplierOrderAsync(SupplierOrderDTO orderDetail)
        {
            var billAmount = orderDetail.SupplierBillingSummary.BillAmount;
            var payingAmount = (decimal)orderDetail.PayingAmount;

            var supplier = await db.Suppliers.FindAsync(orderDetail.SupplierId);
            if (supplier == null)
                throw new Exception(String.Format("Supplier with Id {0} does not exist", orderDetail.SupplierId));
            decimal payedAmountByWallet = 0;
            // If Supplier owes the Retailer.
            if (supplier.WalletBalance < 0)
                payedAmountByWallet = Math.Min(Math.Abs(supplier.WalletBalance), (decimal)(billAmount - payingAmount));

            var settledPayedAmount = payingAmount + payedAmountByWallet;

            if (settledPayedAmount > billAmount)
                throw new Exception(String.Format("Settled Payed Amount {0} can never be less than bill amount {1}", settledPayedAmount, billAmount));

            var supplierOrder = new SupplierOrder
            {
                SupplierOrderId = Guid.NewGuid(),
                DueDate = (DateTime)orderDetail.DueDate,
                InterestRate = orderDetail.IntrestRate,
                OrderDate = DateTime.Now,
                BillAmount = (decimal)orderDetail.SupplierBillingSummary.BillAmount,
                PayedAmount = payingAmount,
                PayedAmountByWallet = payedAmountByWallet,
                SettledPayedAmount = settledPayedAmount,
                SupplierOrderNo = Utility.GenerateSupplierOrderNo(),
                SupplierId = (Guid)orderDetail.SupplierId,
                TotalItems = (int)orderDetail.SupplierBillingSummary.TotalItems,
                TotalQuantity = (decimal)orderDetail.SupplierBillingSummary.TotalQuantity,
            };
            db.SupplierOrders.Add(supplierOrder);
            return supplierOrder;
        }

        private async Task<List<Product>> UpdateStockOfProductsAsync(List<ProductPurchased> productsPurchased)
        {
            List<Product> products = new List<Product>();
            try
            {
                foreach (var productPurchased in productsPurchased)
                {
                    var product = await UpdateProductStockAsync(productPurchased);
                    products.Add(product);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return products;
        }

        private async Task<Product> UpdateProductStockAsync(ProductPurchased productPurchased)
        {
            var product = await db.Products.FindAsync(productPurchased.ProductId);
            if (product == null)
                throw new Exception(String.Format("Product with id {0} not found while updating the stock", productPurchased.ProductId));
            product.TotalQuantity += (decimal)productPurchased.QuantityPurchased;
            db.Entry(product).State = EntityState.Modified;
            return product;
        }

        private void AddIntoSupplierOrderProduct(List<ProductPurchased> productsPurchased, Guid supplierOrderId)
        {
            // Not checking if product exists or not because it has been already checked by updateProductStock.
            foreach (var productPurchased in productsPurchased)
            {
                // Adding each product purchased in the order into the Entity SupplierOrderProduct.
                var supplierOrderProduct = new SupplierOrderProduct
                {
                    SupplierOrderProductId = Guid.NewGuid(),
                    PurchasePrice = (decimal)productPurchased.PurchasePricePerUnit,
                    QuantityPurchased = (decimal)productPurchased.QuantityPurchased,
                    SupplierOrderId = supplierOrderId,
                    ProductId = productPurchased.ProductId
                };
                db.SupplierOrderProducts.Add(supplierOrderProduct);
            }
        }
    }
}