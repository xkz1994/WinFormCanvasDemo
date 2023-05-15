using System.Drawing;

namespace CanvasDemo.Canvas;

public interface IToolTipElement
{
    /// <summary>
    /// 获得提示的内容
    /// </summary>
    /// <returns></returns>
    string GetToolTipTitle();
}

public interface IToolTip
{
    /// <summary>
    /// 开启鼠标悬停
    /// </summary>
    void Show(IToolTipElement element);

    /// <summary>
    /// 隐藏鼠标悬停
    /// </summary>
    void Hide();

    void Drawing(Graphics g);

    void DrawingAfter(Graphics g);
}