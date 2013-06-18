using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3Space.GroundControl.Core.DataModel
{
    /// <summary>
    /// An interface for a data parameter
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Adds a value.
        /// The value type must correspond with the parameter type.
        /// </summary>
        /// <param name="value">the value to add</param>
        void AddValue(object value);

        /// <summary>
        /// Adds a value from a string.
        /// The value must be compatible with the parameter type.
        /// </summary>
        /// <param name="value">the value to add</param>
        void AddValue(string str);

        /// <summary>
        /// Gets the value at index i.
        /// </summary>
        /// <param name="i">the index</param>
        /// <returns>a value of the parameter type</returns>
        object GetValue(int i);

        /// <summary>
        /// Gets the value at index i as string.
        /// </summary>
        /// <param name="i">the index</param>
        /// <returns>a string</returns>
        string GetStringValue(int i);

        /// <summary>
        /// Gets the value at index i as string.
        /// Optionally with unit.
        /// </summary>
        /// <param name="i">the index</param>
        /// <param name="withUnit">if true the unit is displayed</param>
        /// <returns>a string</returns>
        string GetStringValue(int i, bool withUnit);

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parameter unit.
        /// </summary>
        string Unit { get; }

        /// <summary>
        /// Gets the number of values.
        /// </summary>
        int ValueCount { get; }
    }
}
