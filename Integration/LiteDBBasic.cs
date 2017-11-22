using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace CryptLinkTests {

    /// <summary>
    /// Some very basic tests for LiteDB
    /// </summary>
    [TestFixture]
    public class LiteDBBasic {

        [Test]
        public void LiteDbBasicIO() {
            var dbPath = CryptLink.Utility.GetTempFilePath("db");

            //create database (and file)
            var db = new LiteDatabase(dbPath);               
            var col = db.GetCollection<LiteItem>("Test");

            col.Insert(new LiteItem() { Name = "John" });
            col.Insert(new LiteItem() { Name = "Doe" });
            col.Insert(new LiteItem() { Name = "Joana" });
            col.Insert(new LiteItem() { Name = "Marcus" });

            db.Dispose();

            //reopen
            db = new LiteDatabase(dbPath);
            col = db.GetCollection<LiteItem>("Test");
            var p = col.Find(Query.All("Name", Query.Ascending));
            //var p = col.Find(X => X.Id < 100);
            Assert.AreEqual(4, p.Count());

            db.Dispose();

            //wait for a bit so the file system can catch up
            System.Threading.Thread.Sleep(1000);

            CryptLink.Utility.WaitDeleteFile(dbPath, new TimeSpan(0, 0, 30));
        }

        [Test]
        public void LiteDbHashableIO() {
            var dbPath = CryptLink.Utility.GetTempFilePath("db");

            //create database (and file)
            var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<CryptLink.HashableString>("Test");

            var insertItem = new CryptLink.HashableString("Test1", CryptLink.Hash.HashProvider.MD5);

            col.Insert(insertItem);
            col.Insert(new CryptLink.HashableString("Test2", CryptLink.Hash.HashProvider.MD5));
            col.Insert(new CryptLink.HashableString("Test3", CryptLink.Hash.HashProvider.MD5));
            col.Insert(new CryptLink.HashableString("Test4", CryptLink.Hash.HashProvider.MD5));

            db.Dispose();

            //reopen
            db = new LiteDatabase(dbPath);
            col = db.GetCollection<CryptLink.HashableString>("Test");

            var allItems = col.Find(Query.All(Query.Ascending));
            //var p = col.Find(X => X.Id < 100);
            Assert.AreEqual(4, allItems.Count());

            //var foundItem = col.Find(x => x.Hash == insertItem.Hash).FirstOrDefault();
            var foundItem = col.Find(x => x.HashBytes == insertItem.HashBytes).FirstOrDefault();
            Assert.NotNull(foundItem);
            Assert.AreEqual(foundItem.Hash, insertItem.Hash);

            //wait for a bit so the file system can catch up
            System.Threading.Thread.Sleep(1000);

            db.Dispose();

            System.IO.File.Delete(dbPath);
        }

        [Test, Category("Expensive")]
        public void LiteDbCacheItemIO() {
            var dbPath = CryptLink.Utility.GetTempFilePath("db");

            //create database (and file)
            var db = new LiteDatabase(dbPath);
            var col = db.GetCollection<CryptLink.CacheItem>("Test");

            for (int i = 0; i < 1000; i++) {
                var insertHashable = new CryptLink.HashableString(i.ToString(), CryptLink.Hash.HashProvider.MD5);
                var insertItem = new CryptLink.CacheItem(insertHashable.Hash, insertHashable, new TimeSpan(0));
                col.Insert(insertItem);

                var foundItem = col.FindOne(x => x.Key == insertItem.Key);
                Assert.NotNull(foundItem);
                Assert.AreEqual(foundItem.Key, insertItem.Key);

                Assert.True(col.Exists(x => x.Key == insertItem.Key), "Item exists");
            }

            var allItems = col.Find(Query.All(Query.Ascending));
            Assert.AreEqual(1000, allItems.Count());

            db.Dispose();

            System.IO.File.Delete(dbPath);
        }

    }



public class LiteItem {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
