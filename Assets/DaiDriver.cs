using System.Runtime.InteropServices;
using UnityEngine;

public sealed class DaiDriver : MonoBehaviour
{
    public Texture2D ColorTexture { get; private set; }
    public Texture2D DepthTexture { get; private set; }

    void Start()
      => PluginInitialize();

    void OnDestroy()
    {
        PluginFinalize();

        if (ColorTexture != null) Destroy(ColorTexture);
        if (DepthTexture != null) Destroy(DepthTexture);
    }

    void Update()
    {
        var info = new FrameInfo();
        PluginTryGetFrame(out info);

        if (ColorTexture == null)
            ColorTexture = new Texture2D
              (info.colorWidth, info.colorHeight, TextureFormat.R8, false)
              { filterMode = FilterMode.Point };

        if (DepthTexture == null)
            DepthTexture = new Texture2D
              (info.depthWidth, info.depthHeight, TextureFormat.R16, false)
              { filterMode = FilterMode.Point };

        ColorTexture.LoadRawTextureData
          (info.colorData, info.colorWidth * info.colorHeight);
        ColorTexture.Apply();

        DepthTexture.LoadRawTextureData
          (info.depthData, info.depthWidth * info.depthHeight * 2); 
        DepthTexture.Apply();
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameInfo
    {
        public int colorWidth, colorHeight;
        public int depthWidth, depthHeight;
        public System.IntPtr colorData;
        public System.IntPtr depthData;
    }

    [DllImport("libDepthAITest")]
    static extern void PluginInitialize();

    [DllImport("libDepthAITest")]
    static extern int PluginTryGetFrame(out FrameInfo info);

    [DllImport("libDepthAITest")]
    static extern void PluginFinalize();
}
