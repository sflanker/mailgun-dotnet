using Mailgun.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Mailgun.Tests.Extensions
{
    [TestClass]
    public class BooleanExtensionSpec
    {
        [TestMethod]
        public void TestBooleanNo()
        {
            const bool yesNo = false;
            yesNo.AsYesNo().ShouldBe("no");
        }

        [TestMethod]
        public void TestBooleanYes()
        {
            const bool yesNo = true;
            yesNo.AsYesNo().ShouldBe("yes");
        }
    }
}