using System;
using Godot;

public partial class Robot : CharacterBody3D
{
    [Export]
    float MovementSpeed = 1;

    bool Falling;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var movementVector = new Vector2();

        if (OS.IsDebugBuild())
        {
            movementVector = new Vector2(
                Input.GetAxis("cheat_robot_left", "cheat_robot_right"),
                Input.GetAxis("cheat_robot_backward", "cheat_robot_forward")
            );
        }

        Velocity = ((GlobalTransform.Basis.Z * -movementVector.Y) + (GlobalTransform.Basis.X * movementVector.X)) * 10 * MovementSpeed;

        if (Falling) Velocity += Vector3.Down * 20;

        var initialVelocity = Velocity;

        MoveAndSlide();

        Falling = GetSlideCollisionCount() == 0;

        if (Velocity.Length() <= 0.1f)
            MotionMode = MotionModeEnum.Floating;
        else
            MotionMode = MotionModeEnum.Grounded;

        // if (Mathf.Abs(robotControl.X) > 0.01f)
        // {
        //     AngularVelocity = new Vector3(
        //         AngularVelocity.X,
        //         robotControl.X * -10f,
        //         AngularVelocity.Z
        //     );
        //     AxisLockAngularY = false;
        // }
        // else
        // {
        //     AxisLockAngularY = true;
        // }

        // AddConstantCentralForce(-GlobalBasis.Z * robotControl.Y);

        // GD.Print(AngularVelocity);
    }
}
