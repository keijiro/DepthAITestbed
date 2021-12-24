#include "depthai/depthai.hpp"

std::unique_ptr<dai::Device> _device;
std::shared_ptr<dai::DataOutputQueue> _queue;

extern "C" void PluginInitialize()
{
    dai::Pipeline pipeline;

    auto cam_l = pipeline.create<dai::node::MonoCamera>();
    auto cam_r = pipeline.create<dai::node::MonoCamera>();
    auto depth = pipeline.create<dai::node::StereoDepth>();
    auto xout  = pipeline.create<dai::node::XLinkOut>();

    xout->setStreamName("depth");

    cam_l->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);
    cam_r->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);

    cam_l->setBoardSocket(dai::CameraBoardSocket::LEFT);
    cam_r->setBoardSocket(dai::CameraBoardSocket::RIGHT);

    depth->initialConfig.setConfidenceThreshold(245);
    depth->setLeftRightCheck(true);

    cam_l->out.link(depth->left);
    cam_r->out.link(depth->right);

    depth->depth.link(xout->input);

    _device.reset(new dai::Device(pipeline));

    _queue = _device->getOutputQueue("depth", 1, false);
}

struct FrameInfo
{
    unsigned int width;
    unsigned int height;
    void* data;
};

extern "C" int PluginTryGetFrame(FrameInfo* info)
{
    auto frame = _queue->get<dai::ImgFrame>();
    info->width = frame->getWidth();
    info->height = frame->getHeight();
    info->data = frame->getData().data();
    return 1;
}

extern "C" void PluginFinalize()
{
    _queue.reset();
    _device.reset();
}
