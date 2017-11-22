using NUnit.Framework;
using System;
using System.Collections.Generic;
using CryptLink;

namespace CryptLinkTests {
    [TestFixture]
    public class HashableTests {

        [Test]
        public void HashableIsHashable() {
            Assert.False(Hashable.IsHashable(this), "A HashableTest should not derive from the Hashable type");
            Assert.False(Hashable.IsHashable(new Hash()), "A Hash should not derive from the Hashable type");
            Assert.True(Hashable.IsHashable(new HashableString("Test", Hash.HashProvider.MD5)), "A HashableString should derive from the Hashable type");
        }

        [Test]
        public void HashableStringHashes() {

            var hashDictionary = new Dictionary<Hash.HashProvider, string>();
            hashDictionary.Add(Hash.HashProvider.MD5, "DLxmEfVUC9CAmjiNyVphWw==");
            hashDictionary.Add(Hash.HashProvider.SHA1, "ZAqyuuB77cTBY/Z5p0b3q3+10fo=");
            hashDictionary.Add(Hash.HashProvider.SHA256, "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=");
            hashDictionary.Add(Hash.HashProvider.SHA384, "e49GVAdrgOuWORHxnPrRqvQoXtSOgm9s3hsBp5qnP621RG5mf8T5BBd4LJEnBUDz");
            hashDictionary.Add(Hash.HashProvider.SHA512, "xu6eM89cZxWh0Uj9c/cxiIS0Gty5FgIeK8DoAKXF3Zf1FCF49q6IyP3Zjhr7DOTI0sVLXzezC32hmXuzOwuKMQ==");

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h = new HashableString("Test", provider);

                Assert.True(hashDictionary.ContainsKey(provider), "The stored test hash dictionary has a comparison Hash");

                var storedTestHash = hashDictionary[provider];
                var computedHashString = h.Hash.ToString();

                Assert.AreEqual(computedHashString, storedTestHash, "Computed and stored hash differ, the hash of 'Test' should never change");
            }
            
        }

    }
}
