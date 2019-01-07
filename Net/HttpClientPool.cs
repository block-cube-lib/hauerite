using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Haurite.Threading;

namespace Haurite.Net
{
    /// <summary>
    /// URIごとにHttpClientを使い回すためのPool
    /// </summary>
    public static class HttpClientPool
    {
        /// <summary>
        /// HttpClientの辞書
        /// キーはURI
        /// </summary>
        static Dictionary<Uri, HttpClient> clients = new Dictionary<Uri, HttpClient>();

        /// <summary>
        /// AsycLock
        /// </summary>
        static AsyncLock clientsLock = new AsyncLock();

        /// <summary>
        /// UriからHttpClientを取得
        /// </summary>
        /// <param name="endPointUri">接続先のURI</param>
        /// <returns>指定したURIに接続するHttpClient</returns>
        public static async Task<HttpClient> GetHttpClient(Uri endPointUri)
        {
            using (await clientsLock.LockAsync())
            {
                if (!clients.Any())
                {
                    var sp = ServicePointManager.FindServicePoint(endPointUri);
                    sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute
                }

                if (clients.ContainsKey(endPointUri))
                {
                    return clients[endPointUri];
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = endPointUri;
                clients[endPointUri] = client;
                return client;
            }
        }
    }
}
