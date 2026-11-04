using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sandbox.Graphics.GUI;

namespace ClientPlugin.Settings.Elements;

internal class DropdownAttribute(
    int visibleRows = 20,
    string label = null,
    string description = null,
    string enabledBy = null)
    : Attribute, IElement
{
    public readonly int VisibleRows = visibleRows;
    public readonly string Label = label;
    public readonly string Description = description;
    public string EnabledBy { get; } = enabledBy;

    private static string UnCamelCase(string str)
    {
        return Regex.Replace(
            Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }

    public List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    )
    {
        var selectedEnum = propertyGetter();
        var choiceEnum = selectedEnum.GetType();

        var dropdown = new MyGuiControlCombobox(toolTip: Description);
        var elements = Enum.GetNames(choiceEnum);

        for (var i = 0; i < elements.Length; i++)
        {
            dropdown.AddItem(i, UnCamelCase(elements[i]));
        }

        dropdown.ItemSelected += OnItemSelect;
        dropdown.SelectItemByIndex(Convert.ToInt32(selectedEnum));

        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return
        [
            new Control(new MyGuiControlLabel(text: label), minWidth: Control.LabelMinWidth),
            new Control(dropdown, fillFactor: 1f),
        ];

        void OnItemSelect()
        {
            var key = dropdown.GetSelectedKey();
            var value = elements[key];

            var enumValue = Enum.Parse(choiceEnum, value);
            propertySetter(enumValue);
        }
    }

    public List<Type> SupportedTypes { get; } = [typeof(Enum)];
}
