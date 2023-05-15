using System;
using System.Drawing;

namespace CanvasDemo.Extension;

/// <summary>
/// Point扩展雷
/// </summary>
public static class PointExtension
{
    /// <summary>
    /// 复制Point
    /// </summary>
    /// <param name="source">源Point</param>
    /// <returns>复制Point</returns>
    public static Point ToPoint(this Point source)
    {
        return new Point(
            Convert.ToInt32(source.X),
            Convert.ToInt32(source.Y));
    }

    /// <summary>
    /// 两个坐标围城的尺寸
    /// </summary>
    /// <param name="source">源Point</param>
    /// <param name="pt">目标Point</param>
    /// <returns>尺寸</returns>
    public static Size Subtract(this Point source, Point pt)
    {
        return new Size(source.X - pt.X, source.Y - pt.Y);
    }

    /// <summary>
    /// 计算两点之间的距离
    /// </summary>
    /// <param name="source">源Point</param>
    /// <param name="pt">目标Point</param>
    /// <returns>距离</returns>
    public static double Distance(this Point source, Point pt)
    {
        return Math.Sqrt((source.X - pt.X) * (source.X - pt.X) + (source.Y - pt.Y) * (source.Y - pt.Y));
    }

    /// <summary>
    /// 判断当前点是否在一组点组成的多边形中
    /// </summary>
    /// <param name="point">源Point</param>
    /// <param name="points">一组Point</param>
    /// <returns>是否在里面</returns>
    public static bool InZone(this Point point, Point[] points)
    {
        var myGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
        myGraphicsPath.Reset();
        //添家多边形点，绘制出路径 
        myGraphicsPath.AddPolygon(points);
        var myRegion = new Region();
        myRegion.MakeEmpty();
        //获得交集
        myRegion.Union(myGraphicsPath);

        //返回判断点是否在多边形里
        return myRegion.IsVisible(point);
    }
}