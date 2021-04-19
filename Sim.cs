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

                if (id != 0 && id != 1) // Exclude user aircraft
                {
                    aircraftData.Add(id, s1);
                }

                //Debug.WriteLine("Title: " + s1.title);
                //Debug.WriteLine("ID:    " + id);
                //Debug.WriteLine("Lat:   " + s1.latitude);
                //Debug.WriteLine("Lon:   " + s1.longitude);
                //Debug.WriteLine("Alt:   " + s1.altitude);
                //Debug.WriteLine("HDG:   " + s1.heading);

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

        public Dictionary<uint, Struct1> GetTraffic()
        {
            if (!Connect())
            {
                return null;
            }
            Dictionary<uint, Struct1> result = null;
            try
            {
                processed = new TaskCompletionSource<Dictionary<uint, Struct1>>();
                Task.Run(async () => {
                    simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, MAXIMUM_RADIUS, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);
                    await processed.Task;
                }).Wait(); 
                result = processed.Task.Result;
            }
            finally
            {
                processed = null;
            }
            return result;
        }

        public bool Connect()
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

        public void CloseConnection()
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
