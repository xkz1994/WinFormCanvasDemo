using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasDemo.Canvas;

public abstract class Layer
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 画布控件
    /// </summary>
    public TimCanvas Canvas { get; set; }

    /// <summary>
    /// 所有元素
    /// </summary>
    public List<ObjElement> Elements { get; set; } = new();

    /// <summary>
    /// 是否被激活，只有激活状态的图层上的对象才能被选择 默认false
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 图层是否可见，但图层多的时候便于操作
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 是否是互动图层，就是用于交互操作的图层，通常用于放置对象，与他对应的就是辅助图层，用于显示辅助内容，没有交互操作 默认false
    /// </summary>
    public bool IsInteractionLayer { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="canvas">控件</param>
    /// <param name="name">名称</param>
    protected Layer(TimCanvas canvas, string name)
    {
        Canvas = canvas;
        Name = name;
        Canvas.LayerList.Add(this);
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

    public virtual bool MouseDown(MouseEventArgs e) => false;
    public virtual bool MouseMove(MouseEventArgs e) => false;
    public virtual bool MouseUp(MouseEventArgs e) => false;
    public virtual bool MouseWheel(MouseEventArgs e) => false;
}