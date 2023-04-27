using System.Drawing;

// ReSharper disable MemberCanBeProtected.Global

namespace CanvasDemo.Canvas;

/// <summary>
/// 元素
/// </summary>
public abstract class Element
{
    /// <summary>
    /// 元素唯一标识ID
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 当前对象的区域范围
    /// 不能用属性，因为属性不能给修改内部值
    /// </summary>
    public Rectangle Rect;

    /// <summary>
    /// 画布控件
    /// </summary>
    public readonly TimCanvas Canvas;

    /// <summary>
    /// 视图
    /// </summary>
    public readonly Viewer Viewer;

    protected Element(TimCanvas canvas, string id)
    {
        Canvas = canvas;
        Viewer = canvas.Viewer;
        Id = id;
    }

    /// <summary>
    /// 正常绘图
    /// </summary>
    /// <param name="g"></param>
    public abstract void Drawing(Graphics g);

    /// <summary>
    /// 第二次绘制，用于显示一些在前端的文字等
    /// </summary>
    public abstract void DrawingAfter(Graphics g);
}