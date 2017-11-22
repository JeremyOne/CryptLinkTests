using CryptLink;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLinkTests {
    [TestFixture]
    public class ConfigTests {
        [Test]
        public void ConfigCreateFile() {
            var tempConfig = CryptLink.Utility.GetTempFilePath(".json");
            var c = CryptLink.ServiceConfig.Load(tempConfig, true);
            
            Assert.NotNull(c);
            Assert.True(System.IO.File.Exists(tempConfig), "File was created");

            CryptLink.Utility.WaitDeleteFile(tempConfig, new TimeSpan(0, 0, 30));
            Assert.True(!System.IO.File.Exists(tempConfig), "File was successfully deleted");
        }

        [Test]
        public void ConfigFactoryTestDefaults() {
            var tempConfig = CryptLink.Utility.GetTempFilePath(".json");
            //var c = CryptLink.Config.Load(tempConfig, true);

            var c = ServiceConfigFactory.TestDefaults();

            Assert.NotNull(c);
            Assert.NotNull(c.ServerPeerInfo);
            Assert.NotNull(c.Swarm);

            CryptLink.Utility.WaitDeleteFile(tempConfig, new TimeSpan(0, 0, 30));
            Assert.True(!System.IO.File.Exists(tempConfig), "File was successfully deleted");
        }

        [Test]
        public void ConfigSaveLoad() {
            var tempConfig = CryptLink.Utility.GetTempFilePath(".json");

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

            var c2 = ServiceConfig.Load(tempConfig, false);

            Assert.NotNull(c2);
            Assert.NotNull(c2.ServerPeerInfo);
            Assert.NotNull(c2.Server);
            Assert.NotNull(c2.Swarm);

            Assert.AreEqual(c.Swarm.PublicAddress, c2.Swarm.PublicAddress);
            Assert.AreEqual(c.ServerPeerInfo.PublicKey, c2.ServerPeerInfo.PublicKey);
            Assert.AreEqual(c.Server.PrivateKey.Thumbprint, c2.Server.PrivateKey.Thumbprint);
            Assert.AreEqual(c.Swarm.PrivateKey.Thumbprint, c2.Swarm.PrivateKey.Thumbprint);

            Utility.WaitDeleteFile(tempConfig, new TimeSpan(0, 0, 30));
            Assert.True(!System.IO.File.Exists(tempConfig), "File was successfully deleted");
        }


        [Test]
        public void ConfigLoadError() {
            var tempConfig = Utility.GetTempFilePath(".json");

            var ex = Assert.Throws<System.IO.FileNotFoundException>(() => CryptLink.ServiceConfig.Load(tempConfig, false));
            Assert.NotNull(ex);

            Assert.False(System.IO.File.Exists(tempConfig), "File was not created");
        }
    }
}
