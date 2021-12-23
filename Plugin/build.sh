#!/bin/sh
cmake -D'depthai_DIR=../../depthai-core/build' -S. -Bbuild && cmake --build build
cp build/libDepthAITest.dylib ../Assets/libDepthAITest.bundle
