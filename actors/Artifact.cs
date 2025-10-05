using Godot;

public partial class Artifact : Grabbable
{
    [Export]
    public int ArtifactID;

    public override void _Ready()
    {
        base._Ready();

        LD58Util.AddBonkToBody(this);
    }
}