using System;
using System.Collections.Generic;
using Sandbox.Graphics.GUI;
using VRageMath;

namespace ClientPlugin.Settings.Elements;

internal class SeparatorAttribute(string caption = null) : Attribute, IElement
{
    public readonly string Caption = caption;
    public string EnabledBy => null;

    public List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    )
    {
        var label = new MyGuiControlLabel(text: Caption ?? "") { ColorMask = Color.Orange };

        var lineColor = Color.LightCyan;
        lineColor.A = 0x22;

        var line = new MyGuiControlLabel
        {
            Size = new Vector2(0.5f, 0f),
            BorderEnabled = true,
            BorderSize = 1,
            BorderColor = lineColor,
        };

        return
        [
            new Control(label, rightMargin: 0.005f),
            new Control(line, fillFactor: 1f, offset: new Vector2(0f, 0.003f)),
        ];
    }

    public List<Type> SupportedTypes { get; } = [typeof(object)];
}
