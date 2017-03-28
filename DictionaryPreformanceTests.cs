using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace CryptLinkTests {
	
	[TestFixture()]
	public class DictionaryPreformanceTests {
		int iterations = 10000000;

        private string NormalizeName(string Name) {
            return Name.Trim().ToLowerInvariant();
        }

        [Test()]
		public void DictionaryString() {
            GC.Collect();
			var start = DateTime.Now;

			var d = new Dictionary<string, int>();

			for(int i = 0; i < iterations; i++) {
				d.Add(i.ToString(), i);
			}

			for(int i = iterations; i == 0; i--) {
				var name = i.ToString();
				d.ContainsKey(name);
				var r = d[name];
			}

			var took = (DateTime.Now - start);
            var size = GC.GetTotalMemory(false);
            
			Assert.Pass("Dictionary with string key took: " + took.TotalSeconds.ToString()
                + " binary size of: " + size.ToString("N0"));
		}

        [Test()]
        public void DictionaryStringNormalize() {
            GC.Collect();
            var start = DateTime.Now;

            var d = new Dictionary<string, int>();

            for (int i = 0; i < iterations; i++) {
                d.Add(NormalizeName(i.ToString()), i);
            }

            for (int i = iterations; i == 0; i--) {
                var name = NormalizeName(i.ToString());
                d.ContainsKey(name);
                var r = d[name];
            }

            var took = (DateTime.Now - start);
            var size = GC.GetTotalMemory(false);

            Assert.Pass("Dictionary with string key took: " + took.TotalSeconds.ToString()
                + " binary size of: " + size.ToString("N0"));
        }

        [Test()]
		public void DictionaryLongString() {
            GC.Collect();
            var start = DateTime.Now;

			var d = new Dictionary<string, int>();
			var longString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

			for(int i = 0; i < iterations; i++) {
				d.Add(i.ToString() + longString, i);
			}

			for(int i = iterations; i == 0; i--) {
				var name = i.ToString() + longString;
				d.ContainsKey(name);
				var r = d[name];
			}

			var took = (DateTime.Now - start);
            var size = GC.GetTotalMemory(false);

            Assert.Pass("Dictionary with long string key took: " + took.TotalSeconds.ToString()
                + " binary size of: " + size.ToString("N0"));
        }

		[Test()]
		public void DictionaryHashableSringStore() {
            GC.Collect();
            var start = DateTime.Now;

			var d = new Dictionary<byte[], int>();

			for(int i = 0; i < iterations; i++) {
				var ib = new CryptLink.HashableString(i.ToString());
				d.Add(ib.GetHash(CryptLink.Hash.HashProvider.SHA256).Bytes, i);
			}

			var took = (DateTime.Now - start);
            var size = GC.GetTotalMemory(false);

            Assert.Pass("Dictionary with HashableString generated hash key took: " + took.TotalSeconds.ToString()
                + " binary size of: " + size.ToString("N0"));
        }

        [Test()]
        public void DictionaryHashableSringGet() {
            GC.Collect();

            var d = new Dictionary<byte[], int>();

            for (int i = 0; i < iterations; i++) {
                var ib = new CryptLink.HashableString(i.ToString());
                d.Add(ib.GetHash(CryptLink.Hash.HashProvider.SHA256).Bytes, i);
            }

            var start = DateTime.Now;
            for (int i = iterations; i == 0; i--) {
                var ib = new CryptLink.HashableString(i.ToString());
                var ih = ib.GetHash(CryptLink.Hash.HashProvider.SHA256).Bytes;
                d.ContainsKey(ih);
            }

            var took = (DateTime.Now - start);
            var size = GC.GetTotalMemory(false);

            Assert.Pass("Dictionary with HashableString generated hash key took: " + took.TotalSeconds.ToString()
                + " binary size of: " + size.ToString("N0"));
        }

        [Test()]
		public void DictionaryHash() {
            GC.Collect();
            var start = DateTime.Now;

			var d = new Dictionary<byte[], int>();

			for(int i = 0; i < iterations; i++) {
				var ib = CryptLink.Hash.Compute(BitConverter.GetBytes(i), CryptLink.Hash.HashProvider.SHA256).Bytes;
				d.Add(ib, i);
			}

			for(int i = iterations; i == 0; i--) {
				var ib = CryptLink.Hash.Compute(BitConverter.GetBytes(i), CryptLink.Hash.HashProvider.SHA256).Bytes;
				d.ContainsKey(ib);
			}

			var took = (DateTime.Now - start);
            var size = GC.GetTotalMemory(false);

            Assert.Pass("Dictionary with Hash generated key took: " + took.TotalSeconds.ToString()
                + " binary size of: " + size.ToString("N0"));
        }

		[Test()]
		public void DictionaryByte() {
            GC.Collect();
            var start = DateTime.Now;

			var d = new Dictionary<byte[], int>();

			for(int i = 0; i < iterations; i++) {
				var ib = BitConverter.GetBytes(i);
				d.Add(ib, i);
			}

			for(int i = iterations; i == 0; i--) {
				var ib = BitConverter.GetBytes(i);
				d.ContainsKey(ib);
				var r = d[ib];
			}

			var took = (DateTime.Now - start);
            var size = GC.GetTotalMemory(false);

            Assert.Pass("Dictionary with byte key took: " + took.TotalSeconds.ToString()
                + " binary size of: " + size.ToString("N0"));
        }
	}
}

