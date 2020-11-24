using System;
using System.Threading.Tasks;
using Amazon.AppConfig.Model;
using Microsoft.Extensions.Caching.Memory;

namespace AwsAppConfigClient
{
    public static class Cache
    {
        private static readonly MemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

        public static T Get<T>(string key)
        {
            return MemoryCache.Get<T>(key);
        }

        public static T Set<T>(string key, T obj)
        {
            return MemoryCache.Set(key, obj);
        }

        public static T GetOrAdd<T>(string key, Func<T> objFactoryMethod, TimeSpan lifeTime)
        {
            return MemoryCache.GetOrCreate(key, entry =>
            {
                var objToCache = objFactoryMethod();
                entry.AbsoluteExpirationRelativeToNow = lifeTime;
                return objToCache;
            });
        }

        public static async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> objFactoryMethod, TimeSpan lifeTime)
        {
            return await MemoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = lifeTime;
                return await objFactoryMethod().ConfigureAwait(false);
            });
        }

        private class CacheItem
        {
            public object Object { get; set; }
        }
    }

    
}