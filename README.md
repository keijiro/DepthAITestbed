DepthAITestbed
--------------

![96a121a9b0ca2b35b00a761370d3f971db384766](https://user-images.githubusercontent.com/343936/147397155-7a6f984a-4c58-4578-8e2e-ea1fb49d2872.gif)

This is a proof-of-concept project trying to implement a non-OpenCV dependent
DepthAI plugin for Unity.

At the moment, I've just implemented an interface for stereo depth sensing
(only on macOS with Apple silicon). I confirmed that I can implement the plugin
without OpenCV dependency. That's not an easy task due to C++ dependency,
though...
