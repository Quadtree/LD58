using System;
using Godot;

public partial class Destructible : StaticBody3D
{
    public bool DestroyedPreviously = false;

    AudioStream BurnSound;

    public override void _Ready()
    {
        base._Ready();

        BurnSound = GD.Load<AudioStream>("res://sounds/burn.wav");
    }

    public virtual void Destructed()
    {
        if (DestroyedPreviously) GD.PushWarning("Multi-destroy???");

        Util.SpawnOneShotSound(BurnSound, this, GlobalPosition);

        DestroyedPreviously = true;

        CallDeferred("queue_free");
    }
}