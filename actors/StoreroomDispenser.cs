using System;
using Godot;

public partial class StoreroomDispenser : MeshInstance3D
{
    float DispenseCharge = 0;

    public void Dispense()
    {
        if (DispenseCharge >= 1)
        {
            var n = GD.Load<PackedScene>("res://actors/Artifact3.tscn").Instantiate<Node3D>();
            GetTree().CurrentScene.AddChild(n);
            n.GlobalPosition = GlobalPosition;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        DispenseCharge += (float)delta;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (OS.IsDebugBuild())
        {
            if (@event is InputEventKey ik)
            {
                if (ik.IsActionPressed("cheat_dispense"))
                {
                    Dispense();
                }
            }
        }
    }
}
