using System;
using Godot;

public partial class SlidingDoor : Node3D
{
    [Export]
    float MoveSpeed = 1;

    [Export]
    public bool IsOpen = false;

    bool PlayedOpeningSound = false;

    float OpenAmount = 0;

    AudioStream OpenSound;

    public override void _Ready()
    {
        base._Ready();

        OpenSound = GD.Load<AudioStream>("res://sounds/door_open2.wav");
    }

    public override void _PhysicsProcess(double delta)
    {
        float TargetOpenAmount = IsOpen ? 1 : 0;

        if (IsOpen && !PlayedOpeningSound)
        {
            Util.SpawnOneShotSound(OpenSound, this);
            PlayedOpeningSound = true;
        }

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
