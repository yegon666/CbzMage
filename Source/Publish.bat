echo off
set targets=CbzMage_Win CbzMage_Linux CbzMage_macOS
set zip="C:\Program Files\7-Zip\7z.exe" a -tzip
cd ..\publish
for %%v in (*.minor) do set oldminor=%%~nv
set /a newminor=oldminor + 1
move %oldminor%.minor %newminor%.minor >nul:
for %%v in (*.major) do set major=%%~nv
set oldversion=%major%.%oldminor%
set newversion=%major%.%newminor%
echo Version: %newversion%
for %%t in (%targets%) do if exist %%t rmdir /s /q %%t
for %%t in (%targets%) do if exist %%t%oldversion% rmdir /s /q %%t%oldversion%
for %%t in (%targets%) do if exist %%t%newversion% rmdir /s /q %%t%newversion%
cd ..\source\cbzmage
for %%t in (%targets%) do (
	echo Publish %%t
	dotnet publish -p:publishprofile=%%t >nul:
)
cd ..\..\publish
for %%t in (%targets%) do call :create_target %%t
cd ..\Source
echo Done
pause
exit /b
:create_target
setlocal enabledelayedexpansion
set base=%1
del %base%\*.pdb
del %base%\*.development.json
set full=!base:_=%newversion%_!
move %base% %full% >nul:
echo Create %full%.zip
if exist %full%.zip del %full%.zip
%zip% %full%.zip %full% >nul: