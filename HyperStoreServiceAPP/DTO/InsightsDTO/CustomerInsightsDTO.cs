using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.InsightsDTO
{
    public class CustomerInsightsDTO : InsightsDTO
    {
        public CustomerInsightsDTO(uint numberOfDays, uint numberOfRecords) : base(numberOfDays, numberOfRecords) { }
    }

    public class CustomerInsights
    {
        List<Person> _Customer;
        public List<Person> Customer { get { return this._Customer; } }

        public CustomerInsights(List<Person> customers)
        {
            _Customer = customers;
        }
    }

    public class DetachedCustomerInsights : CustomerInsights
    {
        int _detachedCustomerCount;
        public int DetachedCustomerCount { get { return this._detachedCustomerCount; } }

        public DetachedCustomerInsights(int detachedCustomerCount, List<Person> customers) : base(customers)
        {
            _detachedCustomerCount = detachedCustomerCount;
        }
    }

    public class NewCustomerInsights : CustomerInsights
    {
        int _newCustomerCount;
        public int NewCustomerCount { get { return this._newCustomerCount; } }

        public NewCustomerInsights(int newCustomerCount, List<Person> customer) : base(customer)
        {
            _newCustomerCount = newCustomerCount;
        }
    }
}