using System;
using ServiceStack;
using System.Runtime.Remoting;
using NUnit.Framework;

namespace CryptLinkTests.Integration {
    public class AppHost : AppSelfHostBase {
        public AppHost() : base("My ServiceStack Service", typeof(AppHost).Assembly) {
        }

        public override void Configure(Funq.Container container) {
            container.Register<TestResult>(c => new TestResult());
        }
    }

    public class TestResult {
        public string Name { get { return "Hello"; } }
    }

    public class IsolatedAppHost : MarshalByRefObject {
        readonly AppHost Host;

        public IsolatedAppHost() {
            // Start your HttpTestableAppHost here
            Host = new AppHost();
            Host.Init();
            Host.Start("http://*:8090/");
            Console.WriteLine("ServiceStack is running in AppDomain '{0}'", AppDomain.CurrentDomain.FriendlyName);
        }

        public void RunTest(Action<AppHost> test) {
            test.Invoke(Host);
        }

        public void Teardown() {
            if (Host != null) {
                Console.WriteLine("Shutting down ServiceStack host");
                if (Host.HasStarted)
                    Host.Stop();
                Host.Dispose();
            }
        }
    }

    [TestFixture]
    public class ServiceStackIsolationTests {
        AppDomain ServiceStackAppDomain;
        IsolatedAppHost IsolatedAppHost;

        [TestFixtureSetUp]
        public void TestFixtureSetup() {
            // Get the assembly of our host
            var assemblyName = typeof(IsolatedAppHost).Assembly.GetName();
            ServiceStackAppDomain = AppDomain.CreateDomain("ServiceStackAppDomain");
            ServiceStackAppDomain.Load(assemblyName);
            var handle = ServiceStackAppDomain.CreateInstance(assemblyName.FullName, typeof(IsolatedAppHost).FullName);
            
            IsolatedAppHost = (IsolatedAppHost)handle.Unwrap();
        }

        [Test]
        public void ServiceStackIsolationTest() {
            IsolatedAppHost.RunTest(appHost => {
                var t = appHost.TryResolve<TestResult>();
                Assert.That(t.Name, Is.EqualTo("Hello"));
            });
        }

        [Test]
        public void ServiceStackIsolationTestFail() {
            IsolatedAppHost.RunTest(appHost => {
                var t = appHost.TryResolve<TestResult>();
                Assert.That(t.Name, Is.EqualTo("World"));
            });
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown() {
            // Tell ServiceStack to stop the host
            IsolatedAppHost.Teardown();

            // Shutdown the ServiceStack application
            AppDomain.Unload(ServiceStackAppDomain);
            ServiceStackAppDomain = null;
        }
    }
}