using CanvasDemo.Canvas;
using System.Drawing;

namespace CanvasDemo.Painter;

/// <summary>
/// 鼠标指针悬停
/// </summary>
public class ToolTipComponent : IToolTip
{
    private readonly TimCanvas _canvas;

    private CubeElement _lastCube;

    public ToolTipComponent(TimCanvas canvas)
    {
        _canvas = canvas;
    }

    public void Drawing(Graphics g)
    {
    }

    public void DrawingAfter(Graphics g)
    {
    }

    /// <summary>
    /// 隐藏鼠标悬停
    /// </summary>
    public void Hide()
    {
        if (_lastCube == null) return;

        _lastCube.IsHover = false;
        _lastCube = null;
        _canvas.Refresh();
    }


    /// <summary>
    /// 开启鼠标悬停
    /// </summary>
    public void Show(IToolTipElement element)
    {
        if (element is not CubeElement cube || _lastCube == cube) return;

        if (_lastCube != null) _lastCube.IsHover = false;

        _lastCube = cube;
        _lastCube.IsHover = true;

        _canvas.Refresh();
    }
}