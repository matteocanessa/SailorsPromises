cd ..
cd ..

echo off

cls

echo deleting old packages...
del SailorsPromises*.nupkg

echo Updating nuget if necessary...
NuGet Update -self

echo Packaging...
NuGet Pack ./SailorsPromises.nuspec -Prop Configuration=Release
echo Packaged successfully

echo publishing package...
NuGet Push SailorsPromises*.nupkg
echo Package published successfully

echo on