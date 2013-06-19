using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using M3Space.GroundControl.Core.DataModel;

namespace M3Space.GroundControl.Core.Tests.DataModel
{
    [TestClass]
    public class FloatParameterTest
    {
        static float fTest = 123.45f;
        static string sTest = "123.45";

        IParameter mParameter;

        [TestInitialize]
        public void SetUp()
        {
            mParameter = new FloatParameter("floattest", Unit.None, 2);
        }

        [TestMethod]
        public void TestParse()
        {
            Assert.IsNotNull(mParameter.ParseValue(sTest));
        }

        [TestMethod]
        public void TestAddInt()
        {
            Assert.IsTrue(mParameter.AddValue(fTest));
        }

        [TestMethod]
        public void TestAddString()
        {
            Assert.IsTrue(mParameter.AddValue(sTest));
        }
    }
}
