# MSFSTrafficService

## API
### /status /check
Return status information:
```json
{
  "status":
    {
      "version":"0.1.0",
      "installed":true,
      "connected":false
    }
}
```
* status.version [string]: The version number of the service.
* status.installed [boolean]: If the service is installed and running. At the moment always `true` when reachable.
* status.connected [boolean]: `true` if a SimConnect connection could be established, otherwise `false`.

### /traffic
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
