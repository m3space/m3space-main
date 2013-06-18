using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GroundControl.Core;

namespace VideoPostProcess
{
    public class UpdateProgressEventArgs : EventArgs
    {
        public FrameInfo Frame { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public TimeSpan TimeRemaining { get; set; }
    }

    public struct FrameInfo
    {
        public Bitmap Image;
        public int FrameId;
        public int VideoId;
    }

    class VideoProcessor
    {
        private Video           m_video = new Video();
        private DataCache       m_telemetryData = new DataCache();
        private Thread          m_workerThread;
        private bool            m_threadAbort;
        private string          m_missionName = "M3 Space Mission 002";
        private DateTime        m_videoStart = new DateTime(2012, 8, 13, 10, 5, 20);
        private DateTime        m_balloonStart = new DateTime(2012, 8, 13, 10, 17, 0);
        private MapOverlay      m_mapOverlay = new MapOverlay();
        private TelemetryOverlay m_dataOverlay;
        private int             m_startTime;
        private Task            m_task1;
        private Task            m_task2;
        private Task            m_task3;
        private BlockingCollection<FrameInfo> m_inputFrameCollection;
        private BlockingCollection<FrameInfo> m_outputFrameCollection;
        private BlockingCollection<FrameInfo> m_showFrameCollection;


        public delegate void UpdateProgressEventHandler(object sender, UpdateProgressEventArgs e);
        public event UpdateProgressEventHandler UpdateProgress;

        #region Properties
        [Browsable(false)]
        public bool Ready
        {
            get { return m_telemetryData.Size > 0 && m_video.VideoCount > 0; }
        }

        [Browsable(false)]
        public Control MapOverlay 
        { 
            get { return m_mapOverlay; } 
        }

        [CategoryAttribute("Mission Properties")]
        public string Name
        {
            get { return m_missionName; }
            set { m_missionName = value; }
        }

        [CategoryAttribute("Mission Properties")] 
        public string VideoStart
        {
            get { return m_videoStart == null ? "" : m_videoStart.ToString(); }
            set { m_videoStart = DateTime.Parse(value); }
        }

        [CategoryAttribute("Mission Properties")] 
        public string BalloonStart
        {
            get { return m_balloonStart == null ? "" : m_balloonStart.ToString(); }
            set { m_balloonStart = DateTime.Parse(value); }
        }

        [CategoryAttribute("Telemetry"), DisplayName("# of entries"), ReadOnlyAttribute(true)] 
        public int NrOfEntries
        {
            get { return m_telemetryData == null ? 0 : m_telemetryData.Size; }
        }

        [CategoryAttribute("Telemetry"), DisplayName("First entry"), ReadOnlyAttribute(true)] 
        public string TelemetryFrom
        {
            get { return m_telemetryData == null || m_telemetryData.Telemetry.Count == 0 ? "" : m_telemetryData.Telemetry.First().UtcTimestamp.ToString(); }
        }

        [CategoryAttribute("Telemetry"), DisplayName("Last entry"), ReadOnlyAttribute(true)] 
        public string TelemetryTo
        {
            get { return m_telemetryData == null || m_telemetryData.Telemetry.Count == 0 ? "" : m_telemetryData.Telemetry.Last().UtcTimestamp.ToString(); }
        }

        [CategoryAttribute("Video")]
        public string OutputFilename
        {
            get { return m_video.OutputFilename; }
            set { m_video.OutputFilename = value; }
        }

        [CategoryAttribute("Video"), ReadOnlyAttribute(true)]
        public Size Size
        {
            get { return m_video.Size; }
        }

        [CategoryAttribute("Video"), ReadOnlyAttribute(true)] 
        public int Framerate
        {
            get { return m_video.Framerate; }
        }

        [CategoryAttribute("Video"), ReadOnlyAttribute(true)]
        public string Codec
        {
            get { return m_video.Codec; }
        }

        [CategoryAttribute("Video"), DisplayName("# of frames"), ReadOnlyAttribute(true)] 
        public long NrOfFrames
        {
            get { return m_video.FrameCount; }
        }

        [CategoryAttribute("Video"), DisplayName("# of videos"), ReadOnlyAttribute(true)]
        public int NrOfVideos
        {
            get { return m_video.VideoCount; }
        }

        [CategoryAttribute("Map")]
        public double Zoom
        {
            get { return m_mapOverlay.Zoom; }
            set { m_mapOverlay.Zoom = value; }
        }
        #endregion


        public VideoProcessor()
        {
        }

        private void Task1()
        {
            foreach (var item in m_inputFrameCollection.GetConsumingEnumerable())
            {
                // calculate time and interpolate telemtry data
                DateTime currentTime = m_videoStart.AddSeconds(item.FrameId / (double)Framerate);
                TimeSpan missionTime = currentTime.Subtract(m_balloonStart);
                TelemetryData data = m_telemetryData.GetInterpolatedData(currentTime);

                // draw frame and overlays
                Bitmap outputFrame = new Bitmap(m_video.Size.Width, m_video.Size.Height);
                Graphics.FromImage(outputFrame).DrawImage(item.Image, new Point(200, 0));
                m_dataOverlay.DrawOverlay(outputFrame, data, missionTime);
                m_mapOverlay.DrawOverlay(outputFrame, data.Latitude, data.Longitude);
                m_outputFrameCollection.Add(new FrameInfo() { FrameId = item.FrameId, Image = outputFrame, VideoId = item.VideoId });
                item.Image.Dispose();
            }
            m_outputFrameCollection.CompleteAdding();
            Console.WriteLine("Task 1 finished");
        }

