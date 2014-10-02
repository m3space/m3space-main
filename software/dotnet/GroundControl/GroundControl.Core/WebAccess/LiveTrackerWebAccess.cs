using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroundControl.Core;
using System.Threading;
using System.IO;
using System.Net;

namespace GroundControl.Core.WebAccess
{
    public class LiveTrackerWebAccess
    {
        private LiveTrackerWebClient webClient;
        private Queue<WebTask> taskQueue;
        private Semaphore semaphore;
        private bool doRun;

        public bool IsRunning { get { return doRun; } }

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
            taskQueue = new Queue<WebTask>();
            semaphore = new Semaphore(0, 10000);
            doRun = false;
        }

        public void Start()
        {
            if (!doRun)
            {
                doRun = true;
                try
                {
                    new Thread(new ThreadStart(Run)).Start();
                }
                catch (Exception)
                {
                    doRun = false;
                }
            }
        }

        public void Stop()
        {
            doRun = false;
            taskQueue.Clear();
        }

        private void Run()
        {
            taskQueue.Clear();
            while (doRun)
            {
                semaphore.WaitOne();
                if (taskQueue.Count > 0)
                {
                    WebTask task = taskQueue.Peek();
                    try
                    {
                        task.Execute(webClient);
                        taskQueue.Dequeue();
                    }
                    catch (LiveTrackerException e1)
                    {
                        OnError(e1.Message);
                        taskQueue.Dequeue();
                    }
                    catch (Exception e2)
                    {
                        OnError(e2.Message);
                        semaphore.Release();
                    }
                }
                else
                {
                    doRun = false;
                }
            }
            taskQueue.Clear();
        }

        public void UploadTelemetry(TelemetryData telemetry)
        {
            if (doRun)
            {
                taskQueue.Enqueue(new UploadTelemetryTask(telemetry));
                semaphore.Release();
            }
            else
            {
                OnError("Web uploader not running.");
            }
        }

        public void PostBlog(DateTime utcTs, string message)
        {
            if (doRun)
            {
                taskQueue.Enqueue(new PostBlogTask(utcTs, message));
                semaphore.Release();
            }
            else
            {
                OnError("Web uploader not running.");
            }
        }

        public void UploadLiveImage(DateTime utcTs, byte[] imgData, bool ok)
        {
            if (doRun)
            {
                taskQueue.Enqueue(new UploadLiveImageTask(utcTs, imgData));
                semaphore.Release();
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
