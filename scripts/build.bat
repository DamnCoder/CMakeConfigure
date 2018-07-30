@echo off

set bat_dir=%~dp0
set directory=%bat_dir%..\Build
set executable_name=%bat_dir%..\Build\CMakeConfigure.exe
set src=%bat_dir%..\CMakeConfigure\Src\*.cs

echo We need the folder: %directory%

if not exist %directory% (
	echo %directory% does not exists so we create it
	mkdir %directory%
)

C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /out:%executable_name% %src%

::exit