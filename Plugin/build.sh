#!/bin/sh

TARGET="DepthCamera"
PACKAGE_DIR="../jp.keijiro.depthai.depthcamera/Runtime"

cmake "-Ddepthai_DIR=../../depthai-core/build" "-DCMAKE_OSX_ARCHITECTURES=arm64;x86_64" -S. -Bbuild
cmake --build build

rm ${PACKAGE_DIR}/*.bundle*
cp build/lib${TARGET}.dylib ${PACKAGE_DIR}/${TARGET}.bundle
