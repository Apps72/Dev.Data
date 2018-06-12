To create the NuGet tool package: 
   C:\> dotnet pack -c Release 

To install and use this package locally:
   Copy the generated package to C:\Program Files\dotnet\sdk\NuGetFallbackFolder\Apps72.Dev.Data.Generator.Tools
   C:\> dotnet tool install -g Apps72.Dev.Data.Generator.Tools

The application will be installer in the current user folder: %USERPROFILE%\.dotnet\tools

To display all tool installed:
  C:\> dotnet tool list -g

To uninstall the tool:
  C:\> dotnet tool uninstall -g Apps72.Dev.Data.Generator.Tools