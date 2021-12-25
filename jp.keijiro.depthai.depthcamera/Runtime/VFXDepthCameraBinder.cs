using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace DepthAI {

[AddComponentMenu("VFX/Property Binders/DepthCamera Binder")]
[VFXBinder("DepthCamera")]
class VFXDepthCameraBinder : VFXBinderBase
{
    [SerializeField] DepthCameraDriver _driver = null;

    public string MonoMapProperty
      { get => (string)_monoMapProperty;
        set => _monoMapProperty = value; }

    public string DepthMapProperty
      { get => (string)_depthMapProperty;
        set => _depthMapProperty = value; }

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _monoMapProperty = "MonoMap";

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _depthMapProperty = "DepthMap";

    public override bool IsValid(VisualEffect component)
      => _driver != null &&
         component.HasTexture(_monoMapProperty) &&
         component.HasTexture(_depthMapProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        if (_driver.MonoTexture == null) return;

        component.SetTexture(_monoMapProperty, _driver.MonoTexture);
        component.SetTexture(_depthMapProperty, _driver.DepthTexture);
    }

    public override string ToString()
      => $"DepthAI : {_monoMapProperty}, {_depthMapProperty}";
}

} // namespace DepthAI
