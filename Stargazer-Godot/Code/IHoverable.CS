using System;
using Godot;

public interface IHoverable{
    public String getHoverText();
    public Transform3D getGlobalTransform();
}
