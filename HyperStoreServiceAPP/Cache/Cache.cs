using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HyperStoreServiceAPP.InMemoryStorage
{
    class RefreshInterval
    {
        public const int ProductEstimatedConsumption = 24;
    }

    public delegate Task<T> ComputeValue<T>(Guid userId);
    public class Cache<T>
    {
        private Guid _userId;
        private int _refreshInterval;//in number of hours
        private ComputeValue<T> _ComputeValue;
        private T _value;
        private DateTime _lastUpdated;

        public Cache(Guid userId, int refreshInterval, ComputeValue<T> computeValue)
        {
            this._userId = userId;
            this._refreshInterval = refreshInterval;
            this._ComputeValue = computeValue;
            this._lastUpdated = DateTime.Now;
            this._value = default(T);
        }

        public async Task<T> GetValue()
        {
            if (this._value == null || _lastUpdated.AddHours(_refreshInterval) <= DateTime.Now)
            {
                _value = await _ComputeValue.Invoke(_userId);
                _lastUpdated = DateTime.Now;
            }
            return _value;
        }
    }
}