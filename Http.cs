using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficService
{
    class Http
    {
        private HttpListener listener;
        private readonly RequestHandler requestHandler;
        private readonly CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource;

        public delegate void RequestHandler(HttpListenerContext context);

        public Http(string prefix, RequestHandler requestHandler)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            this.requestHandler = requestHandler;
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
        }

        public void Start()
        {
            listener.Start();
            Task.Run(() => {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        NonblockingListener();
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Error With the listening Proccess, Message : " + e.Message);
                    }

                }
            }, cancellationToken);
            Logger.Log("HTTP started. [" + String.Join(',', listener.Prefixes) + "]");
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            ListenerCallback(null);
            listener.Close();
            Logger.Log("HTTP stopped.");
        }

        public void NonblockingListener()
        {
            IAsyncResult result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }


        public void ListenerCallback(IAsyncResult result)
        {
            if (cancellationToken.IsCancellationRequested) return;

            listener = (HttpListener)result.AsyncState;


            HttpListenerContext context = listener.EndGetContext(result);

            requestHandler(context);
        }

    }
}
