using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GroundControl.Core
{
    /// <summary>
    /// Builds JPEG images from raw data.
    /// </summary>
    public class ImageDecoder
    {
        private const int YEAR_OFFSET = 1600;  // because different DateTime origin in the microframework (year 1601 instead of 0001)
        // http://netmf.codeplex.com/workitem/1003

        private MemoryStream dataBuffer;
        private DateTime currentTs;

        /// <summary>
        /// Checks if there is no image being decoded.
        /// </summary>
        public bool IsIdle { get { return (dataBuffer == null); } }

        /// <summary>
        /// Gets the current UTC timestamp
        /// </summary>
        public DateTime CurrentUtcTimestamp { get { return currentTs; } }

        /// <summary>
        /// Gets the current image data.
        /// Returns null if image is not initialized or contains no data.
        /// </summary>
        public byte[] ImageData { get { return ((dataBuffer != null) && (dataBuffer.Length > 0)) ? dataBuffer.ToArray() : null; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageDecoder()
        {
            dataBuffer = null;
        }

        /// <summary>
        /// Begins a new image.
        /// </summary>
        /// <param name="utcTs">the image timestamp (UTC)</param>
        /// <returns>true if ok, false if other image is already initialized</returns>
        public bool BeginImage(DateTime utcTs)
        {
            if (dataBuffer == null)
            {
                currentTs = utcTs.AddYears(YEAR_OFFSET);
                dataBuffer = new MemoryStream();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Appends image data.
        /// </summary>
        /// <param name="data">the image data</param>
        /// <returns>ok if data can be appended, false if image is not initialized</returns>
        public bool AppendData(byte[] data)
        {
            if (dataBuffer != null)
            {
                dataBuffer.Write(data, 0, data.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finishes image decoding and cleans up.
        /// </summary>
        public void EndImage()
        {
            if (dataBuffer != null)
                dataBuffer.Close();
            dataBuffer = null;
        }

    }
}
