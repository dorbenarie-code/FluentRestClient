# FluentRestClient

FluentRestClient is a small C# library that demonstrates a clean and simple fluent API for building REST calls.

The goal of the project is to keep the public usage straightforward:
- configure a client with a base URL
- add headers or basic authentication
- create a request for a specific path
- send GET or POST requests
- receive a lightweight response object

The project currently includes:
- a core abstraction layer for clients, requests, and responses
- two infrastructure implementations:
  - `HttpClientRestClient`
  - `WebRequestRestClient`
- a small factory for choosing the desired client implementation

## Example

```csharp
using FluentRestClient.Factories;

var client = RestClientFactory
    .CreateDefault()
    .WithBaseUrl("https://api.example.com")
    .WithHeader("Accept", "application/json");

var response = await client
    .CreateRequest("users/1")
    .WithBasicAuth("demo", "secret")
    .GetAsync();

Console.WriteLine(response.StatusCode);
Console.WriteLine(response.Body);
Console.WriteLine(response.IsSuccess);
Project Structure
Core — public abstractions and response model
Infrastructure — concrete request/client implementations
Factories — factory methods for creating clients
Notes

This project is intentionally small and focused.
It is meant to practice fluent API design, separation between abstraction and implementation, and clean object-oriented structure in C#.
