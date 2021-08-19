using NUnit.Framework;

namespace BrowserStack
{
    [TestFixture("parallel", "chrome")]
    [TestFixture("parallel", "firefox")]
    [TestFixture("parallel", "safari")]
    [TestFixture("parallel", "edge")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ParallelTest : SingleTest
    {
        public ParallelTest(string profile, string environment) : base(profile, environment) { }
    }
}
