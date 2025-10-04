using System;
using Godot;

public partial class Default : Node3D
{
    public override void _Ready()
    {
        base._Ready();

        var sb = this.FindChildByName<Node3D>("main_structure").FindChildByType<StaticBody3D>();
        sb.CollisionLayer = ColGroup.CHARACTERS | ColGroup.WALLS;
        sb.CollisionMask = ColGroup.CHARACTERS | ColGroup.WALLS;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey evt2)
        {
            if (evt2.IsActionPressed("quit_game") && !OS.HasFeature("web") && OS.IsDebugBuild())
            {
                GetTree().Quit();
            }

            if (evt2.IsActionPressed("cheat_open_all_doors") && OS.IsDebugBuild())
            {
                foreach (var it in GetTree().CurrentScene.FindChildrenByType<SlidingDoor>()) it.IsOpen = true;
                foreach (var it in GetTree().CurrentScene.FindChildrenByType<DestructibleLock>()) it.QueueFree();
            }

            if (evt2.IsActionPressed("cheat_spawn_laser") && OS.IsDebugBuild())
            {
                var pp = this.FindChildByType<PlayerCharacter>().GlobalPosition;
                var laser = GD.Load<PackedScene>("res://actors/Laser.tscn").Instantiate<Laser>();
                GetTree().CurrentScene.AddChild(laser);
                laser.GlobalPosition = pp;
            }

            if (evt2.IsActionPressed("cheat_into_room_2") && OS.IsDebugBuild())
            {
                var pp = GetTree().CurrentScene.FindChildByName<Node3D>("Room2");
                this.FindChildByType<PlayerCharacter>().GlobalPosition = pp.GlobalPosition;

                var laser = GD.Load<PackedScene>("res://actors/Laser.tscn").Instantiate<Laser>();
                GetTree().CurrentScene.AddChild(laser);
                laser.GlobalPosition = pp.GlobalPosition;
            }

            if (evt2.IsActionPressed("cheat_into_room_3") && OS.IsDebugBuild())
            {
                var pp = GetTree().CurrentScene.FindChildByName<Node3D>("Room3");
                this.FindChildByType<PlayerCharacter>().GlobalPosition = pp.GlobalPosition + new Vector3(0, 2.6f, 0);
            }
        }
    }
}
