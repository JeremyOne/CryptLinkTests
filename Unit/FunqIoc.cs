using NUnit.Framework;
using System;
using Funq;

namespace CryptLinkTests {
	[TestFixture()]
	public class FunqIoc {
		
		public interface IFoo { }
		public interface IFoo2 { }
		public interface IBar { }
		public class Foo : IFoo { }
		public class Bar : IBar { }

		public class FunqTest
		{
			public IFoo Foo { get; set; }
			public IFoo2 Foo2 { get; set; }
			public IBar Bar { get; set; }
		}

		[Test]
		public void FunqCanAutoWire() {
			var container = new Container();
			container.Register<IFoo>(c => new Foo());
			container.Register<IBar>(c => new Bar());
			container.RegisterAutoWired<FunqTest>();

            var test = container.Resolve<FunqTest>();
			Assert.IsNotNull(test.Foo);
			Assert.IsNotNull(test.Bar);
			Assert.IsNull(test.Foo2 as Foo);

		}


	}
}

