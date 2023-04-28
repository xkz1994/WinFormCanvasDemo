using CanvasDemo.Canvas;
using CanvasDemo.Data;
using System.Drawing;

namespace CanvasDemo.Painter;

/// <summary>
/// 方形元素
/// </summary>
public class CubeElement : ObjElement<ElementData>, IToolTipElement
{
    public static readonly Brush FillBrush = new SolidBrush(Color.Blue);
    public static readonly Brush SelectBrush = new SolidBrush(Color.Green);
    public static readonly Brush ErrorBrush = new SolidBrush(Color.Red);

    protected static readonly StringFormat SfAlignment = new()
    {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center,
    };

    public CubeElement(CubeLayer layer, ElementData data, int sideLength) : base(layer, data)
    {
        Rect.Width = sideLength;
        Rect.Height = sideLength;
    }

    /// <summary>
    /// 鼠标指针是否悬停
    /// </summary>
    public bool IsHover { get; set; }


    public override void Drawing(Graphics g)
    {
        var titleH = (int)(Rect.Height * 0.25);

        //选择和错误使用不同的颜色
        var fillBrush = FillBrush;
        if (Data.IsError)
            fillBrush = ErrorBrush;
        else if (IsSelected)
            fillBrush = SelectBrush;

        switch (titleH * Viewer.Zoom)
        {
            //如果标题大于10就认真绘制，如哦小于那么就简化
            // ReSharper disable PossibleLossOfFraction
            case > 10:
            {
                var borderW = (int)(Rect.Height * 0.01 * Viewer.Zoom) + 1;

                g.FillRectangle(Brushes.White, Viewer.LocalToShow(Rect.X, Rect.Y, Rect.Width, titleH + 1));

                var fontSize = (int)(titleH / 2 * Viewer.Zoom) + 1;
                if (fontSize >= 3)
                {
                    g.DrawString(Data.Title, new Font("微软雅黑", fontSize > 60 ? 60 : fontSize), Brushes.Black,
                        Viewer.LocalToShow(Rect.X + (int)(borderW / Viewer.Zoom), Rect.Y + (int)(borderW / Viewer.Zoom), Rect.Width, Rect.Height));
                }

                var contentRect = Viewer.LocalToShow(Rect.X, Rect.Y + titleH, Rect.Width, Rect.Height - titleH);
                g.FillRectangle(fillBrush, contentRect);
                g.DrawString(Data.Group.ToString(), new Font("微软雅黑", (Rect.Height - titleH) / 2 * Viewer.Zoom),
                    Brushes.White, contentRect, SfAlignment);

                g.DrawRectangle(IsHover
                        ? new Pen(Brushes.Red, borderW * 2)
                        : new Pen(Brushes.Black, borderW),
                    Viewer.LocalToShow(Rect));
                break;
            }
            case > 5:
            {
                g.FillRectangle(fillBrush, Viewer.LocalToShow(Rect));

                var fontSize = (int)(titleH * Viewer.Zoom) + 1;
                if (fontSize >= 3)
                {
                    g.DrawString(Data.Group.ToString(), new Font("微软雅黑", fontSize > 60 ? 60 : fontSize), Brushes.White, Viewer.LocalToShow(Rect.X + 1, Rect.Y + 1, Rect.Width, Rect.Height),
                        SfAlignment);
                }

                break;
            }
            default:
                g.FillRectangle(fillBrush, Viewer.LocalToShow(Rect));
                break;
        }
    }

    public override void DrawingAfter(Graphics g)
    {
    }

    public string GetToolTipTitle()
    {
        return $"[{Data.Group}] {Data.Title}";
    }
}