pushd nuspec
del *.nupkg
nuget pack OpenCoverTestImpactProvider.nuspec

NuGet.exe push *.nupkg -Source http://ts-nuget/nuget/ts-oss-libraries/
popd