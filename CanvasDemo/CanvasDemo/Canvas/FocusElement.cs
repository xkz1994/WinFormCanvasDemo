using System.Drawing;

namespace CanvasDemo.Canvas;

/// <summary>
/// 绘制十字焦点元素
/// 定位绘制的元素
/// </summary>
public class FocusElement : Element
{
    /// <summary>
    /// 是否展示
    /// </summary>
    public bool IsShow { get; set; }

    /// <summary>
    /// 焦点坐标
    /// </summary>
    public Point Focus { get; private set; }

    public void SetFocus(Point focus)
    {
        Focus = focus;
    }

    public FocusElement(TimCanvas canvas) : base(canvas, nameof(FocusElement))
    {
    }

    public override void Drawing(Graphics g)
    {
        var focus = Canvas.Viewer.LocalToShow(Focus);

        g.DrawLine(Pens.Black, focus with { Y = 0 }, focus with { Y = Canvas.Height });
        g.DrawLine(Pens.Black, focus with { X = 0 }, focus with { X = Canvas.Width });
    }

    public override void DrawingAfter(Graphics g)
    {
    }
}