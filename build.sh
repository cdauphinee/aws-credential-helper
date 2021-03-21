if [[ -z "$1" ]] ;
then
    echo 'Please specify the runtime to build against (ex. osx-x64 or linux-x64)'
    exit 1
fi

rm -rf bin/$1/
dotnet publish AwsCredentialHelper/AwsCredentialHelper.csproj -o bin/$1/ -c Release --nologo -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained true -r $1
