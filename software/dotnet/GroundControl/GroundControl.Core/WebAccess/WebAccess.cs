﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroundControl.Core;
using System.Threading;
using System.IO;
using System.Net;

namespace GroundControl.Core.WebAccess
{
    public class WebAccess
    {
        private static readonly string key = "gn8lgz7xg73d22e0xif";
        private static readonly Encoding encoding = Encoding.UTF8;
        private static readonly string userAgent = "GroundControl";

        private string url;

        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                if (!url.EndsWith("/"))
                    url += '/';
            }
        }

        /// <summary>
        /// This event is fired in case of an error.
        /// </summary>
        public event ErrorHandler Error;

        public void UploadTelemetry(TelemetryData telemetry)
        {
            new Thread(delegate() { UploadTelemetryExec(telemetry); }).Start();
        }

        public void UploadLiveImage(DateTime utcTs, byte[] imgData, bool ok)
        {
            new Thread(delegate() { UploadLiveImageExec(utcTs, imgData); }).Start();
        }

        private void UploadTelemetryExec(TelemetryData telemetry)
        {
            try
            {
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("key", key);
                postParameters.Add("utctimestamp", telemetry.UtcTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                postParameters.Add("latitude", telemetry.Latitude);
                postParameters.Add("longitude", telemetry.Longitude);
                postParameters.Add("galtitude", telemetry.GpsAltitude);
                postParameters.Add("paltitude", telemetry.PressureAltitude);
                postParameters.Add("heading", telemetry.Heading);
                postParameters.Add("hspeed", telemetry.HorizontalSpeed);
                postParameters.Add("vspeed", telemetry.VerticalSpeed);
                postParameters.Add("satellites", telemetry.Satellites);
                postParameters.Add("inttemperature", telemetry.IntTemperature);
                postParameters.Add("temperature1", telemetry.Temperature1);
                postParameters.Add("temperature2", telemetry.Temperature2);
                postParameters.Add("pressure", telemetry.Pressure);
                postParameters.Add("vin", telemetry.Vin);

                HttpWebResponse webResponse = MultipartFormDataPost(url + "ws/uploadtelemetry.php", userAgent, postParameters);
                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    OnError("Failed to upload telemetry to web server.");
                }
            }
            catch (Exception e)
            {
                OnError("Web access exception: " + e.Message);
            }
        }

        private void UploadLiveImageExec(DateTime utcTs, byte[] imgData)
        {
            try
            {
                string filename = utcTs.ToString("yyyyMMdd_HHmmss") + ".jpg";
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("key", key);
                postParameters.Add("utctimestamp", utcTs.ToString("yyyy-MM-dd HH:mm:ss"));
                postParameters.Add("uploadedfile", new FileParameter(imgData, filename, "image/jpeg"));

                HttpWebResponse webResponse = MultipartFormDataPost(url + "ws/uploadliveimage.php", userAgent, postParameters);
                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    OnError("Failed to upload live image to web server.");
                }
            }
            catch (Exception e)
            {
                OnError("Web access exception: " + e.Message);
            }
        }

        private void OnError(string message)
        {
            if (Error != null)
                Error(message);
        }

        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }


        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            // request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}