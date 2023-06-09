using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CanvasDemo.Canvas;

/// <summary>
/// 编辑的对象元素
/// 选择绘制的元素
/// </summary>
public class ElementEditor : Element
{
    /// <summary>
    /// 选择的对象集合
    /// </summary>
    public readonly List<ObjElement> SelectedElements = new();

    /// <summary>
    /// 放大或者移动的起始点
    /// </summary>
    public Point MPoint;

    /// <summary>
    /// 对象状态
    /// </summary>
    private EditorState _editorState = EditorState.None;

    /// <summary>
    /// 拖动柄状态
    /// </summary>
    private TransformState _transformState = TransformState.None;

    /// <summary>
    /// 鼠标选择元素框元素
    /// </summary>
    private readonly SelectionBox _selectionBox;

    public ElementEditor(TimCanvas canvas) : base(canvas, nameof(ElementEditor))
    {
        _selectionBox = new SelectionBox(this, canvas);
    }

    public override void Drawing(Graphics g)
    {
        //绘制选择对象的拖动柄
        SelectedElements.ForEach(x => x.DrawingJoystick(g));

        _selectionBox.Drawing(g);
    }


    public void MouseDown(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        MPoint = Viewer.MousePointToLocal(e.Location);
        var elem = SelectedElements.FirstOrDefault(x => x.Rect.Contains(MPoint));
        if (elem != null)
        {
            //点击已经选择的对象
            SetCurrent(elem); //设定当前点的对象为操作对象

            //如果是只读，那么就不要进入移动和调整大小模式
            if (Canvas.IsLocked) return;

            var leftJoystickLeftBoardResult = MPoint.X > elem.Rect.X; // 左排锚点 左边界
            var leftJoystickRightBoardResult = MPoint.X < elem.Rect.X + elem.JoystickSize; // 左排锚点 右边界

            var rightJoystickLeftBoardResult = MPoint.X > elem.Rect.Right - elem.JoystickSize; // 右排锚点 左边界
            var rightJoystickRightBoardResult = MPoint.X < elem.Rect.Right; // 右排锚点 右边界

            var topJoystickTopBoardResult = MPoint.Y > elem.Rect.Top; // 上排锚点 上边界
            var topJoystickBottomBoardResult = MPoint.Y < elem.Rect.Top + elem.JoystickSize; // 上排锚点 下边界

            var bottomJoystickTopBoardResult = MPoint.Y > elem.Rect.Bottom - elem.JoystickSize; // 下排锚点 上边界
            var bottomJoystickBottomBoardResult = MPoint.Y < elem.Rect.Bottom; // 下排锚点 下边界

            var middleOfTopAndBottomJoystickTopBoardResult = MPoint.Y > elem.Rect.Y + elem.Rect.Height / 2 - elem.JoystickSize / 2; // 上下排中间的锚点 上边界
            var middleOfTopAndBottomJoystickBottomBoardResult = MPoint.Y < elem.Rect.Y + elem.Rect.Height / 2 + elem.JoystickSize / 2; // 上下排中间的锚点 上边界

            var middleOfLeftAndRightJoystickLeftBoardResult = MPoint.X > elem.Rect.X + elem.Rect.Width / 2 - elem.JoystickSize / 2; // 左右排中间的锚点 左边界
            var middleOfLeftAndRightJoystickRightBoardResult = MPoint.X < elem.Rect.X + elem.Rect.Width / 2 + elem.JoystickSize / 2; // 左右排中间的锚点 右边界

            if (leftJoystickLeftBoardResult && leftJoystickRightBoardResult && topJoystickTopBoardResult && topJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.LeftTop;
            }
            else if (middleOfLeftAndRightJoystickLeftBoardResult && middleOfLeftAndRightJoystickRightBoardResult && topJoystickTopBoardResult && topJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Top;
            }
            else if (rightJoystickLeftBoardResult && rightJoystickRightBoardResult && topJoystickTopBoardResult && topJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.RightTop;
            }
            else if (rightJoystickLeftBoardResult && rightJoystickRightBoardResult && middleOfTopAndBottomJoystickTopBoardResult && middleOfTopAndBottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Right;
            }
            else if (rightJoystickLeftBoardResult && rightJoystickRightBoardResult && bottomJoystickTopBoardResult && bottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.RightBottom;
            }
            else if (middleOfLeftAndRightJoystickLeftBoardResult && middleOfLeftAndRightJoystickRightBoardResult && bottomJoystickTopBoardResult && bottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Bottom;
            }
            else if (leftJoystickLeftBoardResult && leftJoystickRightBoardResult && bottomJoystickTopBoardResult && bottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.LeftBottom;
            }
            else if (leftJoystickLeftBoardResult && leftJoystickRightBoardResult && middleOfTopAndBottomJoystickTopBoardResult && middleOfTopAndBottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Left;
            }
            else
            {
                _editorState = EditorState.Move;
            }
        }
        else
        {
            _editorState = EditorState.Selection;
            _selectionBox.MouseDown(e);
        }
    }

