using System;
using Godot;

public partial class DestructibleRope : Destructible
{
    public override void Destructed()
    {
        base.Destructed();

        var crb = GetParent().GetParent().FindChildByType<RigidBody3D>();
        crb.AxisLockAngularX = false;
        crb.AxisLockAngularY = false;
        crb.AxisLockAngularZ = false;
        crb.AxisLockLinearX = false;
        crb.AxisLockLinearY = false;
        crb.AxisLockLinearZ = false;
        crb.Sleeping = false;
    }
}
