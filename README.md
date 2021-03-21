## Overview
This is a bare-bones replacement for the `aws codecommit credential-helper` command from the AWS CLI. The motivation behind this tool is to make git operations against CodeCommit repositories faster, by avoiding the slow start up of the AWS CLI.

Unlike the AWS CLI, **this tool only retrieves credentials from the EC2 instance metadata service**. It does not read them from your credentials file or environment variables. If you want to use it on your own machine, you will need to run a local metadata service, such as [aws-vault](https://github.com/99designs/aws-vault) in server mode.

## Requirements
* [.NET 5 Runtime](https://dotnet.microsoft.com/download/dotnet/5.0)

## Installation
Download and extract the AwsCredentialHelper archive appropriate for your platform from the [latest releases](https://github.com/cdauphinee/aws-credential-helper/releases/latest), or build it yourself (see instructions below). You will probably want to place the executable somewhere in your system's PATH, otherwise you will have to specify the full path to its location in your Git config.

Follow the AWS CodeCommit documentation ([Windows](https://docs.aws.amazon.com/codecommit/latest/userguide/setting-up-https-windows.html#setting-up-https-windows-credential-helper)/[Linux](https://docs.aws.amazon.com/codecommit/latest/userguide/setting-up-https-unixes.html#setting-up-https-unixes-credential-helper)), but replace `aws codecommit credential-helper` with AwsCredentialHelper:

```
git config --global credential.helper "!AwsCredentialHelper $@"
```

## Building
Building from source requires the .NET 5 SDK, in addition to the runtime. You build the assembly by running `dotnet build` in the repository root, which will output AwsCredentialHelper.dll; the full path to the assembly can be found at the end of the build logs.

In order to use this DLL as the credential helper, you will need to change your Git configuration to run it via dotnet:
```
git config --global credential.helper "!dotnet /path/to/AwsCredentialHelper.dll $@"
```

### Self-Contained Executable
To create the self-contained executables found in the Releases section, you can run either [build.sh](build.sh) for Linux/OS X or [build.ps1](build.ps1) for Windows. Both of these scripts take the target runtime as arguments and will publish the executable to the `bin` folder. You can find a list of runtime identifiers [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).
