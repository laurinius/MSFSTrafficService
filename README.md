# MSFSTrafficService

## Overview
This is small app that connects to MSFS via SimConnect and provides a Web-API to query the traffic from the sim.

The app will try to establish the SimConnect connection automatically, but the user can start and stop the inbuild webserver for the API manually.  
An option to automatically start the webserver with the app is also available and active by default.

The structure of the JSON response containing the aircraft data matches the structure of the data that is available from `Coherent.call("GET_AIR_TRAFFIC")` in the MSFS JavaScript facilities, though that call is limited to only the inbuilt "Live"-traffic, this app also provides traffic from **injected multiplayer traffic** (e.g. VatSim via vPilot) and **Offline-AI traffic**.

**Inbuilt-Multiplayer traffic is not supported at the moment.** The data needed for that is not exposed via SimConnect.

## Usage
Download and unzip the latest release [here](https://github.com/laurinius/MSFSTrafficService/releases/latest) and run `MSFSTrafficService.exe`.  
The webservice starts automatically with the app by default, but this can be disabled with the Auto-Run checkbox.

Since version 0.2.0 there is a configurable cache time. When the last SimConnect is younger than the cache time, that result will be return and no call to SimConnect will be performed.  
The default of 50ms (20 requests per second) should be sufficient for most use cases while preventing an unreasonable load on the SimConnect API, as non-stop calling that API for traffic information can result in a performance loss in the sim.  
Ideally the caller of the service should limit the requests to what is necessary for its functionality, but this setting gives the user an option to control the amount of SimConnect requests passed through the service.

## Known Issues
* ---

## Add-On integration
The service can be integrated into HTML/JavaScript addons by calling the web API.  
See this example on how to call the API from JavaScript:
```javascript
class ExternalTraffic {
    constructor(port = 8383) {
        this.port = port;
        this.timeout = 1000;
    }

    callAvailable() {
        return this.request(this.getUrl("/status"), this.timeout).then(
            json => json.status.connected === true,
            () => false
        );
    }
    
    callTraffic() {
        return this.request(this.getUrl("/traffic"), this.timeout).then(
            json => json,
            () => []
        );
    }
    
    getUrl(path) {
        return "http://localhost:" + this.port + path;
    }

    request(url, timeout) {
        return new Promise((resolve, reject) => {
            let httpRequest = new XMLHttpRequest();
            httpRequest.timeout = timeout;
            httpRequest.onload = function() {
                try {
                    const json = JSON.parse(this.responseText);
                    resolve(json);
                } catch (e) {
                    reject();
                }
            };
            httpRequest.onerror = function() {
                reject();
            };
            httpRequest.ontimeout = function() {
                reject();
            };
            httpRequest.open('GET', url);
            httpRequest.send(null);
        });
    }
}

let externalTraffic = new ExternalTraffic();
let externalTrafficAvailable = false;

// Check availability every 10 seconds
let checkAvailability = () => {
	externalTraffic.callAvailable().then(isAvailable => {
		externalTrafficAvailable = isAvailable;
	});
};
checkAvailability();
setInterval(checkAvailability, 10000);

// Process traffic every second
setInterval(() => {
	if (externalTrafficAvailable) 
		externalTraffic.callTraffic().then(trafficArray => {
			for (let i = 0; i < trafficArray.length; i++) {
				let aircraft = trafficArray[i];
				let id = aircraft.id;
				let lat = aircraft.lat;
				let lon = aircraft.lon;
				let alt = aircraft.alt;
				let heading = aircraft.heading;
				// Do something
			}
		});
	}, 1000);
```

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
Returns an array of simulator traffic.
Returns an empty array in error cases (e.g. if traffic could not be retreived via SimConnect).
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
* heading [double]: The heading (degrees true) of the traffic object.
