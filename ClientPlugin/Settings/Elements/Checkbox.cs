using System;
using System.Collections.Generic;
using Sandbox.Graphics.GUI;

namespace ClientPlugin.Settings.Elements;

class CheckboxAttribute : Attribute, IElement
{
    public readonly string Label;
    public readonly string Description;
    public string EnabledBy { get; }

    public CheckboxAttribute(
        string label = null,
        string description = null,
        string enabledBy = null
    )
    {
        Label = label;
        Description = description;
        EnabledBy = enabledBy;
    }

    public List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    )
    {
        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return new List<Control>()
        {
            new Control(new MyGuiControlLabel(text: label), minWidth: Control.LabelMinWidth),
            new Control(
                new MyGuiControlCheckbox(toolTip: Description)
                {
                    IsChecked = (bool)propertyGetter(),
                    IsCheckedChanged = (x) => propertySetter(x.IsChecked),
                }
            ),
        };
    }

    public List<Type> SupportedTypes { get; } = new List<Type>() { typeof(bool) };
}
