cmake_minimum_required(VERSION 3.4)

project(DepthCamera)

find_package(depthai CONFIG REQUIRED)

add_library(${PROJECT_NAME} SHARED DepthCamera.cpp)

target_link_libraries(${PROJECT_NAME} PRIVATE depthai::core)
