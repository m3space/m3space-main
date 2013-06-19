using System;
using System.Collections.Generic;
using System.Text;

namespace M3Space.GroundControl.Core.DataModel
{
    abstract public class ParameterImpl : IParameter
    {
        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the parameter unit.
        /// </summary>
        public string Unit { get; protected set; }

        /// <summary>
        /// Gets the number of values.
        /// </summary>
        public int ValueCount { get; protected set; }

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="unit">the parameter unit</param>
        public ParameterImpl(string name, string unit)
        {
            Name = name;
            Unit = unit;
            ValueCount = 0;
        }

        abstract public bool AddValue(object value);

        abstract public bool AddValue(string str);

        abstract public object GetValue(int i);

        abstract public string GetStringValue(int i);

        abstract public string GetStringValue(int i, bool withUnit);

        abstract public object ParseValue(string str);
    }
}
