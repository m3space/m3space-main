using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl.Core.WebAccess
{
    public class LiveTrackerException : Exception
    {
        public LiveTrackerException(string message)
            : base(message)
        {
        }
    }
}
