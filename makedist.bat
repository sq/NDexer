@echo off
SET NDEXER_ROOT=%~dp0
SET NDEXER_REVSTR=$Rev$
SET NDEXER_REVID=%NDEXER_REVSTR:~6,-2%

echo __________________________________________________
msbuild "%NDEXER_ROOT%\Ndexer.sln" /v:m /nologo %*

echo __________________________________________________
mkdir dist
mkdir dist\temp
copy bin\debug\NDexer.exe dist\temp\
copy bin\debug\*.dll dist\temp\
mkdir dist\temp\data
copy data\*.* dist\temp\data\
mkdir dist\temp\ctags
copy ctags\ctags.exe dist\temp\ctags\
mkdir dist\packages

pushd dist\temp

echo __________________________________________________
..\..\ext\7zip\7z.exe a -r -t7z ..\packages\ndexer-r%NDEXER_REVID%.7z .\*

popd

echo __________________________________________________
