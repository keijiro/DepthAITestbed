#!/bin/sh

TARGET="DepthCamera"
PACKAGE_DIR="../jp.keijiro.depthai.depthcamera/Runtime"

cmake -D'depthai_DIR=../../depthai-core/build' -S. -Bbuild
cmake --build build

rm ${PACKAGE_DIR}/*.bundle*
cp build/lib${TARGET}.dylib ${PACKAGE_DIR}/${TARGET}.bundle
