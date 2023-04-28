using System.Drawing;
using System.Linq;
using CanvasDemo.Canvas;

namespace CanvasDemo.Painter;

public class CubeLayer : Layer
{
    public CubeLayer(TimCanvas canvas) : base(canvas, "Cube")
    {
        IsInteractionLayer = true;
    }

    public override void Drawing(Graphics g)
    {
        foreach (var item in Elements.Where(item => Canvas.Viewer.InZone(item)))
        {
            item.Drawing(g);
        }
    }

    public override void DrawingAfter(Graphics g)
    {
    }
}