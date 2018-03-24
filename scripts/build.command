#!/bin/sh

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

cd $DIR
cd ..

if [ ! -d ./Build ]; then
	mkdir Build
fi

mcs -out:./Build/CMakeConfigure.exe ./CMakeConfigure/Src/*.cs