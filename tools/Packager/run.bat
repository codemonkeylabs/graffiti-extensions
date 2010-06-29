@echo off
rd /S /Q root
del /Q *.xml
md root\bin
copy ..\..\bin\CodeMonkeyLabs.Graffiti.dll root\bin
call package.exe "Code Monkey Labs Graffiti Extensions" "root" "/"
pause