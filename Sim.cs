using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrafficService
{
    class Sim
    {
        public Sim(IntPtr handle)
        {
            this.handle = handle;
        }
        private readonly IntPtr handle;

        private enum DATA_REQUESTS
        {
            REQUEST_1,
        };

        private enum DEFINITIONS
        {
            Struct1,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Struct1
        {
            // this is how you declare a fixed size string
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string title;
            public double latitude;
            public double longitude;
            public double altitude;
            public double heading;
        };

        public const int WM_USER_SIMCONNECT = 0x0402;
        private const uint MAXIMUM_RADIUS = 90000;
        private SimConnect simconnect = null;
        public SimConnect SimConnect
        {
            get
            {
                return simconnect;
            }
        }


        private void InitDataRequest()
        {
            try
            {
                // listen to connect and quit msgs
                simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Simconnect_OnRecvOpen);
                simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Simconnect_OnRecvQuit);

                // listen to exceptions
                simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(Simconnect_OnRecvException);

                // define a data structure
                simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Altitude", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "PLANE HEADING DEGREES TRUE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.000f, SimConnect.SIMCONNECT_UNUSED);

                // IMPORTANT: register it with the simconnect managed wrapper marshaller
                // if you skip this step, you will only receive a uint in the .dwData field.
                simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);

                // catch a simobject data request
                simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(Simconnect_OnRecvSimobjectDataBytype);
            }
            catch (COMException e)
            {
                Logger.Log("Exception initializing SimConnect: " + e.Message);
                CloseConnection();
            }
        }

        private TaskCompletionSource<Dictionary<uint, Struct1>> processed;
        Dictionary<uint, Struct1> aircraftData = null;

        private void Simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (processed == null)
            {
                return;
            }
            if ((DATA_REQUESTS)data.dwRequestID == DATA_REQUESTS.REQUEST_1)
            {
                if (data.dwentrynumber == 1)
                {
                    aircraftData = new Dictionary<uint, Struct1>();
                }

                Struct1 s1 = (Struct1)data.dwData[0];
                uint id = data.dwObjectID;

                if ((s1.latitude != 0 || s1.longitude != 0 || s1.altitude != 0) && id != 1) // exclude user aircraft and "ghost" aircraft
                {
                    aircraftData.Add(id, s1);
                }

                if (data.dwentrynumber == data.dwoutof)
                {
                    processed.TrySetResult(aircraftData);
                }
            }
        }

        private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Logger.Log("Connected to MSFS");
        }

        private void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Logger.Log("MSFS has exited");
            CloseConnection();
        }

        private void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Logger.Log("Exception received: " + data.dwException);
        }

        private Dictionary<uint, Struct1> cachedResult = null;
        private DateTime? cacheTime = null;
        private readonly object trafficLock = new object();
        public Dictionary<uint, Struct1> GetTraffic(bool useCache = true)
        {
            lock (trafficLock)
            {
                if (useCache && cachedResult != null && cacheTime != null && (DateTime.Now - cacheTime).Value.TotalMilliseconds < 1000)
                {
                    Logger.Log("Result from Cache.");
                    return cachedResult;
                }
                if (!Connect())
                {
                    return null;
                }
                Dictionary<uint, Struct1> result = null;
                var cts = new CancellationTokenSource(2000);
                try
                {
                    processed = new TaskCompletionSource<Dictionary<uint, Struct1>>();
                    CancellationToken ct = cts.Token;
                    ct.Register(() => processed.TrySetCanceled());
                    Task.Run(async () =>
                    {
                        simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, MAXIMUM_RADIUS, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);
                        await processed.Task;
                    }).Wait();
                    result = processed.Task.Result;
                    if (useCache)
                    {
                        cacheTime = DateTime.Now;
                        cachedResult = result;
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Execution exception: " + e.Message);
                }
                finally
                {
                    processed = null;
                    cts.Dispose();
                }
                Logger.Log("Result from SimConnect.");
                return result;
            }
        }

        private readonly object connectionLock = new object();
        public bool Connect()
        {
            lock (connectionLock)
            {
                if (simconnect != null)
                {
                    return true;
                }
                try
                {
                    simconnect = new SimConnect("Managed Data Request", this.handle, WM_USER_SIMCONNECT, null, 0);
                    InitDataRequest();
                    Logger.Log("SimConnect connected successfully.");
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log("Exception connecting SimConnect: " + e.Message);
                    CloseConnection();
                    return false;
                }
            }
        }

        public void CloseConnection()
        {
            lock (connectionLock)
            {
                if (simconnect != null)
                {
                    simconnect.Dispose();
                    simconnect = null;
                    Logger.Log("SimConnect connection closed.");
                }
            }
        }
    }
}