        private void Task2()
        {
            foreach (var item in m_outputFrameCollection.GetConsumingEnumerable())
            {
                m_video.WriteVideoFrame(item.Image);
                m_showFrameCollection.Add(item);
            }
            m_showFrameCollection.CompleteAdding();
            Console.WriteLine("Task 2 finished");
        }

        private void Task3()
        {
            foreach (var item in m_showFrameCollection.GetConsumingEnumerable())
            {
                int elapsedTicks = Environment.TickCount - m_startTime;
                TimeSpan timeElapsed = TimeSpan.FromMilliseconds(elapsedTicks);
                TimeSpan timeRemaining = TimeSpan.FromMilliseconds(elapsedTicks * ((NrOfFrames - item.FrameId + 1) / (double)(item.FrameId + 1)));
                OnUpdateProgress(new FrameInfo() { FrameId = item.FrameId, Image = item.Image.Clone() as Bitmap, VideoId = item.VideoId }, timeElapsed, timeRemaining);
                item.Image.Dispose();
            }
            Console.WriteLine("Task 3 finished");
        }

        private void ProcessVideo()
        {
            m_dataOverlay = new TelemetryOverlay();
            m_dataOverlay.MissionName = m_missionName;

            m_startTime = Environment.TickCount;
            int currentFrame = 0;
            int currentVideo = 1; 
            m_inputFrameCollection  = new BlockingCollection<FrameInfo>();
            m_outputFrameCollection = new BlockingCollection<FrameInfo>();
            m_showFrameCollection   = new BlockingCollection<FrameInfo>();
            m_task1 = Task.Factory.StartNew(Task1);// start task 1 which draws overlays
            m_task2 = Task.Factory.StartNew(Task2);// start task 2 which writes the frame
            m_task3 = Task.Factory.StartNew(Task3);// start task 3 which updates the gui

            foreach (var videofile in m_video.InputVideoList)
            {
                m_video.OpenInputVideo(videofile);
                for (int i = 0; i < m_video.CurrentVideoFrameCount; i++, currentFrame++)
                {
                    FrameInfo frame = new FrameInfo() { FrameId = currentFrame, Image = m_video.ReadVideoFrame(), VideoId = currentVideo };
                    while (m_inputFrameCollection.Count > 5)
                    {
                        Thread.Sleep(5);
                    }
                    m_inputFrameCollection.Add(frame);
                    if (m_threadAbort)
                    {
                        Finish();
                        return;
                    }
                }
                currentVideo++;
                m_video.CloseInputVideo();
            }            
            Finish();
        }

        private void OnUpdateProgress(FrameInfo frame, TimeSpan timeElapsed, TimeSpan timeRemaining)
        {
            UpdateProgressEventHandler handler = UpdateProgress;
            if (handler != null)
            {
                UpdateProgressEventArgs e = new UpdateProgressEventArgs() 
                {
                    Frame = frame, 
                    TimeElapsed = timeElapsed,
                    TimeRemaining = timeRemaining
                };
                handler(this, e);
            }
        }

        public void Init(string[] filenames)
        {
            List<string> videoList = new List<string>();
            foreach (var item in filenames)
            {
                switch (Path.GetExtension(item))
                {
                    case ".csv":
                        if (!DataLoader.LoadTelemetryData(item, m_telemetryData))
                        {
                            MessageBox.Show("Failed to load telemetry file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case ".MP4":
                        if (Path.GetFileName(item).StartsWith("GOPR"))
                        {
                            string videonumber = Path.GetFileName(item).Substring(4, 4);
                            videoList.Add(item);
                            string[] subvideos = Directory.GetFiles(Path.GetDirectoryName(item), "GO??" + videonumber + ".MP4");
                            Array.Sort(subvideos);
                            foreach (var subvideo in subvideos)
                            {
                                if (!videoList.Contains(subvideo))
                                {
                                    videoList.Add(subvideo);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            m_video.Init(videoList);
        }

        public void Start()
        {
            m_threadAbort = false;
            m_workerThread = new Thread(new ThreadStart(ProcessVideo));
            m_workerThread.Start();
        }

        public void Cancel()
        {
            m_threadAbort = true;
        }

        private void Finish()
        {
            m_inputFrameCollection.CompleteAdding();
            Task.WaitAll(m_task1, m_task2, m_task3);
            m_video.CloseVideos();
            if (m_video.InputVideoList != null)
            {
                OnUpdateProgress(new FrameInfo() { Image = null, FrameId = 0, VideoId = 0 }, TimeSpan.Zero, TimeSpan.Zero);
            }
        }
    }
}
