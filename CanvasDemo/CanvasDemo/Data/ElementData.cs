﻿using CanvasDemo.Canvas;

namespace CanvasDemo.Data;

public class ElementData : IElementData
{
    public string Id { get; set; }

    public int Group { get; set; }

    public string Title { get; set; }

    public bool IsError { get; set; }
}