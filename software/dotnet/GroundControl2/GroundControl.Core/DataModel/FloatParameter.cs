using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3Space.GroundControl.Core.DataModel
{
    /// <summary>
    /// A floating-point parameter.
    /// </summary>
    public class FloatParameter : ParameterImpl
    {
        private List<float> mValues;
        private string mDisplayFormat;
        private string mDisplayFormatWithUnit;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="unit">the parameter unit</param>
        /// <param name="decimals">the number of digits behind the decimal point</param>
        public FloatParameter(string name, string unit, int decimals)
            : base(name, unit)
        {
            mValues = new List<float>();
            StringBuilder sb = new StringBuilder();
            sb.Append("{0:#.");
            for (int i = 0; i < decimals; i++)
            {
                sb.Append('#');
            }
            sb.Append('}');
            mDisplayFormat = sb.ToString();
            sb.Append(" {1}");
            mDisplayFormatWithUnit = sb.ToString();
        }

        override public bool AddValue(object value)
        {
            try
            {
                mValues.Add((float)value);
                ValueCount++;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        override public bool AddValue(string str)
        {
            object value = ParseValue(str);
            if (value != null)
            {
                return AddValue(value);
            }
            return false;
        }

        override public object GetValue(int i)
        {
            return mValues[i];
        }

        override public string GetStringValue(int i)
        {
            return String.Format(mDisplayFormat, mValues[i]);
        }

        override public string GetStringValue(int i, bool withUnit)
        {
            if (withUnit)
            {
                return String.Format(mDisplayFormatWithUnit, mValues[i], Unit);
            }
            else
            {
                return mValues[i].ToString();
            }
        }

        override public object ParseValue(string str)
        {
            float value;
            if (Single.TryParse(str, out value))
            {
                return value;
            }
            return null;
        }
    }
}
