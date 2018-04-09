// Copyright (c) MadDonkeySoftware

namespace Runner
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;

    /// <summary>
    /// A class representing a job runner within the system.
    /// </summary>
    public class SmartHttpClient : IDisposable
    {
        private readonly HttpClient client;
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartHttpClient"/> class.
        /// </summary>
        public SmartHttpClient()
        {
            this.client = new HttpClient();
        }

        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        /// <returns>
        /// The headers which should be sent with each request.
        /// </returns>
        public HttpRequestHeaders DefaultRequestHeaders
        {
            get
            {
                return this.client.DefaultRequestHeaders;
            }
        }

        /// <summary>
        /// Performs an HTTP get against the resource. Should a HTTP 5XX be returned the
        /// request will be retried after the delay period has ellapsed.
        /// </summary>
        /// <param name="uri">The resource to request.</param>
        /// <param name="retries">Maximum number of times to try the request.</param>
        /// <param name="delay">The delay between retries.</param>
        /// <returns>A string containing the response body.</returns>
        public string GetWithReties(Uri uri, int retries = 3, int delay = 500)
        {
            HttpResponseMessage response = null;
            var tries = 0;
            do
            {
                if (response != null)
                {
                    Thread.Sleep(delay);
                }

                response = this.client.GetAsync(uri).Result;
                tries += 1;
            }
            while ((int)response.StatusCode >= 500 && tries < retries);

            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by the RequestHelper.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by the RequestHelper.
        /// </summary>
        /// <param name="disposing">true if cleaning up managed and unmanaged resources, false if only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.client.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}