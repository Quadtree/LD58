using System;
using Godot;

public partial class ButtonConsole : Node3D
{
    [Export]
    float ApparentDirection;

    public bool IsPressed = false;

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

    public void Pressed()
    {
        GD.Print($"{this} Pressed");
    }

    public void Released()
    {
        GD.Print($"{this} Released");
    }
}
