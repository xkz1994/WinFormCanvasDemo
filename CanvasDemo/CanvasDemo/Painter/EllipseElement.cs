using CanvasDemo.Canvas;
using CanvasDemo.Data;
using System.Drawing;

namespace CanvasDemo.Painter;

/// <summary>
/// 园形元素
/// </summary>
public class EllipseElement : ObjElement<ElementData>
{
    public static readonly Brush FillBrush = new SolidBrush(Color.Green);

    public EllipseElement(EllipseLayer layer, ElementData data, int sideLength) : base(layer, data)
    {
        Rect.Width = sideLength;
        Rect.Height = sideLength;
    }

    public override void Drawing(Graphics g)
    {
        // 画实心圆
        g.FillEllipse(FillBrush, Viewer.LocalToShow(Rect));
    }

    public override void DrawingAfter(Graphics g)
    {
    }
}