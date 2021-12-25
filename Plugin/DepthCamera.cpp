#include "depthai/depthai.hpp"
#include <cstdio>

struct FrameInfo
{
    unsigned int width, height;
    void* monoData;
    void* depthData;
};

std::unique_ptr<dai::Device> _device;
std::shared_ptr<dai::DataOutputQueue> _monoQueue;
std::shared_ptr<dai::DataOutputQueue> _depthQueue;

extern "C" void DepthCamera_Initialize()
{
    dai::Pipeline pipeline;

    auto monoLeft = pipeline.create<dai::node::MonoCamera>();
    auto monoRight = pipeline.create<dai::node::MonoCamera>();
    auto depth = pipeline.create<dai::node::StereoDepth>();
    auto monoOut = pipeline.create<dai::node::XLinkOut>();
    auto depthOut = pipeline.create<dai::node::XLinkOut>();

    monoOut->setStreamName("mono");
    depthOut->setStreamName("depth");

    monoLeft->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);
    monoRight->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);

    monoLeft->setBoardSocket(dai::CameraBoardSocket::LEFT);
    monoRight->setBoardSocket(dai::CameraBoardSocket::RIGHT);

    depth->initialConfig.setConfidenceThreshold(245);
    depth->setLeftRightCheck(true);
    depth->setSubpixel(true);

    monoLeft->out.link(depth->left);
    monoRight->out.link(depth->right);
    depth->rectifiedRight.link(monoOut->input);
    depth->depth.link(depthOut->input);

    _device.reset(new dai::Device(pipeline));

    _monoQueue = _device->getOutputQueue("mono", 1, false);
    _depthQueue = _device->getOutputQueue("depth", 1, false);
}

extern "C" int DepthCamera_TryGetFrame(FrameInfo* info)
{
    auto mono = _monoQueue->get<dai::ImgFrame>();
    auto depth = _depthQueue->get<dai::ImgFrame>();

    info->width = mono->getWidth();
    info->height = mono->getHeight();
    info->monoData = mono->getData().data();
    info->depthData = depth->getData().data();

    return 1;
}

extern "C" void DepthCamera_Finalize()
{
    _monoQueue.reset();
    _depthQueue.reset();
    _device.reset();
}
