@echo off
set dotnet=dotnet
set iscc="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
set distributable_runtime=--self-contained true --runtime win-x86 --configuration Release
set single_file=-p:PublishSingleFile=true

if "%1"=="" goto all
if "%1"=="all" goto all
if "%1"=="build" goto build
if "%1"=="build-setup" goto build-setup
if "%1"=="clean-build" goto clean-build
if "%1"=="clean" goto clean

echo Command not found
goto done

:all
echo Building with built-in runtime...
%dotnet% publish %single_file% %distributable_runtime% -o publish .
echo Building Setup...
%iscc% InnoSetupScript.iss
goto clean-build


:build
echo Building with built-in runtime...
%dotnet% publish %single_file% %distributable_runtime% -o publish .
goto done

:clean-build
rmdir /s /q bin
rmdir /s /q obj
del publish\*.dll
del publish\*.pdb
goto done

:clean
echo Cleaning...
rmdir /s /q bin
rmdir /s /q obj
rmdir /s /q publish
goto done

:build-setup
echo Building Setup...
%iscc% InnoSetupScript.iss
goto done

:done
echo Done!