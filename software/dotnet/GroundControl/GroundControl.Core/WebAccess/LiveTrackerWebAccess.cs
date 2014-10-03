using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroundControl.Core;
using System.Threading;
using System.IO;
using System.Net;
using System.Collections.Concurrent;

namespace GroundControl.Core.WebAccess
{
    public class LiveTrackerWebAccess
    {
        private LiveTrackerWebClient webClient;
        private BlockingCollection<WebTask> taskQueue;
        private bool doRun;
        private bool running;

        public bool IsRunning { get { return running; } }

        public string Url
        {
            get { return webClient.Url; }
            set { webClient.Url = value; }
        }

        /// <summary>
        /// This event is fired in case of an error.
        /// </summary>
        public event ErrorHandler Error;

        public LiveTrackerWebAccess()
        {
            webClient = new LiveTrackerWebClient();
            running = false;
            doRun = false;
        }

        public void Start()
        {
            if (!running)
            {
                doRun = true;
                running = true;
                try
                {
                    new Thread(new ThreadStart(Run)).Start();
                }
                catch (Exception)
                {
                    doRun = false;
                    running = false;
                }
            }
        }

        public void Stop()
        {
            if (taskQueue != null)
            {
                taskQueue.CompleteAdding();
            }
            doRun = false;
        }

        private void Run()
        {
            taskQueue = new BlockingCollection<WebTask>();
            while (doRun)
            {
                WebTask task = null;
                try
                {
                    task = taskQueue.Take();                    
                }
                catch (InvalidOperationException)
                {
                    // completed for adding
                    doRun = false;
                    break;
                }
                bool repeat = false;
                do
                {
                    try
                    {
                        task.Execute(webClient);
                        repeat = false;
                    }
                    catch (LiveTrackerException e1)
                    {
                        // service error, abort
                        OnError(e1.Message);
                    }
                    catch (Exception e2)
                    {
                        // web exception like timeout
                        OnError(e2.Message);
                        repeat = true;
                    }
                }
                while (repeat);
   
            }
            running = false;
            taskQueue = null;
        }

        public void UploadTelemetry(TelemetryData telemetry)
        {
            if (running && (taskQueue != null))
            {
                taskQueue.Add(new UploadTelemetryTask(telemetry));
            }
            else
            {
                OnError("Web uploader not running.");
            }
        }

        public void PostBlog(DateTime utcTs, string message)
        {
            if (running && (taskQueue != null))
            {
                taskQueue.Add(new PostBlogTask(utcTs, message));
            }
            else
            {
                OnError("Web uploader not running.");
            }
        }

        public void UploadLiveImage(DateTime utcTs, byte[] imgData, bool ok)
        {
            if (running && (taskQueue != null))
            {
                taskQueue.Add(new UploadLiveImageTask(utcTs, imgData));
            }
            else
            {
                OnError("Web uploader not running.");
            }
        }

        private void OnError(string message)
        {
            if (Error != null)
                Error(message);
        }

        interface WebTask
        {
            void Execute(LiveTrackerWebClient webClient);
        }

        class UploadTelemetryTask : WebTask
        {
            private TelemetryData data;

            public UploadTelemetryTask(TelemetryData data)
            {
                this.data = data;
            }

            public void Execute(LiveTrackerWebClient webClient)
            {
                webClient.UploadTelemetry(data);
            }
        }

        class UploadLiveImageTask : WebTask
        {
            private DateTime utcTs;
            private byte[] imgData;

            public UploadLiveImageTask(DateTime utcTs, byte[] imgData)
            {
                this.utcTs = utcTs;
                this.imgData = imgData;
            }

            public void Execute(LiveTrackerWebClient webClient)
            {
                webClient.UploadLiveImage(utcTs, imgData);
            }
        }

        class PostBlogTask : WebTask
        {
            private DateTime utcTs;
            private string message;

            public PostBlogTask(DateTime utcTs, string message)
            {
                this.utcTs = utcTs;
                this.message = message;
            }

            public void Execute(LiveTrackerWebClient webClient)
            {
                webClient.PostBlog(utcTs, message);
            }
        }
    }
}
