using System;
using System.Linq;
using Godot;

public partial class DestructibleLock : Destructible
{
    public override void Destructed()
    {
        base.Destructed();

        var closestDoor = GetTree().CurrentScene.FindChildrenByType<SlidingDoor>().MinBy(it => it.GlobalPosition.DistanceSquaredTo(GlobalPosition));
        closestDoor.IsOpen = true;
    }
}
