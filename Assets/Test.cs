using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public sealed class Test : MonoBehaviour
{
    Texture2D _colorMap, _depthMap;

    void Start()
      => PluginInitialize();

    void OnDestroy()
    {
        PluginFinalize();

        if (_colorMap != null) Destroy(_colorMap);
        if (_depthMap != null) Destroy(_depthMap);
    }

    unsafe void Update()
    {
        var info = new FrameInfo();
        PluginTryGetFrame(out info);

        if (_colorMap == null)
        {
            _colorMap = new Texture2D
              (info.imageWidth, info.imageHeight, TextureFormat.R8, false)
              { filterMode = FilterMode.Point };

            GetComponent<MeshRenderer>().material.
              SetTexture("_ColorMap", _colorMap);
        }

        if (_depthMap == null)
        {
            _depthMap = new Texture2D
              (info.depthWidth, info.depthHeight, TextureFormat.R16, false)
              { filterMode = FilterMode.Point };

            GetComponent<MeshRenderer>().material.
              SetTexture("_DepthMap", _depthMap);
        }

        _colorMap.LoadRawTextureData
          (info.imageData, info.imageWidth * info.imageHeight);
        _colorMap.Apply();

        _depthMap.LoadRawTextureData
          (info.depthData, info.depthWidth * info.depthHeight * 2); 
        _depthMap.Apply();
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameInfo
    {
        public int imageWidth, imageHeight;
        public int depthWidth, depthHeight;
        public System.IntPtr imageData;
        public System.IntPtr depthData;
    }

    [DllImport("libDepthAITest")]
    static extern void PluginInitialize();

    [DllImport("libDepthAITest")]
    static extern int PluginTryGetFrame(out FrameInfo info);

    [DllImport("libDepthAITest")]
    static extern void PluginFinalize();
}
