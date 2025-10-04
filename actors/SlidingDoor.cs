using System;
using Godot;

public partial class SlidingDoor : Node3D
{
    [Export]
    float MoveSpeed = 1;

    public bool IsOpen = false;

    float OpenAmount = 0;

    public override void _PhysicsProcess(double delta)
    {
        float TargetOpenAmount = IsOpen ? 1 : 0;

        float moveSpeed = (float)delta * MoveSpeed;

        if (Mathf.Abs(TargetOpenAmount - OpenAmount) <= moveSpeed)
            OpenAmount = TargetOpenAmount;
        else if (OpenAmount < TargetOpenAmount)
            OpenAmount += moveSpeed;
        else
            OpenAmount -= moveSpeed;

        this.FindChildByName<Node3D>("LeftDoor").Position = new Vector3(-1, 0, 0) * OpenAmount;
        this.FindChildByName<Node3D>("RightDoor").Position = new Vector3(1, 0, 0) * OpenAmount;
    }
}
