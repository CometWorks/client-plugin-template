using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.Graphics.GUI;
using VRage.Utils;

namespace ClientPlugin.Settings.Elements;

internal class ButtonAttribute(string label = null, string description = null, string enabledBy = null)
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
        var button = new MyGuiControlButton(text: new StringBuilder(label), toolTip: Description);
        button.ButtonClicked += _ => ((Action)propertyGetter())();

        return
        [
            new Control(new MyGuiControlLabel(text: ""), fixedWidth: Control.LabelMinWidth),
            new Control(button, originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP),
        ];
    }

    public List<Type> SupportedTypes { get; } = [typeof(Delegate)];
}
