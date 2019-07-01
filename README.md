# ASP-MVC-ATM

## Description

This app is built on .NET Core using ASP.NET. 
The app represents simple web-based version of automated teller machine (ATM) terminal.

## Compatibility

This app is compatible with ASP.NET version 2.2 and is able to work on Windows due to database provider limitations.

## Run

To run the app on Windows, open the console in containing folder, then go to 'ATM' folder and from there launch the app using the following command:

```
dotnet run
```

You may have to sign https certificate and accept the self-signed certificate in browser.

The server will start and the app can be accessed via browser at the access points, that will be displayed in the console window as follows:

```
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
```

Other ways to launch the app may require additional environment setup for all functions to work properly. To enable custom error page, set environment to anything other than `Development`. Guidelines to environment setup can be found [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-2.2).

## Planned functions

* ~~Simple login using card number and pin~~
* ~~Withdrawal and account information interfaces~~
* ~~Operation failure pages~~
* ~~Records about user actions~~
* ~~Bootstrap-powered UI~~
* ~~Data input using on-screen numpad only~~
* Unit-tests