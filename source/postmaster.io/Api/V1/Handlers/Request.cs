using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Postmaster.io.Managers;

namespace Postmaster.io.Api.V1.Handlers
{
    /// <summary>
    /// Request; handle web requests.
    /// </summary>
    public static class Request
    {
        /// <summary>
        /// Post.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="body">Body.</param>
        /// <param name="acceptType">Accept type.</param>
        /// <param name="contentType">Content type.</param>
        /// <returns>Response string.</returns>
        public static string Post(string url, string body, string acceptType = "application/json",
            string contentType = "application/json")
        {
            string response = null;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    // enceode credentials
                    string credentials =
                        Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.ApiKey + ":" + Config.Password));

                    // set headers and related properties
                    wc.Headers.Add(HttpRequestHeader.ContentType, contentType);
                    wc.Headers.Add(HttpRequestHeader.Authorization, "Basic " + credentials);
                    wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    wc.Headers.Add(HttpRequestHeader.UserAgent, Config.UserAgent);
                    wc.Encoding = Encoding.UTF8;

                    // perform request
                    response = wc.UploadString(new Uri(url), WebRequestMethods.Http.Post, body);
                }
            }
            catch (WebException e)
            {
                //var resp = (HttpWebResponse) e.Response;
                using (var reader = new StreamReader(e.Response.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                }
                ErrorHandlingManager.ReportError(e.Message, response, "Request.cs", "Post");
            }
            catch (Exception e)
            {
                ErrorHandlingManager.ReportError(e.Message, "Request.cs", "Post");
            }
            return response;
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns>HttpStatusCode or null.</returns>
        public static HttpStatusCode? Delete(string url)
        {
            WebRequest request = WebRequest.Create(url);
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.ApiKey + ":" + Config.Password));

            request.Method = "DELETE";
            request.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            Dictionary<HttpStatusCode?, string> response = ReadHandledResponse(request as HttpWebRequest);
            if (response.Count == 1)
            {
                foreach (KeyValuePair<HttpStatusCode?, string> pair in response)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// GET data with specified URL, data type and headers.
        /// </summary>
        /// <param name="url">Url.</param>
        /// <param name="acceptType">Accept type.</param>
        /// <param name="headers">Headers.</param>
        /// <param name="queryStrings">Query String collection (optional).</param>
        /// <returns>Response string.</returns>
        public static string Get(string url, WebHeaderCollection headers = null, string acceptType = "application/json", NameValueCollection queryStrings = null)
        {
            string response = null;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.ApiKey + ":" + Config.Password));
                    wc.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

                    // query strings
                    if (queryStrings != null)
                    {
                        wc.QueryString = queryStrings;
                    }

                    // no 401 from server?
                    // wc.Credentials = new NetworkCredential(Config.ApiKey, Config.Password);

                    wc.Encoding = Encoding.UTF8;
                    response = wc.DownloadString(new Uri(url));
                }
            }
            catch (WebException e)
            {
                //var resp = (HttpWebResponse) e.Response;
                //string responseBody;
                using (var reader = new StreamReader(e.Response.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                }
                ErrorHandlingManager.ReportError(e.Message, response, "Request.cs", "Post");
            }
            catch (Exception e)
            {
                ErrorHandlingManager.ReportError(e.Message, "Request.cs", "Post");
            }
            return response;
        }

        /// <summary>
        /// Read and handle HTTP web response.
        /// </summary>
        /// <param name="request">HttpWebRequest.</param>
        /// <returns>Response HttpStatusCode and string result.</returns>
        public static Dictionary<HttpStatusCode?, string> ReadHandledResponse(HttpWebRequest request)
        {
            // defaults
            Dictionary<HttpStatusCode?, string> result = null;

            // null request?
            if (request == null)
            {
                ErrorHandlingManager.ReportError("Error processing request", "Request.cs", "ReadHandledResponse");
                throw new Exception("No content.");
            }

            // handle response
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    string buffer = sr.ReadToEnd();
                    result = new Dictionary<HttpStatusCode?, string> {{response.StatusCode, buffer}};
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError && e.Response != null)
                {
                    var resp = (HttpWebResponse) e.Response;

                    if (resp == null)
                    {
                        return null;
                    }

                    result = new Dictionary<HttpStatusCode?, string> {{resp.StatusCode, null}};
                }

                if (e.Status == WebExceptionStatus.SecureChannelFailure)
                {
                    ErrorHandlingManager.ReportError(e.Message, "Request.cs",
                        "ReadHandledResponse");
                }
            }
            catch (IOException e)
            {
                ErrorHandlingManager.ReportError(e.Message, "Request.cs",
                    "ReadHandledResponse");
            }
            catch (Exception e)
            {
                ErrorHandlingManager.ReportError(e.Message, "Request.cs",
                    "ReadHandledResponse");
            }

            return result;
        }
    }
}

