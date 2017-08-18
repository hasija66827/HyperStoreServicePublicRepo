﻿using System;
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
    public class ProductPurchased
    {
        [Required]
        public Guid? ProductId { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float? QuantityPurchased { get; set; }
        [Required]
        public decimal? PurchasePricePerUnit { get; set; }
    }

    public class SupplierOrderDTO
    {
        [Required]
        public List<ProductPurchased> ProductsPurchased { get; set; }
        [Required]
        public Guid? SupplierId { get; set; }
        [Required]
        public decimal? BillAmount { get; set; }
        [Required]
        public decimal? PaidAmount { get; set; }
        [Required]
        public DateTime? DueDate { get; set; }
        [Required]
        [Range(0, 100)]
        public float IntrestRate { get; set; }
    }

    public class SupplierOrdersController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/SupplierOrders
        public IQueryable<SupplierOrder> GetSupplierOrders()
        {
            return db.SupplierOrders;
        }

        // GET: api/SupplierOrders/5
        [ResponseType(typeof(SupplierOrder))]
        public async Task<IHttpActionResult> GetSupplierOrder(Guid id)
        {
            SupplierOrder supplierOrder = await db.SupplierOrders.FindAsync(id);
            if (supplierOrder == null)
            {
                return NotFound();
            }

            return Ok(supplierOrder);
        }

        // PUT: api/SupplierOrders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSupplierOrder(Guid id, SupplierOrder supplierOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplierOrder.SupplierOrderId)
            {
                return BadRequest();
            }

            db.Entry(supplierOrder).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
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
        /// <returns></returns>
        [ResponseType(typeof(SupplierOrder))]
        [HttpPost]
        public async Task<IHttpActionResult> PostSupplierOrder(SupplierOrderDTO orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (orderDetail == null)
                return BadRequest("OrderDetails should not have been null while placing the supplier order");
            if (orderDetail.PaidAmount > orderDetail.BillAmount)
                return BadRequest(String.Format("PaidAmount {0} should be less than Bill Amount {1}", orderDetail.PaidAmount, orderDetail.BillAmount));
            if (orderDetail.DueDate < DateTime.Now)
                return BadRequest(String.Format("DueDate{0} cannot be before current Date {1}", orderDetail.DueDate, DateTime.Now));
            //TODO: Verify bill amount.
            try
            {
                TransactionDTO transactionDTO = new TransactionDTO
                {
                    IsCredit = true,
                    SupplierId = orderDetail.SupplierId,
                    TransactionAmount = orderDetail.BillAmount - orderDetail.PaidAmount
                };
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);
                var supplierOrder = CreateNewSupplierOrder(orderDetail);
                var supplierOrderTransaction = CreateNewSupplierOrderTransaction(supplierOrder, transaction);
                var products=await UpdateStockOfProductsAsync(orderDetail.ProductsPurchased);
                AddIntoSupplierOrderProduct(orderDetail.ProductsPurchased, supplierOrder.SupplierOrderId);
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { id = supplierOrder.SupplierOrderNo }, products);
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        private SupplierOrderTransaction CreateNewSupplierOrderTransaction(SupplierOrder supplierOrder, Transaction transaction)
        {
            var supplierOrderTransaction = new SupplierOrderTransaction
            {
                SupplierOrderTransactionId = Guid.NewGuid(),
                TransactionId = transaction.TransactionId,
                SupplierOrderId = supplierOrder.SupplierOrderId,
                IsPaymentComplete = supplierOrder.BillAmount == supplierOrder.PaidAmount ? true : false,
                PaidAmount = null
            };
            db.SupplierOrderTransactions.Add(supplierOrderTransaction);
            return supplierOrderTransaction;
        }

        private SupplierOrder CreateNewSupplierOrder(SupplierOrderDTO orderDetail)
        {
            var supplierOrder = new SupplierOrder
            {
                SupplierOrderId = Guid.NewGuid(),
                BillAmount = (decimal)orderDetail.BillAmount,
                DueDate = (DateTime)orderDetail.DueDate,
                OrderDate = DateTime.Now,
                PaidAmount = (decimal)orderDetail.PaidAmount,
                SupplierOrderNo = Utility.GenerateSupplierOrderNo(),
                SupplierId = (Guid)orderDetail.SupplierId
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
            product.TotalQuantity += (float)productPurchased.QuantityPurchased;
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
                    QuantityPurchased = (float)productPurchased.QuantityPurchased,
                    SupplierOrderId = supplierOrderId,
                    ProductId = productPurchased.ProductId
                };
                db.SupplierOrderProducts.Add(supplierOrderProduct);
            }
        }

       

        // DELETE: api/SupplierOrders/5
        [ResponseType(typeof(SupplierOrder))]
        public async Task<IHttpActionResult> DeleteSupplierOrder(Guid id)
        {
            SupplierOrder supplierOrder = await db.SupplierOrders.FindAsync(id);
            if (supplierOrder == null)
            {
                return NotFound();
            }

            db.SupplierOrders.Remove(supplierOrder);
            await db.SaveChangesAsync();

            return Ok(supplierOrder);
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
}