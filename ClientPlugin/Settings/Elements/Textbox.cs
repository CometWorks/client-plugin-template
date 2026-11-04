using System;
using System.Collections.Generic;
using Sandbox.Graphics.GUI;

namespace ClientPlugin.Settings.Elements;

internal class TextboxAttribute(string label = null, string description = null, string enabledBy = null)
    : Attribute, IElement
{
    public readonly string Label = label;
    public readonly string Description = description;
    public string EnabledBy { get; } = enabledBy;

    public List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    )
    {
        var textBox = new MyGuiControlTextbox(defaultText: (string)propertyGetter());
        textBox.TextChanged += box => propertySetter(box.Text);
        textBox.SetToolTip(Description);

        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return
        [
            new Control(new MyGuiControlLabel(text: label), minWidth: Control.LabelMinWidth),
            new Control(textBox, fillFactor: 1f),
        ];
    }

    public List<Type> SupportedTypes { get; } = [typeof(string)];
}
