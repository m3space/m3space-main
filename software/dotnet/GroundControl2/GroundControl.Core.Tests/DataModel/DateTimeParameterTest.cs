using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using M3Space.GroundControl.Core.DataModel;

namespace M3Space.GroundControl.Core.Tests.DataModel
{
    [TestClass]
    public class DateTimeParameterTest
    {
        static DateTime dTest = DateTime.Now;
        static string sTest = "23.05.2013 13:37:00.000";

        IParameter mParameter;

        [TestInitialize]
        public void SetUp()
        {
            mParameter = new DateTimeParameter("datetimetest", "dd.MM.yyyy HH:mm:ss.fff", "dd.MM.yyyy HH:mm:ss.fff");
        }

        [TestMethod]
        public void TestParse()
        {
            Assert.IsNotNull(mParameter.ParseValue(sTest));
        }

        [TestMethod]
        public void TestAddInt()
        {
            Assert.IsTrue(mParameter.AddValue(dTest));
        }

        [TestMethod]
        public void TestAddString()
        {
            Assert.IsTrue(mParameter.AddValue(sTest));
        }
    }
}
