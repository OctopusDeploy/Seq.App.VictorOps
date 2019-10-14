mkdir artifacts
mkdir publish

pushd artifacts
del *.* /F /Q
popd

pushd source\Seq.App.VictorOps
dotnet build
dotnet publish -o ..\..\publish
popd

pushd publish
c:\tools\octo.exe pack --id Seq.App.VictorOps --version %1 --outfolder ..\artifacts
popd

pushd artifacts
copy *.* c:\git\LocalPackages
popd
 