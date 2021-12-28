#!/bin/sh

TARGET="DepthCamera"
PACKAGE_DIR="../jp.keijiro.depthai.depthcamera/Runtime"

cmake -D'depthai_DIR=../../depthai-core/build' -S. -Bbuild
cmake --build build --parallel

rm ${PACKAGE_DIR}/*.dll
cp build/Debug/${TARGET}.dll ${PACKAGE_DIR}/${TARGET}.dll

cmd /k
