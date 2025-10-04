using System;
using Godot;

public partial class Robot : RigidBody3D
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var robotControl = new Vector2();

        if (OS.IsDebugBuild())
        {
            robotControl = new Vector2(
                Input.GetAxis("cheat_robot_left", "cheat_robot_right"),
                Input.GetAxis("cheat_robot_backward", "cheat_robot_forward")
            );
        }

        if (Mathf.Abs(robotControl.X) > 0.01f)
        {
            AngularVelocity = new Vector3(
                AngularVelocity.X,
                robotControl.X * -10f,
                AngularVelocity.Z
            );
            AxisLockAngularY = false;
        }
        else
        {
            AxisLockAngularY = true;
        }

        AddConstantCentralForce(-GlobalBasis.Z * robotControl.Y);

        GD.Print(AngularVelocity);
    }
}
