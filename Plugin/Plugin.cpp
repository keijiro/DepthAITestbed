#include "depthai/depthai.hpp"

std::unique_ptr<dai::Device> _device;
std::shared_ptr<dai::DataOutputQueue> _queue;
float _maxDisparity;

extern "C" void PluginInitialize()
{
    dai::Pipeline pipeline;

    // Define sources and outputs
    auto monoLeft = pipeline.create<dai::node::MonoCamera>();
    auto monoRight = pipeline.create<dai::node::MonoCamera>();
    auto depth = pipeline.create<dai::node::StereoDepth>();
    auto xout = pipeline.create<dai::node::XLinkOut>();

    xout->setStreamName("disparity");

    // Properties
    monoLeft->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);
    monoLeft->setBoardSocket(dai::CameraBoardSocket::LEFT);
    monoRight->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);
    monoRight->setBoardSocket(dai::CameraBoardSocket::RIGHT);

    depth->initialConfig.setConfidenceThreshold(245);
    depth->initialConfig.setMedianFilter(dai::MedianFilter::KERNEL_7x7);

    _maxDisparity = depth->initialConfig.getMaxDisparity();

    // Linking
    monoLeft->out.link(depth->left);
    monoRight->out.link(depth->right);
    depth->disparity.link(xout->input);

    // Connect to device and start pipeline
    _device.reset(new dai::Device(pipeline));
    _queue = _device->getOutputQueue("disparity", 1, false);
}

struct FrameInfo
{
    void* data;
    unsigned int width;
    unsigned int height;
    float maxValue;
    float padding;
};

extern "C" int PluginTryGetFrame(FrameInfo* info)
{
    auto frame = _queue->get<dai::ImgFrame>();
    info->width = frame->getWidth();
    info->height = frame->getHeight();
    info->maxValue = _maxDisparity;
    info->data = frame->getData().data();
    return 1;
}

extern "C" void PluginFinalize()
{
    _queue.reset();
    _device.reset();
}
