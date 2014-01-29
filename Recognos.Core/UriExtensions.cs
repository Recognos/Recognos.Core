namespace Recognos.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Useful extensions for Uri processing.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Regular expression for matching uris
        /// </summary>
        public static readonly Regex UriExpression = new Regex(@"([a-z0-9+.-]+):(?://(?:((?:[a-z0-9-._~!$&'()*+,;=:]|%[0-9A-F]{2})*)@)?((?:[a-z0-9-._~!$&'()*+,;=]|%[0-9A-F]{2})*)(?::(\d*))?(/(?:[a-z0-9-._~!$&'()*+,;=:@/]|%[0-9A-F]{2})*)?|(/?(?:[a-z0-9-._~!$&'()*+,;=:@]|%[0-9A-F]{2})+(?:[a-z0-9-._~!$&'()*+,;=:@/]|%[0-9A-F]{2})*)?)(?:\?((?:[a-z0-9-._~!$&'()*+,;=:/?@]|%[0-9A-F]{2})*))?(?:#((?:[a-z0-9-._~!$&'()*+,;=:/?@]|%[0-9A-F]{2})*))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Extracts the uris from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>A collection of Uris</returns>
        public static IEnumerable<Uri> ExtractUris(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                yield break;
            }

            MatchCollection matches = UriExpression.Matches(input);

            foreach (Match match in matches)
            {
                Uri result;
                if (Uri.TryCreate(match.Value, UriKind.Absolute, out result))
                {
                    if (!string.IsNullOrEmpty(result.Host))
                    {
                        yield return result;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified URI is a web uri (http or https).
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        ///   <c>true</c> if the specified URI is http or https; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWeb(this Uri uri)
        {
            Check.NotNull(uri, "uri");
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// Resolves the redirects for the specified URI is required.
        /// </summary>
        /// <remarks>
        /// If http errors are encountered the input uri is returned.
        /// </remarks>
        /// <param name="uri">The URI.</param>
        /// <returns>The final uri after following the redirects.</returns>
        public static Uri ResolveRedirects(this Uri uri)
        {
            return uri.ResolveRedirects(x => uri);
        }

        /// <summary>
        /// Resolves the redirects for the specified URI is required.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="error">Action to be performed when http error is encountered.</param>
        /// <returns>The final uri after following the redirects.</returns>
        public static Uri ResolveRedirects(this Uri uri, Func<WebException, Uri> error)
        {
            Check.NotNull(uri, "uri");
            Check.Condition(uri.IsWeb(), "Redirects can be resolved only for web requests");

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            request.Method = WebRequestMethods.Http.Head;

            request.UserAgent = @"Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    return response.ResponseUri;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null && response.StatusCode == HttpStatusCode.MethodNotAllowed)
                {
                    if (response.ResponseUri != null && response.ResponseUri != uri)
                    {
                        return response.ResponseUri;
                    }

                    request = WebRequest.Create(uri) as HttpWebRequest;
                    request.UserAgent = @"Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                    request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    try
                    {
                        using (response = request.GetResponse() as HttpWebResponse)
                        {
                            return response.ResponseUri;
                        }
                    }
                    catch (WebException webex)
                    {
                        return error(webex);
                    }
                }

                return error(ex);
            }
        }
    }
}
