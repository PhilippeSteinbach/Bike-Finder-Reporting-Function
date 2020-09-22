
## Reporting
This is the Reporting mechanism for the Bike-Finder Service. It enables sending automatic reports about the bike usage based on the history-Schema of the CMS.

## Prerequisites

### Install Visual Studio 2019

Refer [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/) to install Visual Studio 2019

### Install .NET Core 3.1 SDK

Refer [https://dotnet.microsoft.com/download/dotnet-core/3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) to install the latest .NET Core 3.1 SDK

### Set up local environment variables

Rename **local.settings.example** to **local.settings** and fill out all properties from the bike-finder Squidex App (Settings -> Client -> Client ID/Client Secret).


## Cloning and Running the Application

Clone the application to local

Go into the project Folder and install the packages using the following command
```
dotnet restore
```
Open the Reporting.sln within Visual Studio and press **Ctrl + F5**

The application runs in the Azure Function simulator

