// Copyright © 2012 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using CefSharp;
using CefSharp.Handler;

namespace SellAvi.Browser
{
    /// <summary>
    ///     DefaultRequestHandler provides a base class for you to inherit from
    ///     you only need to implement the methods that are relevant to you.
    ///     If you implement the IRequestHandler interface you will need to
    ///     implement every method
    /// </summary>
    public class RequestHandler : DefaultRequestHandler
    {
        public static readonly string VersionNumberString = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
            Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

        public string auth_user { get; set; }
        public string auth_pass { get; set; }

        public event EventHandler PostDataParamsChanged;

        private void OnPostDataParamsChanged(NameValueCollection PostCollection)
        {
            if (PostDataParamsChanged != null) PostDataParamsChanged(this, new PostDataEventArgs(PostCollection));
        }


        //private Dictionary<UInt64, MemoryStreamResponseFilter> responseDictionary = new Dictionary<UInt64, MemoryStreamResponseFilter>();

        public override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public override bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode,
            string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            //callback.Dispose();
            //return false;

            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            if (!callback.IsDisposed)
                using (callback)
                {
                    //To allow certificate
                    //callback.Continue(true);
                    //return true;
                }

            return false;
        }

        public override void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
            // TODO: Add your own code here for handling scenarios where a plugin crashed, for one reason or another.
        }

        public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IRequestCallback callback)
        {
            Uri url;
            if (Uri.TryCreate(request.Url, UriKind.Absolute, out url) == false) return CefReturnValue.Cancel;

            //Example of how to set Referer
            // Same should work when setting any header

            //Example of setting User-Agent in every request.
            //var headers = request.Headers;

            //var userAgent = headers["User-Agent"];
            //headers["User-Agent"] = userAgent + " CefSharp";

            //request.Headers = headers;

            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            //callback.Dispose();
            //return false;

            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            if (!callback.IsDisposed)
                using (callback)
                {
                    if (request.Method == "POST")
                        using (var postData = request.PostData)
                        {
                            if (postData != null && request.Url == "https://www.avito.ru/additem")
                            {
                                var elements = postData.Elements;

                                var charSet = request.GetCharSet();

                                foreach (var element in elements)
                                    if (element.Type == PostDataElementType.Bytes)
                                    {
                                        var body = element.GetBody(charSet);
                                        var postString = HttpUtility.UrlDecode(body);
                                        var nvc = HttpUtility.ParseQueryString(postString);
                                        OnPostDataParamsChanged(nvc);
                                    }
                            }
                        }

                    //Note to Redirect simply set the request Url
                    //if (request.Url.StartsWith("https://www.google.com", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    request.Url = "https://github.com/";
                    //}

                    //Callback in async fashion
                    //callback.Continue(true);
                    //return CefReturnValue.ContinueAsync;
                }

            return CefReturnValue.Continue;
        }

        public override bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.

            //callback.Dispose();
            if (isProxy)
            {
                callback.Continue(auth_user, auth_pass);

                return true;
            }

            return false;
        }

        public override bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy,
            string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            callback.Dispose();
            return false;
        }

        public override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser,
            CefTerminationStatus status)
        {
            // TODO: Add your own code here for handling scenarios where the Render Process terminated for one reason or another.
            //browserControl.Load(CefExample.RenderProcessCrashedUrl);
        }

        public override bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl,
            long newSize, IRequestCallback callback)
        {
            //NOTE: If you do not wish to implement this method returning false is the default behaviour
            // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
            //callback.Dispose();
            //return false;

            //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
            if (!callback.IsDisposed)
                using (callback)
                {
                    //Accept Request to raise Quota
                    //callback.Continue(true);
                    //return true;
                }

            return false;
        }

        public override void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, ref string newUrl)
        {
            //Example of how to redirect - need to check `newUrl` in the second pass
            //if (request.Url.StartsWith("https://www.google.com", StringComparison.OrdinalIgnoreCase) && !newUrl.Contains("github"))
            //{
            //    newUrl = "https://github.com";
            //}
        }

        public override bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return url.StartsWith("mailto");
        }

        public override void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public override bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IResponse response)
        {
            //NOTE: You cannot modify the response, only the request
            // You can now access the headers
            //var headers = response.Headers;

            return false;
        }

        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser,
            IFrame frame, IRequest request, IResponse response)
        {
            var url = new Uri(request.Url);
            /*if (url.Scheme == CefSharpSchemeHandlerFactory.SchemeName)
            {
                if(request.Url.Equals(CefExample.ResponseFilterTestUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return new FindReplaceResponseFilter("REPLACE_THIS_STRING", "This is the replaced string!");
                }

                if (request.Url.Equals("custom://cefsharp/assets/js/jquery.js", StringComparison.OrdinalIgnoreCase))
                {
                    return new AppendResponseFilter(System.Environment.NewLine + "//CefSharp Appended this comment.");
                }

                //Only called for our customScheme
                var dataFilter = new MemoryStreamResponseFilter();
                responseDictionary.Add(request.Identifier, dataFilter);
                return dataFilter;
            }*/


            if (url.ToString() == "https://www.avito.ru/additem") return null;

            return null;
        }

        public override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            //var url = new Uri(request.Url);
            //if (url.Scheme == CefSharpSchemeHandlerFactory.SchemeName)
            //{
            //    MemoryStreamResponseFilter filter;
            //    if (responseDictionary.TryGetValue(request.Identifier, out filter))
            //    {
            //        //TODO: Do something with the data here
            //        var data = filter.Data;
            //        var dataLength = filter.Data.Length;
            //        //NOTE: You may need to use a different encoding depending on the request
            //        var dataAsUtf8String = Encoding.UTF8.GetString(data);
            //    }
            //}
        }

        public class PostDataEventArgs : EventArgs
        {
            public PostDataEventArgs(NameValueCollection data)
            {
                PostCollection = data;
            }

            public NameValueCollection PostCollection { get; set; }
        }
    }
}