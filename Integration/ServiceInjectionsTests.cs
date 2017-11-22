using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace CryptLinkTests {

    [TestFixture]
    public class ServiceInjectionTests {

        [Route("/version")]
        public class VersionRequest {
        }

        public class ServiceConfigTest {
            public int Version { get; set; }
        }

        public class VersionResponse {
            public string Result { get; set; }
        }

        public class TestConfigService : Service {
            public ServiceConfigTest SConfig { get; set; }

            public object Any(VersionRequest request) {
                return new VersionResponse { Result = SConfig.Version.ToSafeJson()};
            }
        }

        public class AppHost : AppSelfHostBase {
            public AppHost() : base("HttpListener Self-Host", typeof(TestConfigService).Assembly) { }

            public override void Configure(Funq.Container container) { }
        }

        [Test]
        public void ServiceStackFunqRegister() {
            var listeningOn = "http://localhost:13337/";
            var appHost = new AppHost();              

            appHost.Container.Register<ServiceConfigTest>(new ServiceConfigTest() { Version = 1337 });
            appHost.Init().Start(listeningOn);

            var wClient = new System.Net.WebClient();
            var response = wClient.DownloadString(listeningOn + "/version?format=json");

            Assert.IsTrue(response.Contains("1337"));

            appHost.Stop();

        }
    }



}
