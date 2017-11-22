using CryptLink.Services;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLinkTests.Integration {

    [SetUpFixture]
    class ServiceStackUnknownTest {
        public static ServiceStackHost AppHost;

        [SetUp]
        public void Setup() {
            AppHost = new BasicAppHost(typeof(ServiceHost).Assembly) {
                ConfigureContainer = container =>
                {
                    //var l = new List<string>();
                    //l.Add(ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
                    //container.Register<IRedisClientsManager>(c => new RedisManagerPool(l, new RedisPoolConfig() { MaxPoolSize = 40 }));
                }
            }
            .Init();
        }

        [TearDown]
        public void TearDown() {
            AppHost.Dispose();
        }
    }
}
