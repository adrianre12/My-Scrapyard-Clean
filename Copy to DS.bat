@echo off
set REPLACE_IN_PATH=%APPDATA%\SpaceEngineersDedicated\content\244850\3214345329

rmdir "%REPLACE_IN_PATH%\Data" /S /Q

robocopy.exe .\ "%REPLACE_IN_PATH%" *.* /S /xd .git bin obj .vs ignored /xf *.exe *.dll *.lnk *.git* *.bat *.zip *.7z *.blend* *.png *.md *.log *.sln *.csproj *.csproj.user *.ruleset desktop.ini *.fbx *.hkt *.xml *.txt

timeout 2