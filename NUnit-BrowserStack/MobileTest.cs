using NUnit.Framework;

namespace BrowserStack
{
    [TestFixture("parallel", "ios")]
    [TestFixture("parallel", "android")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MobileTest : SingleTest
    {
        public MobileTest(string profile, string environment) : base(profile, environment) { }
    }
}
