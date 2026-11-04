using Sandbox.Graphics.GUI;
using VRage.Utils;
using VRageMath;

namespace ClientPlugin.Settings.Elements;

internal class Control(
    MyGuiControlBase guiControl,
    float? fixedWidth = null,
    float minWidth = 0f,
    float? fillFactor = null,
    MyGuiDrawAlignEnum originAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
    Vector2? offset = null,
    float rightMargin = 0f)
{
    // FIXME: This is global and not determined automatically
    public const float LabelMinWidth = 0.18f;

    public readonly MyGuiControlBase GuiControl = guiControl;
    public readonly float? FixedWidth = fixedWidth;
    public readonly float MinWidth = minWidth;
    public readonly float? FillFactor = fillFactor;
    public readonly MyGuiDrawAlignEnum OriginAlign = originAlign;
    public readonly Vector2 Offset = offset ?? Vector2.Zero;
    public readonly float RightMargin = rightMargin;
}
