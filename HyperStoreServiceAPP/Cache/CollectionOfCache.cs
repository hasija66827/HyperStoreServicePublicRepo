using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.InMemoryStorage
{
    public class CollectionOfCache<T>
    {
        //maps the cache with the userId.
        private static Dictionary<Guid, Cache<T>> _Caches;

        static CollectionOfCache()
        {
            _Caches = new Dictionary<Guid, Cache<T>>();
        }

        public static bool InsertCache(Guid userId, Cache<T> cache)
        {
            if (_Caches.ContainsKey(userId))
                return false;

            _Caches.Add(userId, cache);
            return true;
        }

        public static Cache<T> GetCache(Guid userId)
        {
            Cache<T> cache;
            _Caches.TryGetValue(userId, out cache);
            return cache;
        }
    }
}