DepthAITestbed
--------------

This is a proof-of-concept project trying to implement a non-OpenCV dependent
DepthAI plugin for Unity.

At the moment, I've just implemented an interface for stereo depth sensing
(only on macOS with Apple silicon). I confirmed that I can implement the plugin
without OpenCV dependency. That's not an easy task due to C++ dependency,
though...
