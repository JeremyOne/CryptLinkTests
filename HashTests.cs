using NUnit.Framework;
using System;
using CryptLink;
using System.Collections.Generic;

namespace CryptLinkTests {
	[TestFixture()]
	public class HashTests {

        [Test()]
        public void HashSeralizeDeseralize() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = new Hash("TEST", provider);
                var h1FromBytes = Hash.FromBinaryHash(h1.HashBytes, provider);
                var h1FromB64 = Hash.FromB64(Base64.EncodeBytes(h1.HashBytes), provider);

                Assert.AreEqual(h1.HashBytes, h1FromBytes.HashBytes, "Compared Bitwise");
                Assert.True(h1 == h1FromBytes, "Compared with equality");

                Assert.AreEqual(h1.HashBytes, h1FromB64.HashBytes, "Compared Bitwise");
                Assert.True(h1 == h1FromB64, "Compared with equality");
            }
        }

        [Test()]
        public void HashCompareString() {

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var hash1 = new Hash("TEST", provider);
                var hash2 = new Hash("TEST", provider);
                var hash3 = new Hash("test", provider);
                var hash4 = new Hash("", provider);

                Assert.AreEqual(hash1.Provider, provider,
                    "HashProvider is set correctly");

                Assert.AreEqual(hash1.HashBytes, hash2.HashBytes,
                    "'TEST' and 'TEST' hashes are equal for provider: '" + provider.ToString() + "'");
                Assert.AreNotEqual(hash2.HashBytes, hash3.HashBytes,
                    "'TEST' and 'test' hashes are NOT equal for provider: '" + provider.ToString() + "'");

                Assert.True(hash1.CompareTo(hash2) == 0,
                    "Separate hashes of the same string compare returns true for provider: '" + provider.ToString() + "'");
                Assert.True(hash1.CompareTo(hash3) != 0,
                    "Separate hashes of the different case strings returns false for provider: '" + provider.ToString() + "'");

                Assert.AreEqual(hash1.Rehash().HashBytes, hash2.Rehash().HashBytes,
                    "Rehash of the same strings returns true for provider: '" + provider.ToString() + "'");

                Assert.False(hash4.Valid,
                    "Hash of empty string is not valid for provider: '" + provider.ToString() + "'");

            }
        }

        [Test()]
        public void HashOperators() {
            foreach(Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {

                var h1 = new Hash("TEST", provider);
                var h2 = new Hash("TEST", provider);
                var h3 = new Hash("test", provider);

                //All operators (hash to hash)
                Assert.AreEqual(h1 == h2, true,
                    "Operator '==' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 == h3, false,
                    "Operator '==' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h2, false,
                    "Operator '!=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h3, true,
                    "Operator '!=' compares correctly for provider: '" + provider.ToString() + "'");

                Assert.AreEqual(h1 > h2, false,
                    "Operator '>' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 > h3, true,
                    "Operator '>' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h2, false,
                    "Operator '<' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h3, false,
                    "Operator '<' compares correctly for provider: '" + provider.ToString() + "'");

                Assert.AreEqual(h1 >= h2, true,
                    "Operator '>=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 >= h3, true,
                    "Operator '>=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h2, true,
                    "Operator '<=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h3, false,
                    "Operator '<=' compares correctly for provider: '" + provider.ToString() + "'");

            }
        }

        [Test()]
        public void HashBinaryOperators() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = new Hash("TEST", provider);
                var h2 = new Hash("TEST", provider);
                var h3 = new Hash("test", provider);

                //All operators (hash to binary)
                Assert.AreEqual(h1 == h2.HashBytes, true,
                    "Operator '==' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 == h3.HashBytes, false,
                    "Operator '==' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h2.HashBytes, false,
                    "Operator '!=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h3.HashBytes, true,
                    "Operator '!=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");

                Assert.AreEqual(h1 > h2.HashBytes, false,
                    "Operator '>' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 > h3.HashBytes, true,
                    "Operator '>' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h2.HashBytes, false,
                    "Operator '<' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h3.HashBytes, false,
                    "Operator '<' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");

                Assert.AreEqual(h1 >= h2.HashBytes, true,
                    "Operator '>=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 >= h3.HashBytes, true,
                    "Operator '>=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h2.HashBytes, true,
                    "Operator '<=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h3.HashBytes, false,
                    "Operator '<=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
            }
        }

        [Test()]
        public void HashSorting() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                
                var h1 = new Hash("1", provider);
                var h2 = new Hash("2", provider);
                var h3 = new Hash("3", provider);

                var hList = new List<Hash>();
                hList.Add(h1);
                hList.Add(h2);
                hList.Add(h3);

                Assert.IsNotEmpty(hList);
                Assert.AreEqual(hList.Count, 3);

                hList.Sort();

            }
        }

        [Test()]
        public void HashProviderToOID() {
            var providerOIDs = new Dictionary<Hash.HashProvider, string>();
            providerOIDs.Add(Hash.HashProvider.MD5, "1.2.840.113549.2.5");
            providerOIDs.Add(Hash.HashProvider.SHA1, "1.3.14.3.2.26");
            providerOIDs.Add(Hash.HashProvider.SHA256, "2.16.840.1.101.3.4.2.1");
            providerOIDs.Add(Hash.HashProvider.SHA384, "2.16.840.1.101.3.4.2.2");
            providerOIDs.Add(Hash.HashProvider.SHA512, "2.16.840.1.101.3.4.2.3");

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                Assert.True(providerOIDs.ContainsKey(provider), "Test dictionary contains providers.");

                var providerOIDLookup = providerOIDs[provider];
                var providerOID = Hash.GetOIDForProvider(provider);

                Assert.AreEqual(providerOIDLookup, providerOID);
            }
                
        }

    }

}

