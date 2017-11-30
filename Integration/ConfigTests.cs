using CryptLink;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CryptLinkTests {

    [TestFixture]
    public class ConfigTests {
        [Test]
        public void ConfigCreateFile() {
            var tempConfig = CryptLink.Utility.GetTempFilePath(".json");
            var c = CryptLink.ServiceConfig.Load(tempConfig, true, null);
            
            Assert.NotNull(c);
            Assert.True(System.IO.File.Exists(tempConfig), "File was created");

            CryptLink.Utility.WaitDeleteFile(tempConfig, new TimeSpan(0, 0, 30));
            Assert.True(!System.IO.File.Exists(tempConfig), "File was successfully deleted");
        }

        [Test]
        public void ConfigFactoryTestDefaults() {
            var tempConfig = Utility.GetTempFilePath(".json");

            var c = ServiceConfigFactory.TestDefaults();

            Assert.NotNull(c);
            Assert.NotNull(c.ServerPeerInfo);
            Assert.NotNull(c.Swarm);

            Utility.WaitDeleteFile(tempConfig, new TimeSpan(0, 0, 30));
            Assert.True(!System.IO.File.Exists(tempConfig), "File was successfully deleted");
        }

        [Test]
        public void ConfigSaveLoad() {
            var tempConfig = Utility.GetTempFilePath(".json");

            var c = ServiceConfigFactory.TestDefaults();

            Assert.NotNull(c);
            Assert.NotNull(c.ServerPeerInfo);
            Assert.NotNull(c.Server);
            Assert.NotNull(c.Swarm);

            Assert.Throws<NullReferenceException>(() => {
                    c.Save();
                }
            );

            c.SetPath(tempConfig);
            c.Save();

            Assert.True(System.IO.File.Exists(tempConfig), "File was successfully created");

            var c2 = ServiceConfig.Load(tempConfig, false, null);

            Assert.NotNull(c2);
            Assert.NotNull(c2.ServerPeerInfo);
            Assert.NotNull(c2.Server);
            Assert.NotNull(c2.Swarm);

            Assert.AreEqual(c.Swarm.PublicAddress, c2.Swarm.PublicAddress);
            Assert.AreEqual(c.ServerPeerInfo.Cert, c2.ServerPeerInfo.Cert);
            Assert.AreEqual(c.Server.CertManager.Thumbprint, c2.Server.CertManager.Thumbprint);
            Assert.AreEqual(c.Swarm.CertManager.Thumbprint, c2.Swarm.CertManager.Thumbprint);

            Utility.WaitDeleteFile(tempConfig, new TimeSpan(0, 0, 30));
            Assert.True(!System.IO.File.Exists(tempConfig), "File was successfully deleted");
        }


        [Test]
        public void ConfigLoadError() {
            var tempConfig = Utility.GetTempFilePath(".json");

            var ex = Assert.Throws<System.IO.FileNotFoundException>(() => CryptLink.ServiceConfig.Load(tempConfig, false, null));
            Assert.NotNull(ex);

            Assert.False(System.IO.File.Exists(tempConfig), "File was not created");
        }

        [Test]
        public void CertManagerConstructorSaveLoad() {
            var testCert = new X509Certificate2Builder() {
                SubjectName = "CN=Test CA",
                KeyStrength = 1024
            }.Build();

            //make a "secure" string
            var sString = new SecureString();
            sString.AppendChar('t');
            sString.AppendChar('e');
            sString.AppendChar('s');
            sString.AppendChar('t');
            sString.MakeReadOnly();

            var certMan1 = new CertificateManager(); //no certificate assigned
            var certMan2 = new CertificateManager(testCert); //a certificate with no password assigned
            var certMan3 = new CertificateManager(testCert, sString); //a certificate and password assigned

            Assert.Throws<NullReferenceException>(() => {
                var shouldThrow = certMan1.Thumbprint;
              }
            );

            //Both thumbprints should have a value
            Assert.IsNotEmpty(certMan2.Thumbprint);
            Assert.IsNotEmpty(certMan3.Thumbprint);
            
            //The serialized cert should NOT be the same between the plain-text and encrypted version
            Assert.AreNotEqual(certMan2.CertificateBase64, certMan3.CertificateBase64);

            var certMan2Json = Newtonsoft.Json.JsonConvert.SerializeObject(certMan2);
            var certMan3Json = Newtonsoft.Json.JsonConvert.SerializeObject(certMan3);

            var certMan2Desc = Newtonsoft.Json.JsonConvert.DeserializeObject<CertificateManager>(certMan2Json);
            var certMan3Desc = Newtonsoft.Json.JsonConvert.DeserializeObject<CertificateManager>(certMan3Json);

            //plain text cert should deserialize with no interaction
            Assert.IsTrue(certMan2Desc.CheckCertificate());

            //password protected cert should throw if no password is provided
            Assert.Throws<NullReferenceException>(() => {
                certMan3Desc.CheckCertificate();
                }
            );

            //decrypt the cert
            certMan3Desc.LoadCertificate(sString);

            //b64 should still not be the same
            Assert.AreNotEqual(certMan2Desc.CertificateBase64, certMan3Desc.CertificateBase64);

            //Both thumbprints should now be the same
            Assert.AreEqual(certMan2Desc.Thumbprint, certMan3Desc.Thumbprint);
        }
    }
}
