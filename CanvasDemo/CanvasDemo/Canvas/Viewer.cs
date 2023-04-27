using System;
using System.Drawing;
using System.Windows.Forms;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace CanvasDemo.Canvas;

public class Viewer
{
    /// <summary>
    /// 零点坐标（默认为画板中间）
    /// Node: 不能用属性，不然没法使用Offset之类函数
    /// </summary>
    public Point Zero;

    /// <summary>
    /// 视口，当前用户可以看到的区域
    /// Node: 不能用属性，不然没法使用Offset之类函数
    /// </summary>
    public Rectangle Viewport;

    /// <summary>
    /// 缩放比例
    /// </summary>
    public float Zoom = 1;

    /// <summary>
    /// 画布控件
    /// </summary>
    private readonly TimCanvas _canvas;

    /// <summary>
    /// 最小比例
    /// </summary>
    private float _minZoom = 0.01f;

    /// <summary>
    /// 最大比例
    /// </summary>
    private float _maxZoom = 100;

    /// <summary>
    /// 鼠标中键按下
    /// </summary>
    private bool _isMouseMiddleDown;

    /// <summary>
    /// 移动前鼠标位置
    /// </summary>
    private Point _oldMousePoint;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="canvas">画布控件</param>
    public Viewer(TimCanvas canvas)
    {
        _canvas = canvas;

        //默认图纸坐标
        Zero = new Point(_canvas.Width / 2, _canvas.Height / 2);
        Viewport = new Rectangle(0 - Zero.X, 0 - Zero.Y, _canvas.Width, _canvas.Height);
    }


    #region 视图调整

