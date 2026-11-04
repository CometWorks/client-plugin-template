using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ClientPlugin.Settings.Tools;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using VRage;
using VRage.Game;
using VRage.Input;
using VRage.Utils;
using VRageMath;

namespace ClientPlugin.Settings.Elements;

internal class KeybindAttribute(string label = null, string description = null, string enabledBy = null)
    : Attribute, IElement
{
    public readonly string Label = label;
    public readonly string Description = description;
    public string EnabledBy { get; } = enabledBy;

    private Func<Binding> _propertyGetter;
    private Action<Binding> _propertySetter;

    public List<Control> GetControls(
        string name,
        Func<object> propertyGetter,
        Action<object> propertySetter
    )
    {
        _propertyGetter = () => (Binding)propertyGetter();
        _propertySetter = b => propertySetter(b);

        var binding = _propertyGetter();

        var label = new MyGuiControlLabel(text: Tools.Tools.GetLabelOrDefault(name, Label));

        var ctrl = new MyGuiControlCheckbox(isChecked: binding.Ctrl, toolTip: "Ctrl");
        var alt = new MyGuiControlCheckbox(isChecked: binding.Alt, toolTip: "Alt");
        var shift = new MyGuiControlCheckbox(isChecked: binding.Shift, toolTip: "Shift");

        ctrl.IsCheckedChanged += cb =>
        {
            var b = _propertyGetter();
            b.Ctrl = cb.IsChecked;
            _propertySetter(b);
        };

        alt.IsCheckedChanged += cb =>
        {
            var b = _propertyGetter();
            b.Alt = cb.IsChecked;
            _propertySetter(b);
        };

        shift.IsCheckedChanged += cb =>
        {
            var b = _propertyGetter();
            b.Shift = cb.IsChecked;
            _propertySetter(b);
        };

        var control = new MyControl(
            MyStringId.GetOrCompute($"{name.Replace(" ", "")}Keybind"),
            MyStringId.GetOrCompute(name),
            MyGuiControlTypeEnum.General,
            null,
            binding.Key
        );

        StringBuilder output = null;
        control.AppendBoundButtonNames(ref output, MyGuiInputDeviceEnum.Keyboard);
        MyControl.AppendUnknownTextIfNeeded(
            ref output,
            MyTexts.GetString(MyCommonTexts.UnknownControl_None)
        );

        var button = new MyGuiControlButton(
            text: output,
            onButtonClick: OnRebindClick,
            onSecondaryButtonClick: OnUnbindClick,
            toolTip: Description
        )
        {
            VisualStyle = MyGuiControlButtonStyleEnum.ControlSetting,
            UserData = new ControlButtonData(control, MyGuiInputDeviceEnum.Keyboard),
        };

        return
        [
            new Control(label, minWidth: Control.LabelMinWidth),
            new Control(button),
            new Control(ctrl, offset: new Vector2(0f, -0.0025f)),
            new Control(alt, offset: new Vector2(0f, -0.0025f)),
            new Control(shift, offset: new Vector2(0f, -0.0025f)),
        ];
    }

    public List<Type> SupportedTypes { get; } = [typeof(Binding)];

    private class ControlButtonData(MyControl control, MyGuiInputDeviceEnum device)
    {
        public readonly MyControl Control = control;
        public readonly MyGuiInputDeviceEnum Device = device;
    }

    private void OnRebindClick(MyGuiControlButton button)
    {
        var userData = (ControlButtonData)button.UserData;
        var messageText = MyCommonTexts.AssignControlKeyboard;
        if (userData.Device == MyGuiInputDeviceEnum.Mouse)
            messageText = MyCommonTexts.AssignControlMouse;

        // KEEN!!! MyGuiScreenOptionsMouseKeyboard.MyGuiControlAssignKeyMessageBox is PRIVATE!
        var screenClass = typeof(MyGuiScreenOptionsMouseKeyboard).GetNestedType(
            "MyGuiControlAssignKeyMessageBox",
            BindingFlags.NonPublic
        );

        var editBindingDialog = (MyGuiScreenBase)
            Activator.CreateInstance(
                screenClass,
                BindingFlags.CreateInstance,
                null,
                [userData.Device, userData.Control, messageText],
                null
            );

        editBindingDialog.Closed += (_, _) => StoreControl(button);
        MyGuiSandbox.AddScreen(editBindingDialog);
    }

    private void OnUnbindClick(MyGuiControlButton button)
    {
        MyGuiScreenBase alert = MyGuiSandbox.CreateMessageBox(
            MyMessageBoxStyleEnum.Info,
            buttonType: MyMessageBoxButtonsType.YES_NO,
            messageText: new StringBuilder("Are you sure?"),
            messageCaption: new StringBuilder("UNBIND CONTROL"),
            yesButtonText: MyStringId.GetOrCompute("Confirm"),
            noButtonText: MyStringId.GetOrCompute("Cancel"),
            callback: Callback
        );

        MyGuiSandbox.AddScreen(alert);
        return;

        void Callback(MyGuiScreenMessageBox.ResultEnum result)
        {
            if (result == MyGuiScreenMessageBox.ResultEnum.NO)
                return;

            var userData = (ControlButtonData)button.UserData;
            userData.Control.SetControl(userData.Device, MyKeys.None);

            StoreControl(button);
        }
    }

    private void StoreControl(MyGuiControlButton button)
    {
        StringBuilder output = null;
        var userData = (ControlButtonData)button.UserData;
        userData.Control.AppendBoundButtonNames(ref output, userData.Device);

        var binding = _propertyGetter();
        binding.Key = userData.Control.GetKeyboardControl();
        _propertySetter(binding);

        MyControl.AppendUnknownTextIfNeeded(
            ref output,
            MyTexts.GetString(MyCommonTexts.UnknownControl_None)
        );
        button.Text = output.ToString();
        output.Clear();
    }
}
