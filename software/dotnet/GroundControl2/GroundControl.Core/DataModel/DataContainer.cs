using System;
using System.Collections.Generic;
using System.Text;

namespace M3Space.GroundControl.Core.DataModel
{
    /// <summary>
    /// A flexible container for telemetry data.
    /// </summary>
    public class DataContainer
    {
        private List<IParameter> mParameters;
        private Dictionary<string, IParameter> mParametersByName;

        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        public int ParameterCount
        {
            get { return mParameters.Count; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataContainer()
        {
            mParameters = new List<IParameter>();
            mParametersByName = new Dictionary<string, IParameter>();
        }

        /// <summary>
        /// Adds a new parameter.
        /// </summary>
        /// <param name="parameter">the parameter</param>
        public void AddParameter(IParameter parameter)
        {
            mParameters.Add(parameter);
            mParametersByName.Add(parameter.Name, parameter);
        }

        /// <summary>
        /// Gets a parameter.
        /// </summary>
        /// <param name="i">the parameter index</param>
        /// <returns>a parameter</returns>
        public IParameter GetParameter(int i)
        {
            return mParameters[i];
        }

        /// <summary>
        /// Gets a parameter.
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <returns>a parameter</returns>
        public IParameter GetParameter(string name)
        {
            return mParametersByName[name];
        }

        /// <summary>
        /// Adds a row of values.
        /// The number of columns should match the number of parameters.
        /// </summary>
        /// <param name="values">the values</param>
        private void AddValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                mParameters[i].AddValue(values[i]);
            }
        }

        /// <summary>
        /// Adds a row of values.
        /// The number of columns should match the number of parameters.
        /// </summary>
        /// <param name="values">the values</param>
        /// <returns>true if row is added, false if invalid data or number of values less than parameters</returns>
        public bool AddValues(string[] values)
        {
            if (values.Length < ParameterCount)
                return false;

            object[] parsed = new object[ParameterCount];
            for (int i = 0; i < ParameterCount; i++)
            {
                if ((parsed[i] = mParameters[i].ParseValue(values[i])) == null)
                    return false;
            }

            AddValues(parsed);
            return true;
        }
    }
}
