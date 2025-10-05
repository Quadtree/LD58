using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class PlayerCharacter : CharacterBody3D
{
    [Export]
    float MouseSensitivity = 1;

    [Export]
    float MovementSpeed = 1;

    bool Falling = false;

    float CurrentGrabRange;

    ButtonConsole CurrentButtonConsole;

    List<int> ArtifactsTouched = [];

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        var movementVector = new Vector2(
            Input.GetAxis("move_left", "move_right"),
            Input.GetAxis("move_backward", "move_forward")
        );

        Velocity = ((GlobalTransform.Basis.Z * -movementVector.Y) + (GlobalTransform.Basis.X * movementVector.X)) * 10 * MovementSpeed;

        if (Falling) Velocity += Vector3.Down * 20;

        var initialVelocity = Velocity;

        MoveAndSlide();

        Falling = GetSlideCollisionCount() == 0;

        if (Velocity.Length() <= 0.1f)
            MotionMode = MotionModeEnum.Floating;
        else
            MotionMode = MotionModeEnum.Grounded;

        //GD.Print($"Falling={Falling} initialVelocity={initialVelocity} Velocity={Velocity}");

        var cam = this.FindChildByType<Camera3D>();
        var grabDragTargetPos = cam.GlobalPosition + cam.GlobalTransform.Basis.Z * -CurrentGrabRange;

        this.FindChildByName<Node3D>("DebugHand").GlobalPosition = grabDragTargetPos;

        foreach (var it in GetTree().CurrentScene.FindChildrenByPredicate<Grabbable>(it => it.IsGrabbed))
        {
            var worldGrabPos = it.GlobalTransform * it.LocalGrabPos;

            var forceDir = (grabDragTargetPos - worldGrabPos) * 200;

            it.ApplyForce(forceDir, worldGrabPos - it.GlobalPosition);

            if (it.IsRotatable)
            {
                it.GlobalRotation = cam.GlobalRotation;
            }
        }


        if (CurrentButtonConsole != null)
        {
            var res = Picking.PickAtCursor(this, collisionMask: uint.MaxValue);
            if (res.Hit?.GetParent() != CurrentButtonConsole)
            {
                CurrentButtonConsole.Released();
                CurrentButtonConsole = null;
            }
        }

        // var visibleLights = 5;

        // foreach (var it in GetTree().CurrentScene.FindChildrenByType<OmniLight3D>().OrderBy(it => it.GlobalPosition.DistanceSquaredTo(GlobalPosition)))
        // {
        //     if (visibleLights-- > 0)
        //     {
        //         it.Visible = true;
        //     }
        //     else
        //     {
        //         it.Visible = false;
        //     }
        // }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion evt)
        {
            RotateY(evt.Relative.X / -500 * MouseSensitivity);

            var cam = this.FindChildByType<Camera3D>();

            cam.Rotation = new Vector3(
                Util.Clamp(cam.Rotation.X + evt.Relative.Y / -500 * MouseSensitivity, -Mathf.Pi / 2, Mathf.Pi / 2),
                0,
                0
            );
        }

        if (@event.IsAction("grab"))
        {
            var res = Picking.PickAtCursor(this, collisionMask: ColGroup.GRABABLE | ColGroup.WALLS);
            //GD.Print($"{res.Pos} {res.Hit}");

            if (res.Hit is Grabbable g)
            {
                g.Grabbed(res.Pos.Value);

                CurrentGrabRange = res.Pos.Value.DistanceTo(this.FindChildByType<Camera3D>().GlobalPosition);
                GD.Print($"grabbed={res.Hit} CurrentGrabRange={CurrentGrabRange}");

                if (g is Artifact art)
                {
                    if (!ArtifactsTouched.Contains(art.ArtifactID))
                    {
                        ArtifactsTouched.Add(art.ArtifactID);
                    }
                }
            }
            else if (res.Hit?.FindParentByType<ButtonConsole>() is ButtonConsole bb)
            {
                bb.Pressed();
                CurrentButtonConsole = bb;
            }
            else if (res.Hit.FindParentByType<AiScreen>() is AiScreen ai)
            {
                ai.RegisterHit(res.Hit);
            }
            else
            {
                GD.Print($"Failed to grab anything, thing hit was {res.Hit} {res.Hit?.Name} {res.Hit?.FindParentByType<ButtonConsole>()}");
            }
        }

        if (@event.IsActionReleased("grab"))
        {
            foreach (var it in GetTree().CurrentScene.FindChildrenByPredicate<Grabbable>(it => it.IsGrabbed))
            {
                it.Released();
            }

            if (CurrentButtonConsole != null)
            {
                CurrentButtonConsole.Released();
                CurrentButtonConsole = null;
            }
        }
    }
}
