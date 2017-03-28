using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptLink;

namespace CryptLinkTests {

    [TestFixture()]
    public class ObjectCacheTests {

        [Test()]
        public void DictionaryCacheBenchmark() {
            
            DictionaryCache d = new DictionaryCache() {
                AcceptingObjects = true,
                ManageEvery = new TimeSpan(0, 0, 0),
                ManageEveryIOCount = 0,
                MaxCollectionCount = 10000000,
                MaxCollectionSize = 1024 * 1024 * 1024,
                MaxExpiration = new TimeSpan(0, 0, 20),
                MaxObjectSize = 1024 * 1024
            };
            
            BenchmarkCache(new TimeSpan(0, 0, 30), new TimeSpan(0, 0, 30), d, 50, 50, 50);
        }

        [Test()]
        public void LightDBCacheBenchmark() {

            string dbPath = Utility.GetTempFilePath(".db");

            LiteDbCache d = new LiteDbCache() {
                ConnectionString = dbPath,
                AcceptingObjects = true,
                ManageEvery = new TimeSpan(0, 0, 15),
                ManageEveryIOCount = 50000,
                MaxCollectionCount = 1000000,
                MaxCollectionSize = 1024 * 1024 * 1024,
                MaxExpiration = new TimeSpan(0, 0, 20),
                MaxObjectSize = 1024 * 1024
            };

            BenchmarkCache(new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0), d, 50, 50, 50);

            System.IO.File.Delete(dbPath);
        }

        /// <summary>
        /// Does a simple benchmark on a IObjectCache
        /// </summary>
        /// <param name="TestLength">Number of seconds to test for</param>
        /// <param name="Cache">Cache object to work with</param>
        /// <param name="DeleteSkip">Deletes an item every X inserts</param>
        /// <param name="LookupSkip">Does a lookup every X inserts</param>
        public void BenchmarkCache(TimeSpan TestLength, TimeSpan CacheItemLife, IObjectCache Cache, int DeleteSkip, int LookupSkip, int UpdateSkip) {
            Cache.Initialize();
            //var r = new Random();
            

            DateTime start = DateTime.Now;
            DateTime testEnd = DateTime.Now.Add(TestLength);

            var firstItem = new HashableString("first " + Utility.GetRandomString(20));
            Cache.AddOrUpdate(firstItem.Hash, firstItem, new TimeSpan(99, 0, 0));

            Assert.True(Cache.Exists(firstItem.Hash), "Cache contains the last item inserted");

            long totalAdds = 0;

            var lastItem = new HashableString("test");

            while(DateTime.Now < testEnd) {

                if (Cache.AcceptingObjects == false) {
                    Assert.Fail("The cache is no longer accepting objects, adjust the test length, capacity or add a rollover cache.");
                }

                totalAdds += 1;
                lastItem = new HashableString(totalAdds.ToString());
                Cache.AddOrUpdate<HashableString>(lastItem.Hash, lastItem, CacheItemLife);

                if (totalAdds % LookupSkip == 0) {
                    Cache.Remove(lastItem.Hash);
                }

                if (totalAdds % UpdateSkip == 0) {
                    Cache.AddOrUpdate<HashableString>(lastItem.Hash, lastItem, CacheItemLife);
                }

                if (totalAdds % DeleteSkip == 0) {
                    Cache.Remove(lastItem.Hash);
                    Assert.False(Cache.Exists(lastItem.Hash), "Deleted item no longer exists");
                }
                
            }

            //var span = (DateTime.Now - start);
            Assert.True(Cache.Exists(lastItem.Hash), "Cache contains the first item inserted");
            Assert.NotNull(Cache.Get(lastItem.Hash), "Cache contains the first item inserted");
            Assert.True(Cache.Exists(firstItem.Hash), "Cache contains the last item inserted");
            Assert.NotNull(Cache.Get(firstItem.Hash), "Cache contains the first item inserted");

            Assert.Pass($"Added {totalAdds.ToString("N0")}, ended with: {Cache.CurrentCollectionCount.ToString("N0")}, IOPS: {Cache.TotalAverageIOPS().ToString("N1")}");

            Cache.Dispose();
        }

