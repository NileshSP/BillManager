# Bill Manager

## Bill manager ASP.NET 5 web api project

ASP.NET 5 Web API exposing Bill & Friend entities with relative bill share interactions between them

Devops - handled using [Azure DevOps](https://dev.azure.com)

[![Build Status](https://dev.azure.com/nspatel/BillManager/_apis/build/status/BillManagerApp%20-%20CI?branchName=master)](https://dev.azure.com/nspatel/BillManager/_build/latest?definitionId=4&branchName=master)

Demo links
- [PROD](https://billmanagerapp.azurewebsites.net/)
- [UAT](https://billmanagerapp-uat.azurewebsites.net/)
- [DEV](https://billmanagerapp-dev.azurewebsites.net/)

# Steps to get the project running

Pre-requisites:

>1. [.Net 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
>2. [Visual Studio Code](https://code.visualstudio.com/) or Recommended - [Visual Studio Community editon version 15.9.1](https://visualstudio.microsoft.com/vs/community/) or later editor

<br/>

Clone the current repository locally as
 `git clone https://github.com/NileshSP/BillManager.git`

<br/>

Steps: using Visual Studio community edition editor
>1. Open the solution file (.sln) available in the root folder of the downloaded repository
>2. Await until the project is ready as per the status shown in taskbar which loads required packages in the background
>3. Hit -> F5 or select 'Debug -> Start Debugging' option to run the project

<br/>

Steps: using Visual Studio code editor
>1. Open the root folder of the downloaded repository 
>2. Await until the project is ready as per the status shown in taskbar which loads required packages in the background
>3. Open Terminal - 'Terminal -> New Terminal' and execute commands as `cd BillManager` & `dotnet build` & `dotnet run` sequentially
OR
>4. Hit -> F5 or select 'Debug -> Start Debugging' option to run the project

<br/>

Once the project is build and run, a browser page would be presented with navigation options on right wherein 'Websites data' option contains functionality related to data access from in-memory/sql database


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.


## License
[MIT](https://choosealicense.com/licenses/mit/)
