To modify this documentation

1. Install DocFX
    - Download the ZIP file from https://github.com/dotnet/docfx/releases.
    - Copy the ZIP content to a local folder.
    - Optional: Add this folder to your PATH environment variable.

2. Create MarkDown pages in Tutorials folder
   And referenced it via toc.ylm

3. Generate a static documentation (folder '_site') using this command:
      $> docfx --serve

PS: You need to build the C# project to have a DLL in "bin/Release/netstandard2.0" folder.