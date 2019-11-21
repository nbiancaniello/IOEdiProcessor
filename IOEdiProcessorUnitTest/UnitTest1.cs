using IOEdiProcessor.Data.Context;
using IOEdiProcessor.Logic;
using Xunit.Sdk;

namespace IOEdiProcessorUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private readonly IOEdiProcessorContext _context;
        private readonly string _shipPlanId = "15233";
        private readonly string _custID = "VWT";
        private readonly string _docType = "DESADV";
        private readonly string _PurposeCode = "00";

        [TestMethod]
        public void TestMethod1()
        {
            EdifactLogic logic = new EdifactLogic(_context);
            Assert.IsNotNull(logic.GenerateEDIFile(_shipPlanId, _custID, _docType, _PurposeCode));
        }
    }
}
