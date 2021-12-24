using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[AddComponentMenu("VFX/Property Binders/DepthAI Binder")]
[VFXBinder("DepthAI")]
class VFXDaiBinder : VFXBinderBase
{
    [SerializeField] DaiDriver _driver = null;

    public string ColorMapProperty
      { get => (string)_colorMapProperty;
        set => _colorMapProperty = value; }

    public string DepthMapProperty
      { get => (string)_depthMapProperty;
        set => _depthMapProperty = value; }

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _colorMapProperty = "ColorMap";

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _depthMapProperty = "DepthMap";

    public override bool IsValid(VisualEffect component)
      => _driver != null &&
         component.HasTexture(_colorMapProperty) &&
         component.HasTexture(_depthMapProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        if (_driver.ColorTexture == null) return;

        component.SetTexture(_colorMapProperty, _driver.ColorTexture);
        component.SetTexture(_depthMapProperty, _driver.DepthTexture);
    }

    public override string ToString()
      => $"DepthAI : {_colorMapProperty}, {_depthMapProperty}";
}
