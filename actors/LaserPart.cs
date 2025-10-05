using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LaserPart : Grabbable
{
    [Export]
    Godot.Collections.Array<int> PartIDs;

    AudioStream CombineSound;

    public override void _Ready()
    {
        BodyEntered += (v1) =>
        {
            GD.Print($"Collision! {this} {v1}");

            if (v1 is LaserPart lp)
            {
                if (PartIDs.Count > lp.PartIDs.Count || (PartIDs.Count == lp.PartIDs.Count && PartIDs.Min() < lp.PartIDs.Min()))
                {
                    CallDeferred(nameof(ConsumeOther), [lp]);
                }
            }
        };

        CombineSound = GD.Load<AudioStream>("res://sounds/combine.wav");
    }

    void ConsumeOther(LaserPart lp)
    {
        foreach (var it in lp.PartIDs) PartIDs.Add(it);
        lp.PartIDs.Clear();
        lp.QueueFree();

        GD.Print($"After consumption, part ids is {PartIDs}");

        if (PartIDs.Intersect(new int[] { 0, 1, 2 }).Count() == 3)
        {
            CallDeferred(nameof(TurnIntoLaser));
        }

        Util.SpawnOneShotSound(CombineSound, this, GlobalPosition);
    }

    void TurnIntoLaser()
    {
        PartIDs.Clear();

        var laser = GD.Load<PackedScene>("res://actors/Laser.tscn").Instantiate<Laser>();
        GetTree().CurrentScene.AddChild(laser);
        laser.GlobalPosition = GlobalPosition;

        QueueFree();
    }
}
