using System;
using Godot;

public interface IHoverable{
    public String GetHoverText();
    public Transform3D GetGlobalTransform();
}
