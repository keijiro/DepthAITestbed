cmake_minimum_required(VERSION 3.4)

project(DepthAITest)

find_package(depthai CONFIG REQUIRED)

add_library(${PROJECT_NAME} SHARED Plugin.cpp)

target_link_libraries(${PROJECT_NAME} PRIVATE depthai::core)
