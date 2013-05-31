using System;

namespace GroundControl.Core
{
    /// <summary>
    /// An image complete delegate
    /// </summary>
    /// <param name="utcTimestamp">the image timestamp (UTC)</param>
    /// <param name="data">the image data</param>
    /// <param name="ok">true if ok, false if with errors</param>
    public delegate void ImageCompleteHandler(DateTime utcTimestamp, byte[] data, bool ok);
}