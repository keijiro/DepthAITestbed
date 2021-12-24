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
            _texture = new Texture2D(info.width, info.height);
            GetComponent<MeshRenderer>().material.mainTexture = _texture;
        }

        var ptr = (void*)info.data;
        var offs = 0;

        for (var y = 0; y < info.height; y++)
        {
            for (var x = 0; x < info.width; x++)
            {
                var b = UnsafeUtility.ReadArrayElement<ushort>(ptr, offs++);
                _texture.SetPixel(x, info.height - 1 - y, Color.white * (b / 2000.0f));
            }
        }

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
