using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP
{
    public class Utility
    {
        public static string GenerateCustomerOrderNo(int? length = null)
        {
            if (length == null)
                length = 7;
            var random = new Random();
            string s = "CORD";
            s = String.Concat(s, random.Next(1, 10).ToString());
            for (int i = 1; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        public static string GenerateCustomerTransactionNo(int? length = null)
        {
            if (length == null)
                length = 7;
            var random = new Random();
            string s = "CTXN";
            s = String.Concat(s, random.Next(1, 10).ToString());
            for (int i = 1; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        public static string GenerateSupplierTransactionNo(int? length = null)
        {
            if (length == null)
                length = 7;
            var random = new Random();
            string s = "STXN";
            s = String.Concat(s, random.Next(1, 10).ToString());
            for (int i = 1; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        public static string GenerateSupplierOrderNo(int? length = null)
        {
            if (length == null)
                length = 7;
            var random = new Random();
            string s = "SORD";
            s = String.Concat(s, random.Next(1, 10).ToString());
            for (int i = 1; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
    }
}