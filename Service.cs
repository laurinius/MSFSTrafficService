using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TrafficService
{
    class Service
    {
        public Service(Sim sim, string version)
        {
            this.sim = sim;
            this.version = version;
        }

        private readonly Sim sim;
        private readonly string version;

        public void ProcessRequest(HttpListenerContext context)
        {
            string[] segments = context.Request.Url.Segments;
            if (segments.Length < 2)
            {
                ProcessDefaultRequest(context);
            }
            else
            {
                switch (segments[1].Trim('/').ToLower())
                {
                    case "status":
                    case "check":
                        ProcessCheckRequest(context);
                        break;
                    case "traffic":
                        ProcessTrafficRequest(context);
                        break;
                    default:
                        ProcessDefaultRequest(context);
                        break;
                }
            }
        }

        private void ProcessCheckRequest(HttpListenerContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append("\"status\":");
            sb.Append('{');
            sb.Append("\"version\":\"").Append(version).Append("\",");
            sb.Append("\"installed\":").Append(true.ToString().ToLower()).Append(",");
            sb.Append("\"connected\":").Append(sim.Connect().ToString().ToLower());
            sb.Append('}');
            sb.Append('}');
            JsonResponse(context, sb.ToString(), 200);
        }

        private void ProcessDefaultRequest(HttpListenerContext context)
        {
            JsonResponse(context, "{\"message\":\"Resource not found.\"}", 404);
        }

        private void ProcessTrafficRequest(HttpListenerContext context)
        {
            JsonResponse(context, ToJson(sim.GetTraffic()), 200);
        }

        private void JsonResponse(HttpListenerContext context, string json, int statusCode)
        {
            HttpListenerResponse response = context.Response;
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "POST, GET");
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private static string ToJson(Dictionary<uint, Sim.Struct1> aircrafts)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            if (aircrafts != null)
            {
                string sep = "";
                foreach (var entry in aircrafts)
                {
                    sb.Append(sep);
                    sb.Append('{');
                    sb.Append("\"uId\":").Append(entry.Key).Append(",");
                    sb.Append("\"lat\":").Append(entry.Value.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(',');
                    sb.Append("\"lon\":").Append(entry.Value.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(',');
                    sb.Append("\"alt\":").Append((entry.Value.altitude * 0.3048).ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(',');
                    sb.Append("\"heading\":").Append(entry.Value.heading.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    sb.Append('}');
                    sep = ",";
                }
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
