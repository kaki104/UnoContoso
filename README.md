# UnoContoso
Port the Contoso UWP project to the Uno-Prism project.

## Source
https://github.com/microsoft/Windows-appsample-customers-orders-database

## Environment
- unoapp-prism Uno.ProjectTemplates.Dotnet::3.2.0-dev.254

- Microsoft.EntityFrameworkCore v3.1.9
- Microsoft.EntityFrameworkCore.Sqlite v3.1.9
- NETStandard.Library v2.0.3

- Prism.Core v8.0.0.1909
- Prism.DryIoc.Uno v8.0.0.1909

- Uno.Microsoft.Toolkit.Uwp.UI.Controls v6.1.0-build.191
- Uno.Microsoft.Toolkit.Uwp.UI.Controls.DataGrid v6.1.0-build.191

- Uno.UI v3.2.0-dev.254

- Uno.WindowsStateTriggers v1.1.1-uno.132
- WindowsStateTriggers v1.1.0

- UWP Target version : Windows 10, version 1903(10.0; Build 18362)
- UWP Min version : Windows 10, version 1809(10.0; Build 17763)

## How to run
- Click the Code button at the top of the page and copy the clone address.
- Start Visual Studio 2019 and choose Clone a repository.
- Paste the copied address to the Repository location and click the Clone button.
- Select Build -> Configuration Manager menu.
  + Check Build and Deploy of UnoContoso.Droid, Models, Repository, Service, Uwp, Wasm project.
- Press the F6 key to proceed with the build process.
  + If there are components required to build, proceed with additional installation.
- Change the startup project to UnoContoso.Service.
  + Change the startup method from IIS Express to UnoContoso.Service.
- Go to the solution "UnoContoso" property page. (Select the solution title and press Alt+Enter)
- Select Multiple startup projects and change the Action of the UnoContoso.Service and UnoContoso.Uwp projects to Start.
- Press the Start button to start the service and UWP app at the same time.


