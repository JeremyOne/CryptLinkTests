using NUnit.Framework;
using System;
using System.Linq;
using CryptLink;

namespace CryptLinkTests {
    [TestFixture]
    public class ConsistantHashTests {

        [Test]
        public void ConsistantHashProviders() {
            var testSize = 50000;

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {

                DateTime startTime = DateTime.Now;
                var consistentHashList = new ConsistentHash<HashableString>(provider);
                Hash lastHash = new Hash();
                
                for (int i = 0; i < testSize; i++) {
                    var h = new HashableString(i.ToString());
                    lastHash = consistentHashList.Add(h, false, 0);
                }

                Assert.AreEqual(consistentHashList.AllNodes.Count, testSize, "Added all nodes in: " + (DateTime.Now - startTime).TotalMilliseconds + "MS");

                DateTime startTime2 = DateTime.Now;
                consistentHashList.UpdateKeyArray();

                Assert.AreEqual(consistentHashList.AllNodes.Count, testSize, "Updated key in: " + (DateTime.Now - startTime2).TotalMilliseconds + "MS");
                Assert.True(consistentHashList.ContainsNode(lastHash));
                
            }

        }

    }

}


