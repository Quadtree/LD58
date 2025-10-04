using System;
using System.Collections.Generic;
using Godot;

public partial class Laser : Grabbable
{
    Node3D[] BeamSegments;

    public override void _Ready()
    {
        base._Ready();

        List<Node3D> segs = [];

        for (var i = 0; i < 10; ++i)
        {
            var seg = (Node3D)this.FindChildByName<Node3D>("BeamSegment").Duplicate();
            AddChild(seg);
            seg.Visible = false;
            segs.Add(seg);
        }

        BeamSegments = segs.ToArray();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var beamStart = GlobalPosition;
        var beamDirection = -GlobalBasis.Z;

        for (var i = 0; i < 10; ++i)
        {
            BeamSegments[i].Visible = false;
        }

        for (var i = 0; i < 10; ++i)
        {
            var fp = new PhysicsRayQueryParameters3D();
            fp.From = beamStart;
            fp.To = beamDirection * 1000;
            fp.CollisionMask = 1;
            fp.Exclude = [GetRid()];

            var hit = GetWorld3D().DirectSpaceState.IntersectRay(fp);

            if (hit.ContainsKey("position") && hit.ContainsKey("collider"))
            {
                var hitPos = (Vector3)hit["position"];
                var hitCollider = (PhysicsBody3D)hit["collider"];

                var currentBeamSegment = BeamSegments[i];

                currentBeamSegment.GlobalPosition = (beamStart + hitPos) / 2;
                currentBeamSegment.LookAt(hitPos);
                currentBeamSegment.Scale = new Vector3(1, 1, beamStart.DistanceTo(hitPos));
                currentBeamSegment.Visible = true;

                if (hitCollider is Destructible dest && !dest.DestroyedPreviously)
                {
                    dest.Destructed();
                }

                if (!IsReflector(hitCollider)) break;
            }
            else
            {
                GD.PushWarning("Laser hit something invalid?");
                break;
            }
        }
    }

    bool IsReflector(Node3D n3d)
    {
        return false;
    }
}
