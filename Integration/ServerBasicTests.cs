using CryptLink;
using Newtonsoft.Json;
using NUnit.Framework;
using ServiceStack;
using System;
using CryptLink.Services;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace CryptLinkTests {
	[TestFixture()]
	public class ServerBasicTests {

        string serverPath = "http://127.0.0.1:12345/";
        string format = "?format=json";

        [Test]
		public void ServerResponds() {
            try {
                var client = new System.Net.WebClient();

                //create a new service
                var serviceHost = new ServiceHostBase();                    

                //register the config
                serviceHost.Container.Register<ServiceConfig>(
                    ServiceConfigFactory.TestDefaults()
                );

                //start the service
                serviceHost
                    .Init()
                    .Start(serverPath);

                var response = client.DownloadString(serverPath + "version" + format);
				Assert.NotNull(response, "Service returned a string of: " + response.ToString());
                Assert.IsTrue(response.Contains("1.0.0.0"));

                serviceHost.Stop();
                serviceHost.Dispose();

            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }

		}

        [Test]
        public void ServerBasicStoresAndHashes() {

            try {
                var serviceHost = new ServiceHostBase();

                serviceHost.Container.Register<ServiceConfig>(ServiceConfigFactory.TestDefaults());

                serviceHost.Init().Start(serverPath);

                var hashableString = new HashableString("TEST", Hash.HashProvider.MD5);
                var url = serverPath + "store" + format;
                var nvc = new NameValueCollection() {
                       { "ExpireAt", new TimeSpan(0, 5, 0).ToString() },
                       { "SeralizedItem", hashableString.Value }
                   };

                StoreResponse decodedResponse = Utility.JsonDeseralizePostResponse<StoreResponse>(url, nvc);

                Assert.NotNull(decodedResponse, "Response was not deserialized");
                Assert.NotNull(decodedResponse.ItemHash, "Response was not server hashed");
                Assert.AreEqual(hashableString.Hash, decodedResponse.ItemHash, "Server hash matches local hash");

                serviceHost.Stop();
                serviceHost.Dispose();

            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }

        }

        [Test]
        public void ServerServiceStackStoresAndGets() {

            var serviceHost = new ServiceHostBase();
            serviceHost.Container.Register<ServiceConfig>(ServiceConfigFactory.TestDefaults());
            serviceHost.Init().Start(serverPath);

            var hashableString = new HashableString("TEST", Hash.HashProvider.MD5);

            var client = new JsonServiceClient(serverPath);
            var storeResponse = client.Post<StoreResponse>(new StoreRequest() {
                 ExpireAt = new TimeSpan(0, 5, 0),
                 SeralizedItem = hashableString.Value
            });

            Assert.AreEqual(hashableString.Hash, storeResponse.ItemHash, "Server hash matches local hash");

            var getResponse = client.Get <HashableString>(new GetRequest() {
                 ItemHash = hashableString.Hash
            });

            Assert.AreEqual(hashableString.Value, getResponse.Value, "Server returned object correctly");

            serviceHost.Stop();
            serviceHost.Dispose();
        }


        [Test, Category("Expensive")]
        public void ServerServiceStackBenchmark() {

            var serviceHost = new ServiceHostBase();
            serviceHost.Container.Register<ServiceConfig>(ServiceConfigFactory.TestDefaults());
            serviceHost.Init().Start(serverPath);

            int maxItems = 10000;
            var client = new JsonServiceClient(serverPath);

            DateTime start0 = DateTime.Now;

            //Make some items
            List<HashableString> items = new List<HashableString>();
            for (int i = 0; i <= maxItems; i++) {
                items.Add(new HashableString($"TEST-{i}", Hash.HashProvider.MD5));
            }

            DateTime end0 = DateTime.Now;
            DateTime start1 = DateTime.Now;

            //Store items
            foreach(var item in items){
                var storeResponse = client.Post<StoreResponse>(new StoreRequest() {
                    ExpireAt = new TimeSpan(0, 15, 0),
                    SeralizedItem = item.Value
                });

                Assert.AreEqual(item.Hash, storeResponse.ItemHash, "Server hash matches local hash");
            }

            DateTime end1 = DateTime.Now;
            DateTime start2 = DateTime.Now;

            //Get items
            foreach (var item in items) {

                var getItem = client.Get<HashableString>(new GetRequest() {
                    ItemHash = item.Hash
                });

                Assert.AreEqual(getItem.Hash, item.Hash);
            }

            DateTime end2 = DateTime.Now;

            serviceHost.Stop();
            serviceHost.Dispose();

            decimal createAvg = (decimal)(end0 - start0).TotalMilliseconds / (decimal)maxItems;
            decimal storeAvg = (decimal)(end1 - start1).TotalMilliseconds / (decimal)maxItems;
            decimal getAvg = (decimal)(end2 - start2).TotalMilliseconds / (decimal)maxItems;

            Assert.Pass($"Hashing average: {createAvg.ToString("N5")}ms, Store average: {storeAvg.ToString("N3")}ms, Get average: {getAvg.ToString("N3")}ms");
        }
    }
}

