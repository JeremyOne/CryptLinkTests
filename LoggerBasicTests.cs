using NUnit.Framework;
using System;
using NLog;

namespace CryptLinkTests {
	[TestFixture()]
	public class LoggerBasicTests {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		[Test()]
		public void LoggerBasicTest() {
			logger.Trace("Trace: The chatter of people on the street");
			logger.Debug("Debug: Where are you going and why?");
			logger.Info("Info: What bus station you're at.");
			logger.Warn("Warn: You're playing on the phone and not looking up for your bus");
			logger.Error("Error: You get on the wrong bus.");
			logger.Fatal("Fatal: You are run over by the bus.");

			Assert.Pass();
		}
	}
}

