# MSFSTrafficService

## Overview
This is small app that connects to MSFS via SimConnect and provides a Web-API to query the traffic from the sim.

The app will try to establish the SimConnect connection automatically, but the user can start and stop the inbuild webserver for the API manually.  
An option to automatically start the webserver with the app is also available and active by default.

The structure of the JSON response containing the aircraft data matches the structure of the data that is available from `Coherent.call("GET_AIR_TRAFFIC")` in the MSFS JavaScript facilities, though that call is limited to only the inbuilt "Live"-traffic, this app also provides traffic from Offline-AI and injected traffic.

## API
### GET /status /check
Return status information:
```json
{
  "status":
    {
      "version":"0.1.0",
      "installed":true,
      "connected":true
    }
}
```
* status.version [string]: The version number of the service.
* status.installed [boolean]: If the service is installed and running. At the moment always `true` when reachable.
* status.connected [boolean]: `true` if a SimConnect connection could be established, otherwise `false`.

### GET /ready
Simplified check that returns `true` when ready and a SimConnect connection could be established, `false` otherwise.

### GET /traffic
Returns a list of simulator traffic.
```json
[
  {
    "uId":12345,
    "lat":10.234,
    "lon":10.234,
    "alt":10.234,
    "heading":10.234
  },
]
```
* uId [uint]: The traffic object ID.
* lat [double]: The latitude of the traffic object.
* lon [double]: The longitude of the traffic object.
* alt [double]: The altitude in meters MSL of the traffic object.
* heading [double]: The heading of the traffic object.
