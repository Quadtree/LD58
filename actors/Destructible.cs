using System;
using Godot;

public partial class Destructible : StaticBody3D
{
    public bool DestroyedPreviously = false;

    public virtual void Destructed()
    {
        if (DestroyedPreviously) GD.PushWarning("Multi-destroy???");

        DestroyedPreviously = true;

        CallDeferred("queue_free");
    }
}