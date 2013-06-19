using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace M3Space.GroundControl.Core.DataModel
{
    /// <summary>
    /// A date/time parameter.
    /// </summary>
    public class DateTimeParameter : ParameterImpl
    {
        private List<DateTime> mValues;
        private string[] mImportFormats;
        private string mDisplayFormat;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="importFormats">the date/time formats for import</param>
        /// <param name="displayFormat">the date/time display format</param>
        public DateTimeParameter(string name, string[] importFormats, string displayFormat)
            : base(name, DataModel.Unit.None)
        {
            mValues = new List<DateTime>();
            mImportFormats = importFormats;
            mDisplayFormat = "{0:" + displayFormat + '}';
        }

        override public bool AddValue(object value)
        {
            try
            {
                mValues.Add((DateTime)value);
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
            return GetStringValue(i);
        }

        override public object ParseValue(string str)
        {
            DateTime value;
            if (DateTime.TryParseExact(str, mImportFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            {
                return value;
            }
            return null;
        }
    }
}
