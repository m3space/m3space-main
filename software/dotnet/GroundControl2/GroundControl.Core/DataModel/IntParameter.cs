﻿using System;
using System.Collections.Generic;
using System.Text;

namespace M3Space.GroundControl.Core.DataModel
{
    /// <summary>
    /// An integer parameter.
    /// </summary>
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

        override public bool AddValue(object value)
        {
            try
            {
                mValues.Add((int)value);
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

        override public object ParseValue(string str)
        {
            int value;
            if (Int32.TryParse(str, out value))
            {
                return value;
            }
            return null;
        }
    }
}
