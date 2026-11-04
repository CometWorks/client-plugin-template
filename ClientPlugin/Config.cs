using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using ClientPlugin.Settings;
using ClientPlugin.Settings.Elements;
using ClientPlugin.Settings.Tools;
using Sandbox.Graphics.GUI;
using VRage.Input;
using VRageMath;

namespace ClientPlugin;

public enum ExampleEnum
{
    FirstAlpha,
    SecondBeta,
    ThirdGamma,
    AndTheDelta,
    Epsilon,
}

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Config : INotifyPropertyChanged
{
    #region User interface

    // TODO: Settings dialog title
    // ReSharper disable once ConvertToConstant.Global
    public readonly string Title = "Config Demo";

    [Separator("Some settings")]
    // TODO: Settings dialog controls, one property for each configuration option

    [Checkbox(description: "Checkbox Tooltip")]
    public bool Toggle
    {
        get;
        set => SetField(ref field, value);
    } = true;

    [Slider(
        -1f,
        10f,
        1f,
        SliderAttribute.SliderType.Integer,
        description: "Integer Slider Tooltip, enabled by the checkbox",
        enabledBy: nameof(Toggle)
    )]
    public int Integer
    {
        get;
        set => SetField(ref field, value);
    } = 2;

    [Slider(
        -5f,
        4.5f,
        0.5f,
        SliderAttribute.SliderType.Float,
        description: "Float Slider Tooltip",
        unit: "m/s"
    )]
    public float Number
    {
        get;
        set => SetField(ref field, value);
    } = 0.1f;

    [Textbox(description: "Textbox Tooltip")]
    public string Text
    {
        get;
        set => SetField(ref field, value);
    } = "Default Text";

    [Dropdown(description: "Dropdown Tooltip")]
    public ExampleEnum Dropdown
    {
        get;
        set => SetField(ref field, value);
    } = ExampleEnum.FirstAlpha;

    [Separator("More settings")]
    [Color(description: "RGB color")]
    public Color Color
    {
        get;
        set => SetField(ref field, value);
    } = Color.Cyan;

    [Color(hasAlpha: true, description: "RGBA color")]
    public Color ColorWithAlpha
    {
        get;
        set => SetField(ref field, value);
    } = new(0.8f, 0.6f, 0.2f, 0.5f);

    [Keybind(description: "Keybind Tooltip - Unbind by right clicking the button")]
    public Binding Keybind
    {
        get;
        set => SetField(ref field, value);
    } = new(MyKeys.None);

    [Button(description: "Button Tooltip")]
    public void Button()
    {
        MyGuiSandbox.AddScreen(
            MyGuiSandbox.CreateMessageBox(
                MyMessageBoxStyleEnum.Info,
                buttonType: MyMessageBoxButtonsType.OK,
                messageText: new StringBuilder("You clicked me!"),
                messageCaption: new StringBuilder("Custom Button Function"),
                size: new Vector2(0.6f, 0.5f)
            )
        );
    }

    #endregion

    #region Property change notification boilerplate

    public static readonly Config Default = new();
    public static readonly Config Current = ConfigStorage.Load();

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}
