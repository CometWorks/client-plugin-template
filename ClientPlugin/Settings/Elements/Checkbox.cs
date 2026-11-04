using System;
using System.Collections.Generic;
using Sandbox.Graphics.GUI;

namespace ClientPlugin.Settings.Elements;

internal class CheckboxAttribute(
    string label = null,
    string description = null,
    string enabledBy = null)
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
        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return
        [
            new Control(new MyGuiControlLabel(text: label), minWidth: Control.LabelMinWidth),
            new Control(
                new MyGuiControlCheckbox(toolTip: Description)
                {
                    IsChecked = (bool)propertyGetter(),
                    IsCheckedChanged = x => propertySetter(x.IsChecked),
                }
            ),
        ];
    }

    public List<Type> SupportedTypes { get; } = [typeof(bool)];
}