    /// <summary>
    /// 鼠标按下事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Middle) _isMouseMiddleDown = true;
        _oldMousePoint = e.Location;
    }

    /// <summary>
    /// 鼠标松开事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Middle) _isMouseMiddleDown = false;
    }

    /// <summary>
    /// 鼠标移动事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseMove(MouseEventArgs e)
    {
        var newLocation = e.Location;

        if (_isMouseMiddleDown)
        {
            //鼠标中键移动图纸
            var x = (newLocation.X - _oldMousePoint.X);
            var y = (newLocation.Y - _oldMousePoint.Y);

            Zero.Offset(x, y);

            Viewport.X = (int)((0 - Zero.X) / Zoom);
            Viewport.Y = (int)((0 - Zero.Y) / Zoom);
        }

        _oldMousePoint = newLocation;
    }

    /// <summary>
    /// 鼠标移动事件
    /// </summary>
    /// <param name="e">鼠标事件参数</param>
    public void MouseWheel(MouseEventArgs e)
    {
        //鼠标滚轮滚动图纸
        int tZeroX;
        int tZeroY;
        if (e.Delta > 0)
        {
            if (Zoom == _maxZoom) return;

            Zoom *= 1.25f;
            if (Zoom > _maxZoom) Zoom = _maxZoom;

            // 鼠标滚轮缩放指定地方不是用Zero点缩放
            tZeroX = (int)((e.X - Zero.X) - (e.X - Zero.X) * 1.25f);
            tZeroY = (int)((e.Y - Zero.Y) - (e.Y - Zero.Y) * 1.25f);
        }
        else
        {
            if (Zoom == _minZoom) return;

            Zoom *= 0.8f;
            if (Zoom < _minZoom) Zoom = _minZoom;

            // 鼠标滚轮缩放指定地方不是用Zero点缩放
            tZeroX = (int)((e.X - Zero.X) - (e.X - Zero.X) * 0.8f);
            tZeroY = (int)((e.Y - Zero.Y) - (e.Y - Zero.Y) * 0.8f);
        }

        //调整相对坐标位置
        Zero.Offset(tZeroX, tZeroY);

        //调整视口位置
        Viewport.X = (int)((0 - Zero.X) / Zoom);
        Viewport.Y = (int)((0 - Zero.Y) / Zoom);
        Viewport.Width = (int)((_canvas.Width) / Zoom);
        Viewport.Height = (int)((_canvas.Height) / Zoom);
    }

    /// <summary>
    /// 设置缩放
    /// </summary>
    /// <param name="zoom">比例</param>
    public void SetZoom(float zoom)
    {
        Zoom = zoom;
        _canvas.Refresh();
    }

    /// <summary>
    /// 设置原点
    /// </summary>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    public void SetZero(int x, int y)
    {
        Zero.X = x;
        Zero.Y = y;
        //调整视口位置
        Viewport.X = (int)((0 - Zero.X) / Zoom);
        Viewport.Y = (int)((0 - Zero.Y) / Zoom);
    }


    /// <summary>
    /// 设置成完整显示
    /// </summary>
    public void SetFullDisplay()
    {
        var w = _canvas.Width / (float)_canvas.BackgrounderSize.Width;
        var h = _canvas.Height / (float)_canvas.BackgrounderSize.Height;
        Zoom = w < h ? w : h;
        _minZoom = Zoom;
        _maxZoom = Zoom * 100;

        Zero.X = _canvas.Width / 2;
        Zero.Y = _canvas.Height / 2;

        Viewport.X = (int)((0 - Zero.X) / Zoom);
        Viewport.Y = (int)((0 - Zero.Y) / Zoom);
        Viewport.Width = (int)(_canvas.Width / Zoom);
        Viewport.Height = (int)(_canvas.Height / Zoom);

        _canvas.Refresh();
    }

    #endregion

    #region 控制是否绘制，用于优化性能

    /// <summary>
    /// 是否在区域中用于优化性能，这个涉及到性能优化
    /// </summary>
    /// <param name="element">元素</param>
    /// <returns>是否相交</returns>
    public bool InZone(Element element)
    {
        // 确定此矩形是否与矩形相交
        return Viewport.IntersectsWith(element.Rect);
    }

    /// <summary>
    /// 是否在区域中用于优化性能，这个涉及到性能优化
    /// </summary>
    /// <param name="rect">矩形</param>
    /// <returns>是否相交</returns>
    public bool InZone(Rectangle rect)
    {
        // 确定此矩形是否与矩形相交
        return Viewport.IntersectsWith(rect);
    }

    #endregion

    #region 本地坐标转世界坐标

    /// <summary>
    /// 本地（图纸）矩形变换到显示矩形
    /// </summary>
    /// <param name="rect">本地（图纸）矩形</param>
    /// <returns>显示矩形</returns>
    public Rectangle LocalToShow(Rectangle rect)
    {
        //要清晰的确定本地和世界的关系
        var r = new Rectangle(rect.Location, rect.Size);

        //计算缩放坐标
        r.X = (int)(r.X * Zoom) + Zero.X;
        r.Y = (int)(r.Y * Zoom) + Zero.Y;

        //调整矩形大小
        r.Width = (int)Math.Round(rect.Width * Zoom, 0);
        r.Height = (int)Math.Round(rect.Height * Zoom, 0);

        return r;
    }

    /// <summary>
    /// 本地（图纸）坐标变换到显示坐标
    /// </summary>
    /// <param name="point">本地（图纸）坐标</param>
    /// <returns>显示矩形</returns>
    public Point LocalToShow(Point point)
    {
        return new Point((int)(point.X * Zoom) + Zero.X, (int)(point.Y * Zoom) + Zero.Y);
    }

    /// <summary>
    /// 本地（图纸）矩形变换到显示矩形
    /// </summary>
    /// <param name="x">本地（图纸）x</param>
    /// <param name="y">本地（图纸）y</param>
    /// <param name="width">本地（图纸）width</param>
    /// <param name="height">本地（图纸）height</param>
    /// <returns>显示矩形</returns>
    public Rectangle LocalToShow(int x, int y, int width, int height)
    {
        return new Rectangle(
            (int)(x * Zoom) + Zero.X,
            (int)(y * Zoom) + Zero.Y,
            (int)Math.Round(width * Zoom, 0),
            (int)Math.Round(height * Zoom, 0)
        );
    }

    /// <summary>
    /// 本地（图纸）x变换到显示x
    /// </summary>
    /// <param name="x">本地（图纸）x</param>
    /// <returns>显示x</returns>
    public int ToShowX(int x)
    {
        return (int)(x * Zoom) + Zero.X;
    }

    /// <summary>
    /// 本地（图纸）y变换到显示y
    /// </summary>
    /// <param name="y">本地（图纸）y</param>
    /// <returns>显示y</returns>
    public int ToShowY(int y)
    {
        return (int)(y * Zoom) + Zero.Y;
    }

    /// <summary>
    /// 本地尺寸变换到显示的尺寸
    /// </summary>
    /// <param name="size">本地尺寸</param>
    /// <returns>显示尺寸</returns>
    public Size LocalToShow(Size size)
    {
        return new Size((int)Math.Round(size.Width * Zoom, 0), (int)Math.Round(size.Height * Zoom, 0));
    }

    /// <summary>
    /// 鼠标坐标点变换到显示坐标点
    /// </summary>
    /// <param name="point">鼠标坐标点</param>
    /// <returns>显示坐标点</returns>
    public Point MousePointToLocal(Point point)
    {
        return new Point((int)Math.Round((point.X - Zero.X) / Zoom, 0), (int)Math.Round((point.Y - Zero.Y) / Zoom, 0));
    }

    #endregion
}