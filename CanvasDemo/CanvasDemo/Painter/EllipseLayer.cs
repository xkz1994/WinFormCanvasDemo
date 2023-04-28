using System.Drawing;
using System.Linq;
using CanvasDemo.Canvas;

namespace CanvasDemo.Painter;

public class EllipseLayer : Layer
{
    public EllipseLayer(TimCanvas canvas) : base(canvas, "Cube")
    {
        IsInteractionLayer = true;
    }

    public override void Drawing(Graphics g)
    {
        foreach (var item in Elements.Where(item => Canvas.Viewer.InZone(item) != false))
        {
            item.Drawing(g);
        }
    }

    public override void DrawingAfter(Graphics g)
    {
    }
}