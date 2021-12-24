using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public sealed class Test : MonoBehaviour
{
    Texture2D _texture;

    void Start()
      => PluginInitialize();

    void OnDestroy()
    {
        PluginFinalize();
        if (_texture != null) Destroy(_texture);
    }

    unsafe void Update()
    {
        var info = new FrameInfo();
        PluginTryGetFrame(out info);

        if (_texture == null)
        {
            _texture = new Texture2D(info.width, info.height, TextureFormat.R16, false)
            { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            GetComponent<MeshRenderer>().material.mainTexture = _texture;
        }

        _texture.LoadRawTextureData(info.data, info.width * info.height * 2);
        _texture.Apply();
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameInfo
    {
        public int width, height;
        public System.IntPtr data;
    }

    [DllImport("libDepthAITest")]
    static extern void PluginInitialize();

    [DllImport("libDepthAITest")]
    static extern int PluginTryGetFrame(out FrameInfo info);

    [DllImport("libDepthAITest")]
    static extern void PluginFinalize();
}
