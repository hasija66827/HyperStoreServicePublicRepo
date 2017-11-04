using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP
{
    public class Utility
    {
        public static string GenerateSupplierTransactionNo(int? length = null)
        {
            if (length == null)
                length = 7;
            var random = new Random();
            string s = "TXN";
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
            string s = "ORD";
            s = String.Concat(s, random.Next(1, 10).ToString());
            for (int i = 1; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        public static bool IsErrorAcceptable(decimal? error)
        {
            if (error == null)
                throw new Exception("error should not be null");
            if (error > 1 || error < -1)
                return false;
            return true;
        }
    }
}