        [Test()]
        public void DictionaryCacheRollover() {
            int addCount = 1000000;

            DictionaryCache secondaryCache = new DictionaryCache() {
                AcceptingObjects = true,
                ManageEvery = new TimeSpan(0, 0, 15),
                ManageEveryIOCount = 50000,
                MaxCollectionCount = addCount,
                MaxCollectionSize = 1024 * 1024 * 1024,
                MaxExpiration = new TimeSpan(99, 0, 0),
                MaxObjectSize = 1024 * 1024
            };

            DictionaryCache mainCache = new DictionaryCache() {
                AcceptingObjects = true,
                ManageEvery = new TimeSpan(0, 0, 15),
                ManageEveryIOCount = 50000,
                MaxCollectionCount = addCount / 2,
                MaxCollectionSize = 1024 * 1024 * 1024,
                MaxExpiration = new TimeSpan(0, 0, 20),
                MaxObjectSize = 1024 * 1024,
                OverflowCache = secondaryCache
            };

            RolloverTest(mainCache, secondaryCache, addCount);
        }

        [Test()]
        public void DictionaryToLiteDBCacheRollover() {
            int addCount = 100000;
            string dbPath = Utility.GetTempFilePath(".db");

            LiteDbCache secondaryCache = new LiteDbCache() {
                ConnectionString = dbPath,
                AcceptingObjects = true,
                ManageEvery = new TimeSpan(0, 0, 15),
                ManageEveryIOCount = 50000,
                MaxCollectionCount = 1000000,
                MaxCollectionSize = 1024 * 1024 * 1024,
                MaxExpiration = new TimeSpan(0, 99, 0),
                MaxObjectSize = 1024 * 1024
            };

            DictionaryCache mainCache = new DictionaryCache() {
                AcceptingObjects = true,
                ManageEvery = new TimeSpan(0, 0, 15),
                ManageEveryIOCount = addCount / 10,
                MaxCollectionCount = addCount / 2,
                MaxCollectionSize = 1024 * 1024 * 1024,
                MaxExpiration = new TimeSpan(0, 0, 20),
                MaxObjectSize = 1024 * 1024,
                OverflowCache = secondaryCache
            };

            RolloverTest(mainCache, secondaryCache, addCount);

            System.IO.File.Delete(dbPath);
        }

        private void RolloverTest(IObjectCache MainCache, IObjectCache SecondaryCache, int AddCount) {

            SecondaryCache.Initialize();
            MainCache.Initialize();

            var firstItem = new HashableString("first " + Utility.GetRandomString(20));
            MainCache.AddOrUpdate(firstItem.Hash, firstItem, new TimeSpan(99, 0, 0));

            long totalAdds = 0;

            var lastItem = new HashableString("test");
            DateTime start = DateTime.Now;

            for (int i = 0; i < AddCount; i++) {
                totalAdds += 1;
                lastItem = new HashableString(i.ToString());
                MainCache.AddOrUpdate<HashableString>(lastItem.Hash, lastItem, new TimeSpan(1, 0, 0));
            }

            var span = (DateTime.Now - start);
            Assert.True(MainCache.Exists(lastItem.Hash) || SecondaryCache.Exists(lastItem.Hash));
            Assert.True(MainCache.Exists(firstItem.Hash) || SecondaryCache.Exists(firstItem.Hash));

            long ioTotal = MainCache.CurrentReadCount + MainCache.CurrentWriteCount + SecondaryCache.CurrentReadCount + SecondaryCache.CurrentWriteCount;
            double iops = ioTotal / span.TotalSeconds;

            Assert.Pass($"Added {totalAdds.ToString("N0")}, main cache contains: {MainCache.CurrentCollectionCount.ToString("N0")}, secondary cache contains: {SecondaryCache.CurrentCollectionCount.ToString("N0")}, IOPS: {iops.ToString("N1")}, took: {span.TotalMilliseconds.ToString("N0")}ms");
        }

    }
}
