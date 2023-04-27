using System.Drawing;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable PublicConstructorInAbstractClass

namespace CanvasDemo.Canvas;

/// <summary>
/// 对象元素
/// </summary>
public abstract class ObjElement<T> : ObjElement where T : IElementData
{
    /// <summary>
    /// 对象实体
    /// </summary>
    public T Data { get; set; }

    public ObjElement(Layer layer, T data) : base(layer, data.Id)
    {
        Data = data;
    }
}

/// <summary>
/// 元素
/// </summary>
public abstract class ObjElement : Element
{
    private static readonly Brush JoystickCurrent = new SolidBrush(Color.FromArgb(230, 255, 255, 255));
    private static readonly Brush JoystickSelect = new SolidBrush(Color.FromArgb(230, 50, 50, 50));

    /// <summary>
    /// 画布控件
    /// </summary>
    public readonly Layer Layer;

    /// <summary>
    /// 是否是选中 默认false
    /// </summary>
    public bool IsSelected { get; private set; }

    /// <summary>
    /// 是否是当前对象 默认false
    /// </summary>
    public bool IsCurrent { get; private set; }

    /// <summary>
    /// 八个操纵柄尺寸
    /// </summary>
    public int JoystickSize => (Rect.Width + Rect.Height) / 20 + 1;

    public ObjElement(Layer layer, string id) : base(layer.Canvas, id)
    {
        Layer = layer;
        Layer.Elements.Add(this);
    }

    /// <summary>
    /// 选择对象
    /// </summary>
    public void Selected()
    {
        IsSelected = true;
        SelectedEvent();
    }

    protected virtual void SelectedEvent()
    {
    }

    /// <summary>
    /// 清除对象选择
    /// </summary>
    public void UnSelected()
    {
        IsSelected = false;
        UnSelectedEvent();
    }

    protected virtual void UnSelectedEvent()
    {
    }

    public void Current()
    {
        IsCurrent = true;
        CurrentEvent();
    }

    protected virtual void CurrentEvent()
    {
    }

    public void UnCurrent()
    {
        IsCurrent = false;
        UnCurrentEvent();
    }

    protected virtual void UnCurrentEvent()
    {
    }


    /// <summary>
    /// 绘制八个操纵柄
    /// </summary>
    /// <param name="g">画笔</param>
    public void DrawingJoystick(Graphics g)
    {
        if (IsSelected == false) return;

        var cX = Rect.Width / 2;
        var cY = Rect.Height / 2;
        var s = JoystickSize;
        switch (IsCurrent)
        {
            case true:
                // 画实心矩形
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X, Rect.Y + (0), s, s));
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y + (0), s, s));
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (0), s, s));
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (cY - s / 2), s, s));
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (Rect.Height - s), s, s));
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y + (Rect.Height - s), s, s));
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X + (0), Rect.Y + (Rect.Height - s), s, s));
                g.FillRectangle(JoystickCurrent, Viewer.LocalToShow(Rect.X + (0), Rect.Y + (cY - s / 2), s, s));

                // 画边框矩形
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X, Rect.Y + (0), s, s));
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y + (0), s, s));
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (0), s, s));
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (cY - s / 2), s, s));
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (Rect.Height - s), s, s));
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y + (Rect.Height - s), s, s));
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X + (0), Rect.Y + (Rect.Height - s), s, s));
                g.DrawRectangle(Pens.Black, Viewer.LocalToShow(Rect.X + (0), Rect.Y + (cY - s / 2), s, s));
                break;

            case false:
                // 画实心矩形
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X, Rect.Y + (0), s, s));
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y + (0), s, s));
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (0), s, s));
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (cY - s / 2), s, s));
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (Rect.Height - s), s, s));
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y + (Rect.Height - s), s, s));
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X + (0), Rect.Y + (Rect.Height - s), s, s));
                g.FillRectangle(JoystickSelect, Viewer.LocalToShow(Rect.X + (0), Rect.Y + (cY - s / 2), s, s));
                break;
        }
    }
}