    public void MouseMove(MouseEventArgs e)
    {
        switch (_editorState)
        {
            case EditorState.Move: //移动模式，设定对象位置
                var endMove = Viewer.MousePointToLocal(e.Location);
                var xMove = endMove.X - MPoint.X;
                var yMove = endMove.Y - MPoint.Y;

                SelectedElements.ForEach(elem => elem.Rect.Offset(xMove, yMove));
                MPoint = endMove;
                break;

            case EditorState.Transform: //调整大小
                var endTransform = Viewer.MousePointToLocal(e.Location);
                var xTransform = endTransform.X - MPoint.X;
                var yTransform = endTransform.Y - MPoint.Y;

                SelectedElements.ForEach(elem =>
                {
                    switch (_transformState)
                    {
                        case TransformState.LeftTop: // X动 Y动
                            elem.Rect.Width -= xTransform;
                            elem.Rect.Height -= yTransform;
                            elem.Rect.X += xTransform;
                            elem.Rect.Y += yTransform;
                            break;

                        case TransformState.Top: // X不动 Y动
                            elem.Rect.Height -= yTransform;
                            elem.Rect.Y += yTransform;
                            break;

                        case TransformState.RightTop: // X不动 Y动
                            elem.Rect.Width += xTransform;
                            elem.Rect.Height -= yTransform;
                            elem.Rect.Y += yTransform;
                            break;

                        case TransformState.Right:
                            elem.Rect.Width += xTransform;
                            break;

                        case TransformState.RightBottom:
                            elem.Rect.Width += xTransform;
                            elem.Rect.Height += yTransform;
                            break;

                        case TransformState.Bottom:
                            elem.Rect.Height += yTransform;
                            break;

                        case TransformState.LeftBottom: // X动 Y不动
                            elem.Rect.Width -= xTransform;
                            elem.Rect.Height += yTransform;
                            elem.Rect.X += xTransform;
                            break;

                        case TransformState.Left: // X动 Y不动
                            elem.Rect.Width -= xTransform;
                            elem.Rect.X += xTransform;
                            break;
                    }

                    if (elem.Rect.Width < 10) elem.Rect.Width = 10;
                    if (elem.Rect.Height < 10) elem.Rect.Height = 10;
                });

                MPoint = endTransform;
                break;
            case EditorState.Selection:
                //选择
                _selectionBox.MouseMove(e);
                break;
        }
    }

    public void MouseUp(MouseEventArgs e)
    {
        if (_editorState == EditorState.Selection)
        {
            _selectionBox.MouseUp(e);
        }

        _editorState = EditorState.None;
    }

    public void MouseWheel(MouseEventArgs e)
    {
    }

    #region 对象选择操作

    /// <summary>
    /// 当前对象
    /// </summary>
    public ObjElement CurrentElement = null;

    /// <summary>
    /// 清除选的的对象
    /// </summary>
    public void ClearSelected()
    {
        SelectedElements.ForEach(x => x?.UnSelected());
        SelectedElements.Clear();
    }

    /// <summary>
    /// 设置当前选择的对象
    /// </summary>
    /// <param name="element"></param>
    public void SetCurrent(ObjElement element)
    {
        CurrentElement?.UnCurrent();
        CurrentElement = element;
        CurrentElement?.Current();
    }

    /// <summary>
    /// 添加选中的对象
    /// </summary>
    /// <param name="elements"></param>
    public void AddSelected(List<ObjElement> elements)
    {
        SelectedElements.AddRange(elements);
        elements.ForEach(x => x?.Selected());

        SelectedObjElementsEvent?.Invoke(SelectedElements);
    }

    /// <summary>
    /// 选择了对象元素后触发此方法
    /// </summary>
    public event Action<List<ObjElement>> SelectedObjElementsEvent;

    #endregion


    #region 元素操作

    /// <summary>
    /// 被删除的对象,可以理解成当前布局的回收站，可以从这里拿回已经删除的对象
    /// </summary>
    public Dictionary<string, ObjElement> DeletedElems = new Dictionary<string, ObjElement>();

    /// <summary>
    /// 移除选择的对象
    /// </summary>
    public void RemoveSelectElements()
    {
        foreach (var item in SelectedElements)
        {
            item.Layer.Elements.Remove(item);
            DeletedElems.Add(item.Id, item);
        }

        ClearSelected();

        SelectedObjElementsEvent?.Invoke(SelectedElements);
    }

    #endregion

    #region 对象布局操作

    /// <summary>
    /// 左对齐
    /// </summary>
    public void AlignLeft()
    {
        if (SelectedElements.Count <= 1) return;

        foreach (var item in SelectedElements)
        {
            item.Rect.X = CurrentElement.Rect.X;
        }

        Canvas.Refresh();
    }

