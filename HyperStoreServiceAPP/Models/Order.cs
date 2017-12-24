﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HyperStoreServiceAPP.DTO;
using HyperStoreServiceAPP;

namespace HyperStoreService.Models
{
    public class Order
    {
        [Required]
        public EntityType? EntityType { get; set; }
        public Guid OrderId { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime OrderDate { get; set; }
        // Amount Payed on the order placement time.
        public decimal PayedAmount { get; set; }
        // Amount Settled Up Till date, always -leq BillAmount.
        public decimal SettledPayedAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalQuantity { get; set; }
        [Required]
        public string OrderNo { get; set; }
        public Guid PersonId { get; set; }
        public Person Person { get; set; }

        public Guid? PaymentOptionId { get; set; }
        public PaymentOption PaymentOption { get; set; }

    }
}