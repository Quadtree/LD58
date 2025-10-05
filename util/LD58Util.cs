using System;
using System.Collections.Generic;
using Godot;

public static class LD58Util
{
    static AudioStream[] BonkSFX;

    public static void AddBonkToBody(RigidBody3D b)
    {
        if (BonkSFX == null)
        {
            List<AudioStream> st = [];

            for (var i = 0; i < 4; ++i)
            {
                st.Add(GD.Load<AudioStream>($"res://sounds/bonk{i}.wav"));
            }

            BonkSFX = st.ToArray();
        }

        b.ContactMonitor = true;
        b.MaxContactsReported = Math.Max(b.MaxContactsReported, 16);

        b.BodyEntered += (v1) =>
        {
            Util.SpawnOneShotSound(Util.Choice(BonkSFX), b, b.GlobalPosition, Util.Clamp(b.LinearVelocity.Length() * 8 - 20, -20, 10));
        };
    }
}