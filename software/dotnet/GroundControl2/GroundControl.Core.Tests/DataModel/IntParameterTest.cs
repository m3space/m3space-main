using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using M3Space.GroundControl.Core.DataModel;

namespace M3Space.GroundControl.Core.Tests.DataModel
{
    [TestClass]
    public class IntParameterTest
    {
        static int iTest = 123;
        static string sTest = "123";

        IParameter mParameter;

        [TestInitialize]
        public void SetUp()
        {
            mParameter = new IntParameter("inttest", Unit.None);
        }

        [TestMethod]
        public void TestParse()
        {
            Assert.IsNotNull(mParameter.ParseValue(sTest));
        }

        [TestMethod]
        public void TestAddInt()
        {
            Assert.IsTrue(mParameter.AddValue(iTest));
        }

        [TestMethod]
        public void TestAddString()
        {
            Assert.IsTrue(mParameter.AddValue(sTest));
        }
    }
}
