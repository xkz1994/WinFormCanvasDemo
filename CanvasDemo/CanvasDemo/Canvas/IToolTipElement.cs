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
    void Show(IToolTipElement element);

    void Hide();

    void Drawing(Graphics g);

    void DrawingAfter(Graphics g);
}