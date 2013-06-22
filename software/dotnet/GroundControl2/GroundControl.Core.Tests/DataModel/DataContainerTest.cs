using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using M3Space.GroundControl.Core.DataModel;

namespace M3Space.GroundControl.Core.Tests.DataModel
{
    [TestClass]
    public class DataContainerTest
    {
        static DateTime dTest = DateTime.Now;
        static int iTest = 123;
        static float fTest = 123.45f;

        DataContainer mDataContainer;

        [TestInitialize]
        public void SetUp()
        {
            mDataContainer = new DataContainer();
            mDataContainer.AddParameter(new DateTimeParameter("datetime", "dd.MM.yyyy HH:mm:ss.fff", "dd.MM.yyyy HH:mm:ss.fff"));
            mDataContainer.AddParameter(new IntParameter("int", Unit.None));
            mDataContainer.AddParameter(new FloatParameter("float", Unit.None, 2));
        }

        [TestMethod]
        public void TestAddValues()
        {
            object[] values = new object[] { dTest, iTest, fTest };
            mDataContainer.AddValues(values);
        }
    }
}
