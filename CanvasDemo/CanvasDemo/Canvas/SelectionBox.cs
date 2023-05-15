using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CanvasDemo.Extension;

namespace CanvasDemo.Canvas;

/// <summary>
/// 鼠标选择元素框
/// 鼠标左键按下拖动选择（类似于桌面选择一些快捷键一样）
/// </summary>
public class SelectionBox : Element
{
    /// <summary>
    /// 编辑的对象元素
    /// </summary>
    private readonly ElementEditor _editor;

    /// <summary>
    /// 鼠标是否从左往右选择
    /// 1. 鼠标拖动是从左往右还是从右往左: 显示的颜色不一样
    /// 2. 从左往右选择: 全部选中才算选中
    /// 3. 从右往左选择: 相交就认为已经选中
    /// </summary>
    private bool _mouseMoveLeftToRight;

    /// <summary>
    /// 鼠标选择元素框是否显示
    /// </summary>
    private bool _selectionBoxIsShow;

    /// <summary>
    /// 框选开始坐标
    /// </summary>
    private Point _start;

    /// <summary>
    /// 鼠标左键按下
    /// </summary>
    private bool _leftMouseIsDown;

    public SelectionBox(ElementEditor editor, TimCanvas canvas) : base(canvas, nameof(SelectionBox))
    {
        _editor = editor;
    }

    public override void Drawing(Graphics g)
    {
        if (_selectionBoxIsShow == false) return;

        if (_mouseMoveLeftToRight)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(100, 51, 153, 255)), Viewer.LocalToShow(Rect));
            g.DrawRectangle(new Pen(Color.FromArgb(255, 51, 153, 255)), Viewer.LocalToShow(Rect));
        }
        else
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(100, 153, 255, 51)), Viewer.LocalToShow(Rect));
            g.DrawRectangle(new Pen(Color.FromArgb(255, 153, 255, 51)), Viewer.LocalToShow(Rect));
        }
    }

    /// <summary>
    /// 鼠标按下事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseDown(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        _leftMouseIsDown = true;
        _start = Viewer.MousePointToLocal(e.Location);
        Rect.Width = 0;
        Rect.Height = 0;
        _selectionBoxIsShow = true;
    }

    /// <summary>
    /// 鼠标移动事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseMove(MouseEventArgs e)
    {
        //比例缩放后结束坐标也要做调整
        if (_leftMouseIsDown == false) return;

        // 定位结束坐标
        var end = Viewer.MousePointToLocal(e.Location);

        // 判断鼠标移动方向（开始位置再左边）
        _mouseMoveLeftToRight = _start.X < end.X;

        Rect.X = _start.X < end.X ? _start.X : end.X;
        Rect.Y = _start.Y < end.Y ? _start.Y : end.Y;

        Rect.Width = Math.Abs(_start.X - end.X);
        Rect.Height = Math.Abs(_start.Y - end.Y);
    }

    /// <summary>
    /// 鼠标松开事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseUp(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        _leftMouseIsDown = false;
        _selectionBoxIsShow = false;

        var end = Viewer.MousePointToLocal(e.Location);
        //开始和结束距离短，认为是鼠标点击选择
        if (end.Distance(_start) < 15)
            PointSelectOver(e.Location);
        else
            BoxSelectOver();
    }

    /// <summary>
    /// 鼠标滚轮事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseWheel(MouseEventArgs e)
    {
    }

    /// <summary>
    /// 选择单个对象
    /// </summary>
    /// <param name="mousePoint">鼠标Point</param>
    private void PointSelectOver(Point mousePoint)
    {
        // 获取一个表示哪个修改键（Shift、Ctrl 和 Alt）处于按下状态的值 (Keys 值的按位组合)
        if (Control.ModifierKeys != Keys.Control) _editor.ClearSelected(); //撤销以前的选择

        var point = Viewer.MousePointToLocal(mousePoint);

        foreach (var elm in from layer in Canvas.LayerList
                 where layer.IsActive
                 select layer.Elements.FirstOrDefault(x => x.Rect.Contains(point)) // 获取第一个Element
                 into element
                 where element is not null
                 select element)
        {
            _editor.AddSelected(new List<ObjElement> { elm });
            _editor.SetCurrent(elm);
            return;
        }
    }

    /// <summary>
    /// 选择被框选的对象
    /// </summary>
    private void BoxSelectOver()
    {
        // 获取一个表示哪个修改键（Shift、Ctrl 和 Alt）处于按下状态的值 (Keys 值的按位组合)
        if (Control.ModifierKeys != Keys.Control) _editor.ClearSelected(); //撤销以前的选择

        foreach (var item in Canvas.LayerList.Where(item => item.IsActive))
        {
            _editor.AddSelected(_mouseMoveLeftToRight
                ? item.Elements.AsParallel().Where(x => Rect.Contains(x.Rect)).ToList() // 从左往右选择: 全部选中才算选中
                : item.Elements.AsParallel().Where(x => x.Rect.IntersectsWith(Rect)).ToList()); // 从右往左选择: 相交就认为已经选中
        }

        _editor.SetCurrent(_editor.SelectedElements.FirstOrDefault());
    }

    public override void DrawingAfter(Graphics g)
    {
    }
}