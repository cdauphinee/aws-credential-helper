param ($runtime='win-x64')


if (Test-Path bin/$runtime -PathType Container) 
{
    rm -r -fo bin/$runtime/
}

dotnet publish AwsCredentialHelper/AwsCredentialHelper.csproj -o bin/$runtime/ -c Release --nologo -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained true -r $runtime
