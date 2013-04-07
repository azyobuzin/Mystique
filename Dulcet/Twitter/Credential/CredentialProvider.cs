﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Dulcet.Util;

namespace Dulcet.Twitter.Credential
{
    /// <summary>
    /// Provides credential information for access to Twitter.
    /// </summary>
    public abstract class CredentialProvider
    {
        /// <summary>
        /// Request methods
        /// </summary>
        public enum RequestMethod
        {
            /// <summary>
            /// GET request
            /// </summary>
            GET,

            /// <summary>
            /// POST request
            /// </summary>
            POST
        };

        /// <summary>
        /// Request API
        /// </summary>
        /// <param name="uri">target uri</param>
        /// <param name="method">target method</param>
        /// <param name="param">parameters</param>
        /// <returns>XML document</returns>
        public XDocument RequestAPI(string uri, RequestMethod method, IEnumerable<KeyValuePair<string, string>> param)
        {
            HttpWebRequest req;
            return RequestAPI(uri, method, param, out req);
        }

        /// <summary>
        /// Request API
        /// </summary>
        /// <param name="uri">target uri</param>
        /// <param name="method">target method</param>
        /// <param name="param">parameters</param>
        /// <param name="request">used request</param>
        /// <returns>XML document</returns>
        public abstract XDocument RequestAPI(string uri, RequestMethod method, IEnumerable<KeyValuePair<string, string>> param, out HttpWebRequest request);


        /// <summary>
        /// Request API, returns stream
        /// </summary>
        /// <param name="uri">target uri(full)</param>
        /// <param name="method">requesting method</param>
        /// <param name="param">parameters</param>
        /// <returns>Connection stream</returns>
        [Obsolete("safe disconnection of Streaming API requires HttpWebRequest.Abort(). so, you should use another overload which has out parameter.")]
        public Stream RequestStreamingAPI(string uri, RequestMethod method, IEnumerable<KeyValuePair<string, string>> param)
        {
            HttpWebRequest req;
            return RequestStreamingAPI(uri, method, param, out req);
        }


        /// <summary>
        /// Request API, returns stream
        /// </summary>
        /// <param name="uri">target uri(full)</param>
        /// <param name="method">requesting method</param>
        /// <param name="param">parameters</param>
        /// <param name="request">used request</param>
        /// <returns>Connection stream</returns>
        public abstract Stream RequestStreamingAPI(string uri, RequestMethod method, IEnumerable<KeyValuePair<string, string>> param, out HttpWebRequest request);

        /// <summary>
        /// Pursing header for check rate-limiting and generate XDocument
        /// </summary>
        /// <param name="res">WebResponse</param>
        /// <returns>XML Document</returns>
        protected XDocument XDocumentGenerator(WebResponse res)
        {
            return XDocumentGenerator(res, XmlReader.Create);
        }

        /// <summary>
        /// Pursing header for check rate-limiting and generate XDocument
        /// </summary>
        /// <param name="res">WebResponse</param>
        /// <param name="converter">Input converter</param>
        /// <returns>XML Document</returns>
        protected XDocument XDocumentGenerator(WebResponse res, Func<Stream, XmlReader> converter)
        {
            //read api rate
            try
            {
                using (var s = res.GetResponseStream())
                {
                    return XDocument.Load(converter(s));
                }
            }
            catch (XmlException)
            {
                throw;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update rate-limiting valuet
        /// </summary>
        [Obsolete]
        internal void UpdateRateLimit(int remain, int max, DateTime reset)
        {
            // Obsolete, do nothing.
        }
    }
}
