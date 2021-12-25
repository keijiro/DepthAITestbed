using System.Runtime.InteropServices;
using UnityEngine;

namespace DepthAI {

public sealed class DepthCameraDriver : MonoBehaviour
{
    public Texture2D MonoTexture { get; private set; }
    public Texture2D DepthTexture { get; private set; }

    void Start()
      => DepthCamera_Initialize();

    void OnDestroy()
    {
        DepthCamera_Finalize();

        if (MonoTexture != null) Destroy(MonoTexture);
        if (DepthTexture != null) Destroy(DepthTexture);
    }

    void Update()
    {
        var info = new FrameInfo();
        DepthCamera_TryGetFrame(out info);

        if (MonoTexture == null)
            MonoTexture = new Texture2D
              (info.width, info.height, TextureFormat.R8, false)
              { filterMode = FilterMode.Point };

        if (DepthTexture == null)
            DepthTexture = new Texture2D
              (info.width, info.height, TextureFormat.R16, false)
              { filterMode = FilterMode.Point };

        MonoTexture.LoadRawTextureData(info.monoData, info.width * info.height);
        MonoTexture.Apply();

        DepthTexture.LoadRawTextureData(info.depthData, info.width * info.height * 2); 
        DepthTexture.Apply();
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameInfo
    {
        public int width, height;
        public System.IntPtr monoData;
        public System.IntPtr depthData;
    }

    [DllImport("DepthCamera")]
    static extern void DepthCamera_Initialize();

    [DllImport("DepthCamera")]
    static extern int DepthCamera_TryGetFrame(out FrameInfo info);

    [DllImport("DepthCamera")]
    static extern void DepthCamera_Finalize();
}

} // namespace DepthAI
