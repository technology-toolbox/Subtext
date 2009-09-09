using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Subtext.Akismet.Properties;

namespace Subtext.Akismet
{
    /// <summary>
    /// Class used to make the actual HTTP requests.
    /// </summary>
    /// <remarks>
    /// Yeah, I know you're thinking this is overkill, but it makes it 
    /// easier to write tests to have this layer of abstraction from the 
    /// underlying Http request.
    /// </remarks>
    public class HttpClient
    {
        /// <summary>
        /// Posts the request and returns a text response.  
        /// This is all that is needed for Akismet.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="formParameters">The properly formatted parameters.</param>
        /// <returns></returns>
        public virtual string PostRequest(Uri url, string userAgent, int timeout, string formParameters)
        {
            return PostRequest(url, userAgent, timeout, formParameters, null);
        }

        /// <summary>
        /// Posts the request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="formParameters">The form parameters.</param>
        /// <param name="proxy">The proxy.</param>
        /// <returns></returns>
        public virtual string PostRequest(Uri url, string userAgent, int timeout, string formParameters, IWebProxy proxy)
        {
            ServicePointManager.Expect100Continue = false;
            var request = WebRequest.Create(url) as HttpWebRequest;

            if(proxy != null)
            {
                request.Proxy = proxy;
            }

            if(null != request)
            {
                request.UserAgent = userAgent;
                request.Timeout = timeout;
                request.Method = "POST";
                request.ContentLength = formParameters.Length;
                request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                request.KeepAlive = true;

                using(var myWriter = new StreamWriter(request.GetRequestStream()))
                {
                    myWriter.Write(formParameters);
                }
            }

            var response = (HttpWebResponse)request.GetResponse();
            if(response.StatusCode < HttpStatusCode.OK && response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                throw new InvalidResponseException(
                    string.Format(CultureInfo.InvariantCulture, Resources.InvalidResponse_ServiceUnableToHandleRequest,
                                  response.StatusCode), response.StatusCode);
            }

            string responseText;
            using(var reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
                //They only return "true" or "false"
            {
                responseText = reader.ReadToEnd();
            }

            return responseText;
        }
    }
}