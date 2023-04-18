using System.Drawing;

namespace CanvasDemo.Canvas;

/// <summary>
/// 控制背景
/// </summary>
public class Backgrounder
{
    private readonly Pen _zeroLinePen = new Pen(new SolidBrush(Color.Black), 2);

    private readonly TimCanvas _canvas1;

    public Backgrounder(TimCanvas canvas)
    {
        _canvas1 = canvas;
    }

    public void Drawing(Graphics g)
    {
        var v = _canvas1.Viewer.Viewport;

        // 以_canvas1.Viewer的Zero为中心为坐标系(Zero为坐标原点)
        // 画十字架
        var vP1 = new Point(0, v.Y);
        var vP2 = new Point(0, v.Y + v.Height);
        g.DrawLine(_zeroLinePen, _canvas1.Viewer.LocalToShow(vP1), _canvas1.Viewer.LocalToShow(vP2));
        
        var hP1 = new Point(v.Left, 0);
        var hP2 = new Point(v.Left + v.Width, 0);
        g.DrawLine(_zeroLinePen, _canvas1.Viewer.LocalToShow(hP1), _canvas1.Viewer.LocalToShow(hP2));
    }
}