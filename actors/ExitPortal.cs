using System;
using System.Linq;
using Godot;

public partial class ExitPortal : Area3D
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var nearbyArtifacts = GetTree().CurrentScene.FindChildrenByPredicate<Artifact>(it => it.GlobalPosition.DistanceTo(GlobalPosition) < 5).DistinctBy(it => it.ArtifactID).Count();

        var light = this.FindChildByType<OmniLight3D>();
        light.LightEnergy = nearbyArtifacts * .33f;

        if (GetOverlappingBodies().OfType<PlayerCharacter>().Any())
        {
            GD.Print("PC is in area");
            if (nearbyArtifacts >= 3)
            {
                GetTree().ChangeSceneToFile("res://maps/WinScreen.tscn");
            }
        }
    }


}
