# What goes here?

This project is the public facing API of the solution.  
Everything related to .NET Core API should go here.  
The API exposes [json-api](https://jsonapi.org/) data, and follow the standard for most endpoints.  

## Authentication

There exists multiple IdP clients, and multiple frontends which can connect to this API using one of the IdP's for authentication.  
To determine which IdP client a frontend wants to use, it must send the header `x-api-key` with all requests.  
For some environments the request from the frontend towards the backend is intercepted and the header is attached for security reasons.  
For local development and environments without that capability a simple hard-coded api-key will suffice.  

## JSON Api

The API is exposed following the standards of [json-api](https://jsonapi.org/).  
A NuGet-package have been used to create an output-formatter which basically means that internally in the code we don't deal with json-api and it's structure.  
When a response is sent out from the API the formatter will convert the response data to json-api data.  