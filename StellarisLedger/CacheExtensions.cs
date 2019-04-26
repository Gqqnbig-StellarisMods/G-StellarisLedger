using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace StellarisLedger
{
	public static class CacheExtensions
	{
		static readonly object lock_IsMachineEmpire = new object();

		public static bool GetOrUpdateTag0IsMachineEmpire(this IMemoryCache memoryCache, bool requireValidation, Func<bool> GetIsMachineEmpire, Func<DateTimeOffset> GetInGameDate)
		{
			var cacheOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromDays(1) };
			lock (lock_IsMachineEmpire)
			{
				if (memoryCache.TryGetValue("tag0.IsMachineEmpire", out Tuple<bool, DateTimeOffset> data))
				{
					if (requireValidation == false)
						return data.Item1;

					var date = GetInGameDate();
					if (data.Item2 < date)
					{
						bool value = GetIsMachineEmpire();
						memoryCache.Set("tag0.IsMachineEmpire", Tuple.Create(value, date), cacheOptions);
						return value;
					}
					else
						return data.Item1;
				}
				else
				{
					var value = GetIsMachineEmpire();
					memoryCache.Set("tag0.IsMachineEmpire", Tuple.Create(value, GetInGameDate()), cacheOptions);
					return value;
				}
			}
		}
	}
}
