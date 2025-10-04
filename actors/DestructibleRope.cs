using System;
using Godot;

public partial class DestructibleRope : Destructible
{
    public override void Destructed()
    {
        base.Destructed();

        var crb = GetTree().CurrentScene.FindChildByName<RigidBody3D>("Artifact2");
        crb.AxisLockAngularX = false;
        crb.AxisLockAngularY = false;
        crb.AxisLockAngularZ = false;
        crb.AxisLockLinearX = false;
        crb.AxisLockLinearY = false;
        crb.AxisLockLinearZ = false;
        crb.Sleeping = false;

        GD.Print($"{crb} {crb.Name} is loose!");
    }
}
