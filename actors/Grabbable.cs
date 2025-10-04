using System;
using Godot;

public partial class Grabbable : RigidBody3D
{
    public bool IsGrabbed = false;

    public Vector3 LocalGrabPos;

    float OrigAngularDamp = 0;
    float OrigLinearDamp = 0;

    const float OrigDampReturnRate = 0.15f;

    public virtual bool IsRotatable => false;

    public override void _Ready()
    {
        base._Ready();

        OrigAngularDamp = AngularDamp;
        OrigLinearDamp = LinearDamp;

        CollisionLayer = ColGroup.GRABABLE;
        CollisionMask = ColGroup.GRABABLE | ColGroup.WALLS;

        GD.Print($"Grabbable._Ready CollisionLayer={CollisionLayer}");
    }

    public void Grabbed(Vector3 worldPos)
    {
        LocalGrabPos = GlobalTransform.Inverse() * worldPos;
        IsGrabbed = true;

        AngularDamp = 100;
        LinearDamp = 100;

        GD.Print($"{this} has been grabbed, worldPos={worldPos} LocalGrabPos={LocalGrabPos}");
    }

    public void Released()
    {
        IsGrabbed = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsGrabbed)
        {
            LinearDamp = LinearDamp * (1 - OrigDampReturnRate) + OrigLinearDamp * OrigDampReturnRate;
            AngularDamp = AngularDamp * (1 - OrigDampReturnRate) + OrigAngularDamp * OrigDampReturnRate;
        }
    }
}
