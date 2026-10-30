using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.Graphics.GUI;
using VRage.Utils;

namespace ClientPlugin.Settings.Elements;

internal class SliderAttribute : Attribute, IElement
{
    public enum SliderType
    {
        Integer,
        Float,
    }

    public readonly float Min;
    public readonly float Max;
    public readonly float Step;
    public readonly SliderType Type;
    public readonly string Label;
    public readonly string Description;
    public string EnabledBy { get; }

    // Shown after the value, for example "ms"
    public readonly string Unit;

    // The dialog's CanHideOthers setter is protected, so a subclass sets it to
    // hide the settings screen behind the dialog.
    private sealed class AmountDialog : MyGuiScreenDialogAmount
    {
        public AmountDialog(SliderAttribute slider, float amount)
            : base(
                slider.Min,
                slider.Max,
                MyCommonTexts.DialogAmount_SetValueCaption,
                defaultAmount: amount,
                parseAsInteger: slider.Type == SliderType.Integer,
                backgroundTransition: MySandboxGame.Config.UIBkOpacity,
                guiTransition: MySandboxGame.Config.UIOpacity,
                incrementStep: slider.Step
            )
        {
            CanHideOthers = true;
        }
    }

    public SliderAttribute(
        float min,
        float max,
        float step = 1f,
        SliderType type = SliderType.Float,
        string label = null,
        string description = null,
        string enabledBy = null,
        string unit = null
    )
    {
        Min = min;
        Max = max;
        Step = step;
        Type = type;
        Label = label;
        Description = description;
        EnabledBy = enabledBy;
        Unit = unit;
    }

    public List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    )
    {
        var valueLabel = new MyGuiControlLabel();

        string FormatValue(float value, int decimalPlaces)
        {
            var text =
                Type == SliderType.Integer
                    ? Convert.ToInt32(value).ToString()
                    : MyValueFormatter.GetFormatedFloat(value, decimalPlaces);
            return Unit == null ? text : text + " " + Unit;
        }

        void ValueUpdate(MyGuiControlSlider element)
        {
            if (Type == SliderType.Integer)
                propertySetter(Convert.ToInt32(element.Value));
            else
                propertySetter(element.Value);

            valueLabel.Text = FormatValue(element.Value, element.LabelDecimalPlaces);
        }

        bool SpecifyValue(MyGuiControlSlider element)
        {
            var screen = new AmountDialog(this, Convert.ToSingle(propertyGetter()));
            screen.OnConfirmed += (value) => element.Value = value;
            MyGuiSandbox.AddScreen(screen);
            return true;
        }

        var slider = new MyGuiControlSlider(
            toolTip: Description,
            defaultValue: Convert.ToSingle(propertyGetter()),
            minValue: Min,
            maxValue: Max,
            intValue: Type == SliderType.Integer
        )
        {
            MinimumStepOverride = Step,
        };

        if (Type == SliderType.Float)
        {
            slider.LabelDecimalPlaces = (int)Math.Max(1, Math.Ceiling(-Math.Log10(2f * Step)));
        }

        slider.ValueChanged += ValueUpdate;
        slider.SliderSetValueManual = SpecifyValue;

        // Reserve the width of the widest value text, so the slider leaves room for the value and
        // its unit wherever it is dragged. The label measures its text when the text is set.
        var valueWidth = 0f;
        foreach (var value in new[] { Min, Max })
        {
            valueLabel.Text = FormatValue(value, slider.LabelDecimalPlaces);
            valueWidth = Math.Max(valueWidth, valueLabel.Size.X);
        }

        ValueUpdate(slider);

        var label = Tools.Tools.GetLabelOrDefault(name, Label);
        return new List<Control>()
        {
            new Control(new MyGuiControlLabel(text: label), minWidth: Control.LabelMinWidth),
            new Control(slider, fillFactor: 1f, rightMargin: 0.005f),
            new Control(valueLabel, minWidth: valueWidth),
        };
    }

    public List<Type> SupportedTypes { get; } = new List<Type>() { typeof(float), typeof(int) };
}