    /// <summary>
    /// 右对齐
    /// </summary>
    public void AlignRight()
    {
        if (SelectedElements.Count <= 1) return;

        foreach (var item in SelectedElements)
        {
            item.Rect.X = CurrentElement.Rect.Right - item.Rect.Width;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 上对齐
    /// </summary>
    public void AlignTop()
    {
        if (SelectedElements.Count <= 1) return;

        foreach (var item in SelectedElements)
        {
            item.Rect.Y = CurrentElement.Rect.Y;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 下对齐
    /// </summary>
    public void AlignBottom()
    {
        if (SelectedElements.Count <= 1) return;

        foreach (var item in SelectedElements)
        {
            item.Rect.Y = CurrentElement.Rect.Bottom - item.Rect.Height;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 居中齐
    /// </summary>
    public void AlignCenter()
    {
        if (SelectedElements.Count <= 1) return;

        var center = CurrentElement.Rect.X + CurrentElement.Rect.Width / 2;

        foreach (var item in SelectedElements)
        {
            item.Rect.X = center - item.Rect.Width / 2;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 中间齐
    /// </summary>
    public void AlignMiddle()
    {
        if (SelectedElements.Count <= 1) return;

        var middle = CurrentElement.Rect.Y + CurrentElement.Rect.Height / 2;

        foreach (var item in SelectedElements)
        {
            item.Rect.Y = middle - item.Rect.Height / 2;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 宽度相同
    /// </summary>
    public void SameWidth()
    {
        if (SelectedElements.Count <= 1) return;

        foreach (var item in SelectedElements)
        {
            item.Rect.Width = CurrentElement.Rect.Width;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 高度相同
    /// </summary>
    public void SameHeight()
    {
        if (SelectedElements.Count <= 1) return;

        foreach (var item in SelectedElements)
        {
            item.Rect.Height = CurrentElement.Rect.Height;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 大小相同
    /// </summary>
    public void SameSize()
    {
        if (SelectedElements.Count <= 1) return;

        foreach (var item in SelectedElements)
        {
            item.Rect.Width = CurrentElement.Rect.Width;
            item.Rect.Height = CurrentElement.Rect.Height;
        }

        ;
        Canvas.Refresh();
    }

    /// <summary>
    /// 水平间距相同
    /// </summary>
    public void SameHorizontalSpace()
    {
        if (SelectedElements.Count <= 1) return;

        var orderFans = SelectedElements.OrderBy(x => x.Rect.X).ToList();
        var minLeft = orderFans.First().Rect.X;
        var maxLeft = orderFans.Last().Rect.X;
        var distance = maxLeft - minLeft;

        for (int i = 0; i < orderFans.Count; i++)
        {
            orderFans[i].Rect.X = (int)(minLeft + (float)i / ((float)orderFans.Count - 1.0f) * distance);
        }

        Canvas.Refresh();
    }

    /// <summary>
    /// 垂直间距相同
    /// </summary>
    public void SameVerticalSpace()
    {
        if (SelectedElements.Count <= 1) return;

        var orderFans = SelectedElements.OrderBy(x => x.Rect.Y).ToList();
        var minTop = orderFans.First().Rect.Y;
        var maxTop = orderFans.Last().Rect.Y;
        var distance = maxTop - minTop;

        for (int i = 0; i < orderFans.Count; i++)
        {
            orderFans[i].Rect.Y = (int)(minTop + (float)i / ((float)orderFans.Count - 1.0f) * distance);
        }

        Canvas.Refresh();
    }

    public override void DrawingAfter(Graphics g)
    {
    }

    #endregion


    enum EditorState
    {
        /// <summary>
        /// 没有任何操作
        /// </summary>
        None,

        /// <summary>
        /// 框选状态
        /// </summary>
        Selection,

        /// <summary>
        /// 移动状态
        /// </summary>
        Move,

        /// <summary>
        /// 调整大小状态
        /// </summary>
        Transform
    }


    enum TransformState
    {
        /// <summary>
        /// 没有任何操作
        /// </summary>
        None,

        /// <summary>
        /// 鼠标再操纵柄左上角
        /// </summary>
        LeftTop,

        /// <summary>
        /// 鼠标再操纵柄上边
        /// </summary>
        Top,

        /// <summary>
        /// 鼠标在操纵柄右上角
        /// </summary>
        RightTop,

        /// <summary>
        /// 鼠标再操纵柄右边
        /// </summary>
        Right,

        /// <summary>
        /// 鼠标zai操纵柄右下角
        /// </summary>
        RightBottom,

        /// <summary>
        /// 鼠标再操纵柄下边
        /// </summary>
        Bottom,

        /// <summary>
        /// 鼠标再操纵柄左下角
        /// </summary>
        LeftBottom,

        /// <summary>
        /// 鼠标再操纵柄左边
        /// </summary>
        Left
    }
}