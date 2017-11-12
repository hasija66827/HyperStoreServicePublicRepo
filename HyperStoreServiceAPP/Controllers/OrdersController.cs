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
using HyperStoreServiceAPP.DTO;

namespace HyperStoreServiceAPP.Controllers
{
    public partial class OrdersController : ApiController, IOrder
    {
        private HyperStoreServiceContext db;

        #region Read
        [HttpGet]
        [ResponseType(typeof(List<Order>))]
        public async Task<IHttpActionResult> Get(Guid userId, SupplierOrderFilterCriteria SOFC)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (SOFC == null)
                return BadRequest("A filter criteria object should not be null for retrieving list of supplier orders");
            if (SOFC.DueDateRange.LB < SOFC.OrderDateRange.LB)
                return BadRequest(String.Format("Order DueDate {0} Cannot be less than OrdeDate {1}",
                    SOFC.DueDateRange.LB, SOFC.OrderDateRange.LB));

            List<Order> result;
            db = UtilityAPI.RetrieveDBContext(userId);
            try
            {
                var query = db.Orders.Where(order => order.OrderDate >= SOFC.OrderDateRange.LB.Date &&
                                                             order.OrderDate <= SOFC.OrderDateRange.UB.Date &&
                                                             order.DueDate >= SOFC.DueDateRange.LB.Date &&
                                                             order.DueDate <= SOFC.DueDateRange.UB.Date &&
                                                             order.EntityType == SOFC.EntityType
                                                             )
                                                             .Include(so => so.Person);
                if (SOFC.SupplierId != null)
                {
                    query = query.Where(order => order.PersonId == SOFC.SupplierId);
                }

                if (SOFC.SupplierOrderNo != null)
                {
                    query = query.Where(order => order.OrderNo == SOFC.SupplierOrderNo);
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
        //TODO: NEED TO Modify
        [HttpGet]
        [ResponseType(typeof(Int32))]
        public IHttpActionResult GetTotalRecordsCount(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return Ok(db.Orders.Count());
        }
        #endregion
        #region Create
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
        public async Task<IHttpActionResult> Post(Guid userId, SupplierOrderDTO orderDetail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (orderDetail == null)
                throw new Exception("OrderDetails should not have been null while placing the order");

            if (orderDetail.DueDate < DateTime.Now)
                return BadRequest(String.Format("Due date {0} cannot be before current date {1}", orderDetail.DueDate, DateTime.Now.Date));

            var payingAmount = orderDetail.PayingAmount;
            var billAmount = orderDetail.BillingSummaryDTO.BillAmount;
            if (payingAmount > billAmount)
                return BadRequest(String.Format("Paying Amount {0} should be less than Bill Amount {1}", payingAmount, billAmount));

            db = UtilityAPI.RetrieveDBContext(userId);

            var IsBillingSummaryCorrect = await _ValidateBillingSummary(orderDetail);
            if (!IsBillingSummaryCorrect)
                throw new Exception("BillingSummary is not correct.");

            try
            {
                var supplierOrder = _CreateNewPersonOrderAsync(orderDetail);

                SupplierTransactionDTO transactionDTO = new SupplierTransactionDTO
                {
                    IsCredit = orderDetail.EntityType == EntityType.Supplier ? true : false,
                    SupplierId = orderDetail.SupplierId,
                    TransactionAmount = billAmount - payingAmount,
                    Description = supplierOrder.OrderNo,
                };
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);

                var supplierOrderTransaction = _CreateNewOrderTransaction(supplierOrder, transaction);

                var products = await _UpdateStockOfProductsAsync(orderDetail.ProductsPurchased, orderDetail.EntityType, orderDetail.SupplierId);

                _AddIntoOrderProduct(orderDetail.ProductsPurchased, supplierOrder.OrderId);

                await _UpdatePersonNetWorthAsync((Guid)orderDetail.SupplierId, (decimal)orderDetail.BillingSummaryDTO.BillAmount);
                var productIds = orderDetail.ProductsPurchased.Select(pp => pp.ProductId).ToList();
                await _UpdatePurchaseHistoryAsync((Guid)orderDetail.SupplierId, productIds);

                await db.SaveChangesAsync();
                return Ok(transactionDTO.TransactionAmount);
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }
        #endregion

        private async Task<bool> _ValidateBillingSummary(SupplierOrderDTO orderDetail)
        {
            if (orderDetail.BillingSummaryDTO.TotalItems != orderDetail.ProductsPurchased.Count)
                return false;

            var quantityErrorDiff = orderDetail.BillingSummaryDTO.TotalQuantity
                                    - orderDetail.ProductsPurchased.Sum(p => p.QuantityPurchased);
            if (!Utility.IsErrorAcceptable(quantityErrorDiff))
                return false;

            decimal? BillAmount = 0;

            foreach (var productPurchased in orderDetail.ProductsPurchased)
            {
                var product = await db.Products.FindAsync(productPurchased.ProductId);
                if (product == null)
                    throw new Exception(String.Format("Product with id {0} not found.", productPurchased.ProductId));
                BillAmount += productPurchased.PurchasePricePerUnit * productPurchased.QuantityPurchased;
            }
            var errorDiff = orderDetail.BillingSummaryDTO.BillAmount - BillAmount;
            if (!Utility.IsErrorAcceptable(errorDiff))
                return false;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierOrderExists(Guid? id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }

    /// <summary>
    /// Methods to be executed for placing the order.
    /// </summary>
    public partial class OrdersController : ApiController, IOrder
    {
        private OrderTransaction _CreateNewOrderTransaction(Order supplierOrder, Transaction transaction)
        {
            var orderTransaction = new OrderTransaction
            {
                OrderTransactionId = Guid.NewGuid(),
                TransactionId = transaction.TransactionId,
                OrderId = supplierOrder.OrderId,
                IsPaymentComplete = supplierOrder.BillAmount == supplierOrder.SettledPayedAmount ? true : false,
                PaidAmount = null
            };
            db.OrderTransactions.Add(orderTransaction);
            return orderTransaction;
        }

        private Order _CreateNewPersonOrderAsync(SupplierOrderDTO orderDetail)
        {
            var billAmount = orderDetail.BillingSummaryDTO.BillAmount;
            var payingAmount = (decimal)orderDetail.PayingAmount;

            var supplierOrder = new Order
            {
                EntityType = orderDetail.EntityType,
                OrderId = Guid.NewGuid(),
                DueDate = (DateTime)orderDetail.DueDate,
                InterestRate = (decimal)orderDetail.IntrestRate,
                OrderDate = DateTime.Now,
                BillAmount = (decimal)orderDetail.BillingSummaryDTO.BillAmount,
                PayedAmount = payingAmount,
                SettledPayedAmount = payingAmount,
                OrderNo = Utility.GenerateSupplierOrderNo(),
                PersonId = (Guid)orderDetail.SupplierId,
                TotalItems = (int)orderDetail.BillingSummaryDTO.TotalItems,
                TotalQuantity = (decimal)orderDetail.BillingSummaryDTO.TotalQuantity,
            };
            db.Orders.Add(supplierOrder);
            return supplierOrder;
        }

        private async Task<List<Product>> _UpdateStockOfProductsAsync(List<ProductPurchased> productsPurchased, EntityType? entityType, Guid? personId)
        {
            List<Product> products = new List<Product>();
            try
            {
                foreach (var productPurchased in productsPurchased)
                {
                    var product = await _UpdateProductStockAsync(productPurchased, entityType, personId);
                    products.Add(product);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return products;
        }

        private async Task<Product> _UpdateProductStockAsync(ProductPurchased productPurchased, EntityType? entityType, Guid? personId)
        {
            var product = await db.Products.FindAsync(productPurchased.ProductId);
            if (product == null)
                throw new Exception(String.Format("Product with id {0} not found while updating the stock.", productPurchased.ProductId));
            if (entityType == EntityType.Supplier)
            {
                product.TotalQuantity += (decimal)productPurchased.QuantityPurchased;
                product.LatestSupplierId = personId;
            }
            else
            {
                product.TotalQuantity -= (decimal)productPurchased.QuantityPurchased;
                if (product.TotalQuantity < DeficientStockHit.DeficientStockCriteria)
                {
                    _AddProductToDeficientStockHit(product);
                }
            }
            db.Entry(product).State = EntityState.Modified;
            return product;
        }

        private void _AddIntoOrderProduct(List<ProductPurchased> productsPurchased, Guid supplierOrderId)
        {
            // Not checking if product exists or not because it has been already checked by updateProductStock.
            foreach (var productPurchased in productsPurchased)
            {
                // Adding each product purchased in the order into the Entity SupplierOrderProduct.
                var supplierOrderProduct = new OrderProduct
                {
                    OrderProductId = Guid.NewGuid(),
                    PurchasePrice = (decimal)productPurchased.PurchasePricePerUnit,
                    QuantityPurchased = (decimal)productPurchased.QuantityPurchased,
                    OrderId = supplierOrderId,
                    ProductId = productPurchased.ProductId
                };
                db.OrderProducts.Add(supplierOrderProduct);
            }
        }
    }

    /// <summary>
    /// Analytics Method to be executed post order placement
    /// a)To Update the networth of the customer.
    /// b)Add the product in deficient hit.
    /// c)Add or update the product in recommended product.
    /// </summary>
    public partial class OrdersController : ApiController, IOrder
    {
        private async Task<decimal?> _UpdatePersonNetWorthAsync(Guid personId, decimal billAmount)
        {
            var person = await db.Persons.FindAsync(personId);
            if (person == null)
                throw new Exception(String.Format("Person with id {0} not found while updating its networth", personId));
            person.NetWorth += billAmount;
            person.LastVisited = DateTime.Now;
            db.Entry(person).State = EntityState.Modified;
            return person.NetWorth;
        }

        private void _AddProductToDeficientStockHit(Product product)
        {
            if (product == null)
                throw new Exception("Product should not have been null, while adding the product to DeficientStock\n");
            var currentDate = DateTime.Now.Date;
            var IsExist = db.DeficientStockHits.Count(p => p.ProductId == product.ProductId &&
                                                        p.TimeStamp == currentDate) > 0;

            // Preserving Unique key constraints on DeficientStockHit
            if (!IsExist)
            {
                var deficientStock = new DeficientStockHit()
                {
                    DeficientStockHitId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    QuantitySnapshot = product.TotalQuantity,
                    TimeStamp = DateTime.Now.Date,
                };
                db.DeficientStockHits.Add(deficientStock);
            }
        }

        private async Task _UpdatePurchaseHistoryAsync(Guid personId, List<Guid?> productIds)
        {
            foreach (var productId in productIds)
            {
                var productPurchased = await db.PurchaseHistory.Where(ph => ph.ProductId == productId && ph.PersonId == personId).FirstOrDefaultAsync();
                if (productPurchased == null)
                {
                    var newProductPurchased = new PurchaseHistory()
                    {
                        PurchaseHistoryId = Guid.NewGuid(),
                        PersonId = personId,
                        ProductId = productId,
                        ExpiryDays = null,
                        LatestPurchaseDate = DateTime.Now,
                    };
                    db.PurchaseHistory.Add(newProductPurchased);
                }
                else
                {
                    productPurchased.LatestPurchaseDate = DateTime.Now;
                    db.Entry(productPurchased).State = EntityState.Modified;
                }
            }
        }
    }
}