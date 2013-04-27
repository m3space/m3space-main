using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AForge.Video.FFMPEG;

namespace VideoPostProcess
{
    class Video
    {
        private VideoFileReader m_inputVideo = new VideoFileReader();
        private VideoFileWriter m_outputVideo = new VideoFileWriter();
        private Size            m_videosize = new Size();
        private int             m_framerate;
        private long            m_numberOfFrames;
        private string          m_videocodec;
        private List<string>    m_inputVideoList;
        private string          m_outputFilename;

        public Size             Size { get { return m_videosize; } }
        public int              Framerate { get {return m_framerate;} }
        public string           Codec { get{return m_videocodec;} }
        public long             FrameCount { get { return m_numberOfFrames;} }
        public int              VideoCount { get { return m_inputVideoList == null ? 0 : m_inputVideoList.Count; } }
        public long             CurrentVideoFrameCount { get { return m_inputVideo.FrameCount; } }
        public string           OutputFilename { get { return m_outputFilename; } set { m_outputFilename = value; } }
        public List<string>     InputVideoList { get { return m_inputVideoList; } }

        internal void Init(List<string> videoList)
        {
            if (videoList.Count > 0)
            {
                m_inputVideoList = videoList;
                m_numberOfFrames = 0;
                foreach (var videofilename in m_inputVideoList)
                {
                    m_inputVideo.Open(videofilename);
                    m_videosize = new Size(m_inputVideo.Width, m_inputVideo.Height);
                    m_framerate = m_inputVideo.FrameRate;
                    m_videocodec = m_inputVideo.CodecName;
                    m_numberOfFrames += m_inputVideo.FrameCount;
                    m_inputVideo.Close();
                }
                m_outputFilename = Path.GetDirectoryName(m_inputVideoList[0]) + @"\Output.mp4";
            }
        }

        internal void WriteVideoFrame(Bitmap bitmap)
        {
            if (!m_outputVideo.IsOpen)
            {
                m_outputVideo.Open(m_outputFilename, m_videosize.Width, m_videosize.Height, Framerate, VideoCodec.MPEG4, 15000000);
            }
            m_outputVideo.WriteVideoFrame(bitmap);
        }

        internal void OpenInputVideo(string videofilename)
        {
            m_inputVideo.Open(videofilename);
        }

        internal Bitmap ReadVideoFrame()
        {
            return m_inputVideo.ReadVideoFrame();
        }

        internal void CloseInputVideo()
        {
            if (m_inputVideo != null && m_inputVideo.IsOpen) 
                m_inputVideo.Close();
        }
        
        internal void CloseVideos()
        {
            CloseInputVideo();
            if (m_outputVideo != null && m_outputVideo.IsOpen) 
                m_outputVideo.Close();
        }
    }
}
