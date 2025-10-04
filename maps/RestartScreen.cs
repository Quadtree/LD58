using System;
using Godot;

public partial class RestartScreen : Control
{
    float Cooldown = 1;

    public override void _Process(double delta)
    {
        base._Process(delta);

        Cooldown -= (float)delta;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey || @event is InputEventMouseButton)
        {
            if (Cooldown <= 0) GetTree().ChangeSceneToFile("res://maps/Default.tscn");
        }
    }
}
