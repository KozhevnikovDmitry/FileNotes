cd ..
cd .nuget
NuGet.exe restore ..\FileNotes.Monitor\FileNotes.Monitor.csproj -OutputDirectory ..\packages -Source https://www.nuget.org/api/v2/
MsBuild.exe ..\FileNotes.Monitor\FileNotes.Monitor.csproj /t:Build /p:Configuration=Release /p:TargetFramework=v4.0 /p:DebugSymbols=false /p:DebugType=None /p:OutDir=../FileNotes.Monitor/distr