using CanvasDemo.Canvas;
using CanvasDemo.Data;
using System.Drawing;

namespace CanvasDemo.Painter;

public class EllipseElement : ObjElement<ElementData>
{
    public EllipseElement(EllipseLayer layer, ElementData data, int sideLength) : base(layer, data)
    {
        this.Rect.Width = sideLength;
        this.Rect.Height = sideLength;
    }

    public static Brush FillBrush = new SolidBrush(Color.Green);

    public override void Drawing(Graphics g)
    {
        g.FillEllipse(FillBrush, Viewer.LocalToShow(Rect));
    }

    public override void DrawingAfter(Graphics g)
    {
    }
}