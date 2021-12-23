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

    void Update()
    {
        var info = new FrameInfo();
        PluginTryGetFrame(out info);

        if (_texture == null)
        {
            _texture = new Texture2D(info.width, info.height);
            GetComponent<MeshRenderer>().material.mainTexture = _texture;
        }

        unsafe
        {
            var ptr = (void*)info.data;

        var offs = 0;
        for (var y = 0; y < info.height; y++)
        {
            for (var x = 0; x < info.width; x++)
            {
                var b = UnsafeUtility.ReadArrayElement<byte>(ptr, offs++);
                _texture.SetPixel(x, info.height - 1 - y, Color.white * (b / info.maxValue));
            }
        }
        }

        _texture.Apply();
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameInfo
    {
        public System.IntPtr data;
        public int width, height;
        public float maxValue;
        public float padding;
    }

    [DllImport("libDepthAITest")]
    static extern void PluginInitialize();

    [DllImport("libDepthAITest")]
    static extern int PluginTryGetFrame(out FrameInfo info);

    [DllImport("libDepthAITest")]
    static extern void PluginFinalize();
}
