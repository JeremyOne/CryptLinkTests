using NUnit.Framework;
using System;
using CryptLink;

namespace CryptLinkTests {
	[TestFixture()]
	public class Base64Tests {

		[Test()]
		public void Base64EncodeDecodeNumbers() {
			var testBytes = BitConverter.GetBytes(123);
			var encodedBytes = Base64.EncodeBytes(testBytes);
			Assert.AreEqual(encodedBytes, "ewAAAA..");
			var decodedBytes = Base64.DecodeBytes(encodedBytes);
			Assert.AreEqual(testBytes, decodedBytes);

			testBytes = BitConverter.GetBytes(123.456789);
			encodedBytes = Base64.EncodeBytes(testBytes);
			Assert.AreEqual(encodedBytes, "CwvuBzzdXkA.");
			decodedBytes = Base64.DecodeBytes(encodedBytes);
			Assert.AreEqual(testBytes, decodedBytes);
		}

		[Test()]
		public void Base64EncodeDecodeHashes() {
			foreach(Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
				var testHash = Hash.Compute("Test String", provider);

				var encodedBytes = Base64.EncodeBytes(testHash.Bytes);
				var decodedBytes = Base64.DecodeBytes(encodedBytes);

				Assert.True(testHash == decodedBytes);
			}
		}
	}
}

