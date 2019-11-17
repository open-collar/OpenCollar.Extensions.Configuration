# OpenCollar.Extensions.Configuration
Support for automatic validation, update and strongly-typed access to configuration.

# Set-up Developer Environment

## Install Chocolately

Start a command shell in Administrator mode.  From there run the following to install Chocolatey:

```
@”%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe” -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command “iex ((New-Object System.Net.WebClient).DownloadString(‘https://chocolatey.org/install.ps1'))" && SET “PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin”
```
(For more details see the [Chocolately Website](https://chocolatey.org/docs/installation).)

## Install DocFX

In the same Administrator command line run the following to install DocFX:

```
cinst docfx -y
```

## Visual Studio 2019 Build Tools

Download and install the Visual Studio 2019 Build Tools (free) from the [Microsoft Visual Studio Downloads](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2019) page; look for "Build Tools for Visual Studio 2019" on the page.

## Project Resources

 * [Source Code](https://github.com/open-collar/OpenCollar.Extensions.Configuration)
 * [Issue Tracking](https://github.com/open-collar/OpenCollar.Extensions.Configuration/issues)

## Reference

 * [Options Pattern in .NET Core](https://codeburst.io/options-pattern-in-net-core-a50285aeb18d);
 * [Tutorial: Express your design intent more clearly with nullable and non-nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/nullable-reference-types);