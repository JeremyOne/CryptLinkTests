﻿using NUnit.Framework;
using System;
using CryptLink;
using System.Linq;
using System.Collections.Generic;

namespace CryptLinkTests {
	[TestFixture()]
	public class HashTests {

        [Test()]
        public void HashSeralizeDeseralize() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = Hash.Compute("TEST", provider);
                var h1FromBytes = Hash.FromComputedBytes(h1.Bytes, provider, h1.SourceByteLength);
                var h1FromB64 = Hash.FromB64(Base64.EncodeBytes(h1.Bytes), provider, h1.SourceByteLength);

                Assert.AreEqual(h1.Bytes, h1FromBytes.Bytes, "Compared Bitwise");
                Assert.True(h1 == h1FromBytes, "Compared with equality");

                Assert.AreEqual(h1.Bytes, h1FromB64.Bytes, "Compared Bitwise");
                Assert.True(h1 == h1FromB64, "Compared with equality");
            }
        }

        [Test()]
        public void HashCreateLength() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = Hash.Compute("TEST", provider);
                
                var tooLong = h1.Bytes.Concat(BitConverter.GetBytes(true)).ToArray();
                Assert.Throws<ArgumentException>(delegate {
                    var h1TooLong = Hash.FromComputedBytes(tooLong, provider, h1.SourceByteLength);
                });

                var tooShort = h1.Bytes.Take(h1.Bytes.Length - 1).ToArray();
                Assert.Throws<ArgumentException>(delegate {
                    var h1TooShort = Hash.FromComputedBytes(tooShort, provider, h1.SourceByteLength);
                });

            }
        }


        [Test()]
        public void HashCompareString() {

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var hash1 = Hash.Compute("TEST", provider);
                var hash2 = Hash.Compute("TEST", provider);
                var hash3 = Hash.Compute("test", provider);
                var hash4 = Hash.Compute("", provider);

                Assert.AreEqual(hash1.Provider, provider,
                    "HashProvider is set correctly");

                Assert.AreEqual(hash1.Bytes, hash2.Bytes,
                    "'TEST' and 'TEST' hashes are equal for provider: '" + provider.ToString() + "'");
                Assert.AreNotEqual(hash2.Bytes, hash3.Bytes,
                    "'TEST' and 'test' hashes are NOT equal for provider: '" + provider.ToString() + "'");

                Assert.True(hash1.CompareTo(hash2) == 0,
                    "Separate hashes of the same string compare returns true for provider: '" + provider.ToString() + "'");

                Assert.False(hash1.CompareTo(hash3) == 0,
                    "Separate hashes of the different case strings returns false for provider: '" + provider.ToString() + "'");

                Assert.AreEqual(hash1.Rehash().Bytes, hash2.Rehash().Bytes,
                    "Rehash of the same strings returns true for provider: '" + provider.ToString() + "'");

            }
        }

        [Test()]
        public void HashToHashOperators() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = Hash.Compute("TEST", provider);
                var h2 = Hash.Compute("TEST", provider);
                var h3 = Hash.Compute("test", provider);

                byte[] maxBytes = new byte[h1.HashByteLength(true)];
                byte[] minBytes = new byte[h1.HashByteLength(true)];

                for (var i = 0; i < maxBytes.Length; i++) {
                    maxBytes[i] = 255;
                    minBytes[i] = 0;
                }

                Hash max = Hash.FromComputedBytes(maxBytes, provider, 0);
                Hash min = Hash.FromComputedBytes(minBytes, provider, 0);

                //All operators (hash to binary)
                Assert.True(h1 == h2,
                    "Operator '==' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");
                Assert.False(h1 == h3,
                    "Operator '==' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");

                Assert.False(h1 != h2,
                    "Operator '!=' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");
                Assert.True(h1 != h3,
                    "Operator '!=' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");

                Assert.True(h1 > min,
                    "Operator '>' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");
                Assert.True(h3 > min,
                    "Operator '>' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");

                Assert.True(h1 < max,
                    "Operator '<' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");
                Assert.True(h3 < max,
                    "Operator '<' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");

                Assert.True(h1 >= min,
                    "Operator '>=' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");
                Assert.True(h3 >= min,
                    "Operator '>=' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");

                Assert.True(h1 <= max,
                    "Operator '<=' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");
                Assert.True(h3 <= max,
                    "Operator '<=' compares correctly for provider (Hash to Hash): '" + provider.ToString() + "'");
            }
        }

        [Test()]
        public void HashToBinaryOperators() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = Hash.Compute("TEST", provider);
                var h2 = Hash.Compute("TEST", provider);
                var h3 = Hash.Compute("test", provider);

                byte[] max = new byte[h1.HashByteLength(true)];
                byte[] min = new byte[h1.HashByteLength(true)];

                for (var i = 0; i < max.Length; i++) {
                    max[i] = 255;
                    min[i] = 0;
                }

                //All operators (hash to binary)
                Assert.True(h1 == h2.Bytes,
                    "Operator '==' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");
                Assert.False(h1 == h3.Bytes,
                    "Operator '==' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");

                Assert.False(h1 != h2.Bytes,
                    "Operator '!=' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");
                Assert.True(h1 != h3.Bytes,
                    "Operator '!=' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");

                Assert.True(h1 > min,
                    "Operator '>' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");
                Assert.True(h3 > min,
                    "Operator '>' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");

                Assert.True(h1 < max,
                    "Operator '<' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");
                Assert.True(h3 < max,
                    "Operator '<' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");

                Assert.True(h1 >= min,
                    "Operator '>=' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");
                Assert.True(h3 >= min,
                    "Operator '>=' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");

                Assert.True(h1 <= max,
                    "Operator '<=' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");
                Assert.True(h3 <= max,
                    "Operator '<=' compares correctly for provider (Hash to binary): '" + provider.ToString() + "'");
            }
        }

        [Test()]
        public void BinaryToHashOperators() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = Hash.Compute("TEST", provider);
                var h2 = Hash.Compute("TEST", provider);
                var h3 = Hash.Compute("test", provider);

                byte[] maxBytes = new byte[h1.HashByteLength(true)];
                byte[] minBytes = new byte[h1.HashByteLength(true)];

                for (var i = 0; i < maxBytes.Length; i++) {
                    maxBytes[i] = 255;
                    minBytes[i] = 0;
                }

                Hash max = Hash.FromComputedBytes(maxBytes, provider, 0);
                Hash min = Hash.FromComputedBytes(minBytes, provider, 0);

                //All operators (hash to binary)
                Assert.True(h1.Bytes == h2,
                    "Operator '==' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");
                Assert.False(h1.Bytes == h3,
                    "Operator '==' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");

                Assert.False(h1.Bytes != h2,
                    "Operator '!=' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");
                Assert.True(h1.Bytes != h3,
                    "Operator '!=' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");

                Assert.True(h1.Bytes > min,
                    "Operator '>' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");
                Assert.True(h3.Bytes > min,
                    "Operator '>' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");

                Assert.True(h1.Bytes < max,
                    "Operator '<' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");
                Assert.True(h3.Bytes < max,
                    "Operator '<' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");

                Assert.True(h1.Bytes >= min,
                    "Operator '>=' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");
                Assert.True(h3.Bytes >= min,
                    "Operator '>=' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");

                Assert.True(h1.Bytes <= max,
                    "Operator '<=' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");
                Assert.True(h3.Bytes <= max,
                    "Operator '<=' compares correctly for provider (binary to Hash): '" + provider.ToString() + "'");
            }
        }

        [Test()]
        public void HashSorting() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                
                var h1 = Hash.Compute("1", provider);
                var h2 = Hash.Compute("2", provider);
                var h3 = Hash.Compute("3", provider);

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
        public void HashCompareToNull() {
            //Checking nulls with custom comparer can be tricky, here are all the ways I am aware of checking it

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                Hash hash1 = Hash.Compute("Test", provider);
                Hash hash2 = null;

                Assert.NotNull(hash1, "New hash is not null");
                Assert.True(hash2 == null, "New hash is not null using ==");
                Assert.False(ReferenceEquals(hash1, null), "New hash is not null using ReferenceEquals()");
                Assert.True(hash1 != (Hash)null, "New hash is not null");
                Assert.True(hash1 != default(Hash), "New hash is not null");
                Assert.True(hash1?.Bytes != null, "New hash is not null");

                var x = (hash2 == (Hash)null);

                Assert.Null(hash2, "Null hash is null");
                Assert.True(hash2 == null, "Null hash is null using ==");
                Assert.True(ReferenceEquals(hash2, null), "Null hash is null using ReferenceEquals()");
				Assert.True(hash2 == (Hash)null, "Null hash is null using (Hash)null");
				Assert.True(hash2 == default(Hash), "Null hash is null using default(Hash)");
                Assert.True(hash2?.Bytes == null, "Null hash is null using Hash?.Bytes");
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

