using System;
using Godot;

public partial class ButtonConsole : Node3D
{
    [Export]
    float ApparentDirection;

    public override void _Ready()
    {
        base._Ready();

        var bb = this.FindChildByName<Node3D>("PressableButton");
        bb.Rotation = new Vector3(
            bb.Rotation.X,
            bb.Rotation.Y,
            ApparentDirection * Mathf.Pi / 2 * ApparentDirection
        );
    }
}
