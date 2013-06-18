using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3Space.GroundControl.Core.DataModel
{
    public class IntParameter : ParameterImpl
    {
        private List<int> mValues;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="unit">the parameter unit</param>
        public IntParameter(string name, string unit)
            : base(name, unit)
        {
            mValues = new List<int>();
        }

        override public void AddValue(object value)
        {
            mValues.Add((int)value);
            ValueCount++;
        }

        override public void AddValue(string str)
        {
            int value;
            if (Int32.TryParse(str, out value))
            {
                mValues.Add(value);
                ValueCount++;
            }
        }

        override public object GetValue(int i)
        {
            return mValues[i];
        }

        override public string GetStringValue(int i)
        {
            return mValues[i].ToString();
        }

        override public string GetStringValue(int i, bool withUnit)
        {
            if (withUnit)
            {
                return String.Format("{0} {1}", mValues[i], Unit);
            }
            else
            {
                return mValues[i].ToString();
            }
        }
    }
}
