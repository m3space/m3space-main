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

        private byte[] dataBuffer;
        private DateTime currentTs;
        private bool imageComplete;
        private int lastOffset;

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
        public byte[] ImageData { get { return ((dataBuffer != null) && (dataBuffer.Length > 0)) ? dataBuffer : null; } }

        /// <summary>
        /// Gets if the current image is complete.
        /// </summary>
        public bool IsImageComplete { get { return imageComplete; } }

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
        /// <param name="length">the image size in bytes</param>
        /// <returns>true if ok, false if other image is already initialized</returns>
        public bool BeginImage(DateTime utcTs, int length)
        {
            if (dataBuffer == null)
            {
                imageComplete = false;
                lastOffset = -1;
                currentTs = utcTs.AddYears(YEAR_OFFSET);
                dataBuffer = new byte[length];
                Array.Clear(dataBuffer, 0, dataBuffer.Length);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Inserts an image chunk.
        /// </summary>
        /// <param name="imgOffset">the offset within the image</param>
        /// <param name="data">the chunk data</param>
        /// <param name="srcIdx">the start index of the chunk data</param>
        /// <param name="length">the chunk length</param>
        /// <returns>ok if data can be inserted, false if image is not initialized</returns>
        public bool InsertChunk(int imgOffset, byte[] data, int srcIdx, int length)
        {
            if (dataBuffer != null)
            {
                if (imgOffset <= lastOffset)
                {
                    // missed BeginImage
                    EndImage();
                    return false;
                }

                try
                {
                    Array.Copy(data, srcIdx, dataBuffer, imgOffset, length);
                    lastOffset = imgOffset;
                    if (imgOffset + length >= dataBuffer.Length)
                    {
                        imageComplete = true;
                    }
                    return true;
                }
                catch (IndexOutOfRangeException)
                {
                    // chunk does not fit in buffer, missed end of image and begin of new image
                    EndImage();
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Finishes image decoding and cleans up.
        /// </summary>
        public void EndImage()
        {
            dataBuffer = null;
            imageComplete = false;
        }

    }
}
