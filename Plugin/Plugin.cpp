#include "depthai/depthai.hpp"

std::unique_ptr<dai::Device> _device;
std::shared_ptr<dai::DataOutputQueue> _imageQueue;
std::shared_ptr<dai::DataOutputQueue> _depthQueue;

extern "C" void PluginInitialize()
{
    dai::Pipeline pipeline;

    auto cam_l = pipeline.create<dai::node::MonoCamera>();
    auto cam_r = pipeline.create<dai::node::MonoCamera>();
    auto depth = pipeline.create<dai::node::StereoDepth>();
    auto xout_image = pipeline.create<dai::node::XLinkOut>();
    auto xout_depth = pipeline.create<dai::node::XLinkOut>();

    xout_image->setStreamName("image");
    xout_depth->setStreamName("depth");

    cam_l->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);
    cam_r->setResolution(dai::MonoCameraProperties::SensorResolution::THE_400_P);

    cam_l->setBoardSocket(dai::CameraBoardSocket::LEFT);
    cam_r->setBoardSocket(dai::CameraBoardSocket::RIGHT);

    depth->initialConfig.setConfidenceThreshold(245);
    depth->setLeftRightCheck(true);
    depth->setSubpixel(true);

    cam_l->out.link(depth->left);
    cam_r->out.link(depth->right);

    depth->rectifiedRight.link(xout_image->input);
    depth->depth.link(xout_depth->input);

    _device.reset(new dai::Device(pipeline));

    _imageQueue = _device->getOutputQueue("image", 1, false);
    _depthQueue = _device->getOutputQueue("depth", 1, false);
}

struct FrameInfo
{
    unsigned int imageWidth, imageHeight;
    unsigned int depthWidth, depthHeight;
    void* imageData;
    void* depthData;
};

extern "C" int PluginTryGetFrame(FrameInfo* info)
{
    auto image = _imageQueue->get<dai::ImgFrame>();
    auto depth = _depthQueue->get<dai::ImgFrame>();

    info->imageWidth  = image->getWidth();
    info->imageHeight = image->getHeight();
    info->imageData   = image->getData().data();

    info->depthWidth  = depth->getWidth();
    info->depthHeight = depth->getHeight();
    info->depthData   = depth->getData().data();

    return 1;
}

extern "C" void PluginFinalize()
{
    _imageQueue.reset();
    _depthQueue.reset();
    _device.reset();
}
