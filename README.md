# csharp-openhab-rest-client-backend

HTTP backend for the [csharp-openhab-rest-client test app](https://github.com/Michdo93/csharp-openhab-rest-client-test-app):
a browser cannot run .NET, so this small ASP.NET Core server calls
[csharp-openhab-rest-client](https://github.com/Michdo93/csharp-openhab-rest-client) 2.0.1 on its behalf.
Every method of the library is reachable – 153 REST methods and 25 event streams, found by
reflection, so a new version of the library needs no change here.

| Endpoint | Body / query | Answer |
|---|---|---|
| `GET /` | – | `{"status":"ok","library":"csharp-openhab-rest-client","version":"2.0.1"}` |
| `GET /api/methods` | – | all methods with parameters (name, type, required, default) |
| `POST /api/connect` | `{url, token}` or `{url, username, password}` | `{loggedIn, isCloud, url}` |
| `POST /api/call` | `{url, token, class, method, args: {...}}` | result of the library method |
| `GET /api/events/stream` | `?url&token&class&method&args=<json>` | Server-Sent Events |

`class` and `method` are the names of the library (`Items`, `SendCommand`; the Python name
`sendCommand` works too), `args` holds the parameters by name:

```bash
curl -s localhost:8080/api/call -H 'Content-Type: application/json' -d '{"url":"http://openhab:8080",
  "token":"oh.xxx","class":"Items","method":"SendCommand","args":{"itemName":"Wohnzimmer_Licht","command":"ON"}}'
# {"message":"OK"}
```

Results are exactly those of the library: JSON, text as JSON string, `{"message":"OK"}`,
errors as `{"error":"..."}`. Without `class` the event stream is `Events.GetEvents` with
`topics` (e.g. `openhab/items/*/statechanged`).

The backend keeps no credentials; they come with every request. Run it only where you would
also run openHAB clients – everybody who reaches it can use it as a proxy.

## Build and run

```bash
dotnet run -c Release                  # PORT=8080 by default
```

Requirements: .NET 10 SDK. Docker: `docker build -t csharp-openhab-backend .` – `render.yaml`
deploys it on Render (Frankfurt, free plan).

## License

MIT – Michael Christian Dörflinger
