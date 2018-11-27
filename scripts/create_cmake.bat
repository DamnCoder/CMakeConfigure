@echo off

cd..

set BUILD_SCRIPT=.\scripts\build.bat
set EXECUTABLE=.\Build\CMakeConfigure.exe

set FILE_OUTPUT_PATH=.\Test\project
set TEMPLATES_PATH=.\Templates

set PROJECT_NAME=Test
set PROJECT_TYPE=EXECUTABLE
set PROJECT_ENVIRONMENT=Debug
set PROJECT_EXTERNALS=project_a project_b

echo TEST FOR CMAKECONFIGURE

echo Build

call %BUILD_SCRIPT%

echo Create the CMakeFile on %FILE_OUTPUT_PATH% directory

%EXECUTABLE% %TEMPLATES_PATH% %FILE_OUTPUT_PATH% %PROJECT_TYPE% %PROJECT_NAME% %PROJECT_ENVIRONMENT% %PROJECT_EXTERNALS%

exit