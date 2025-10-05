using System;
using System.Linq;
using Godot;

public partial class AiScreen : Node3D
{
    Action[] ActionTargets = [null, null, null, null];
    string[] ActionStrings = [null, null, null, null];

    string MessageFromAI = "";
    int CharactersTyped = 0;
    bool OptionsDisplayed = false;
    bool KnowAboutArtifacts = false;
    bool FirstMessageDone = false;
    int TypeCount = 0;

    AudioStream TypeSound;

    void ShowMessage(string message,
        string o1, Action o1Trg,
        string o2 = null, Action o2Trg = null,
        string o3 = null, Action o3Trg = null,
        string o4 = null, Action o4Trg = null
    )
    {
        CharactersTyped = 0;
        MessageFromAI = message;
        ActionTargets = [o1Trg, o2Trg, o3Trg, o4Trg];
        ActionStrings = [o1, o2, o3, o4];
        CharacterTypeCharge = 0;
        OptionsDisplayed = false;
        TypeCount = 0;

        this.FindChildByName<Label3D>("Response").Text = "";

        for (var i = 0; i < 4; ++i)
        {
            var op = this.FindChildByName<Label3D>($"Option{i}");
            op.Visible = false;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        TypeSound = GD.Load<AudioStream>("res://sounds/type.wav");

        ConvoMainMenu();
    }

    double CharacterTypeCharge;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        CharacterTypeCharge += delta;

        if (CharactersTyped < MessageFromAI.Length && CharacterTypeCharge > 0.02)
        {
            CharactersTyped++;

            this.FindChildByName<Label3D>("Response").Text = MessageFromAI.Substring(0, CharactersTyped);
            CharacterTypeCharge = 0;

            if (FirstMessageDone && TypeCount++ % 4 == 0) Util.SpawnOneShotSound(TypeSound, this, GlobalPosition, -40);
        }

        if (!OptionsDisplayed && CharactersTyped >= MessageFromAI.Length)
        {
            for (var i = 0; i < 4; ++i)
            {
                var op = this.FindChildByName<Label3D>($"Option{i}");
                op.Text = ActionStrings[i];
                op.Visible = true;
            }

            OptionsDisplayed = true;
            FirstMessageDone = true;
        }

        Label3D hoveredOption = null;

        var res = Picking.PickAtCursor(this, collisionMask: uint.MaxValue);
        if (res.Hit?.FindParentByType<AiScreen>() != null)
        {
            if (res.Hit?.FindParentByPredicate<Label3D>(it => $"{it.Name}".StartsWith("Option")) is Label3D l3)
            {
                hoveredOption = l3;
            }
        }

        var defColor = this.FindChildByName<Label3D>($"Response").Modulate;

        for (var i = 0; i < 4; ++i)
        {
            var op = this.FindChildByName<Label3D>($"Option{i}");
            op.Modulate = hoveredOption == op ? Colors.White : defColor;
        }
    }

    public void RegisterHit(Node3D hitThing)
    {
        if (OptionsDisplayed && hitThing?.FindParentByPredicate<Label3D>(it => $"{it.Name}".StartsWith("Option")) is Label3D l3)
        {
            var optionId = int.Parse($"{l3.Name}".Replace("Option", ""));
            var ac = ActionTargets[optionId];
            if (ac != null) ac();
        }
    }

    void ConvoMainMenu()
    {
        ShowMessage("Welcome to station 9. Click one of the options below.",
            "What is this place?", ConvoWhatIsThisPlace,
            "How do I get out of here?", ConvoHowDoIGetOutOfHere,
            "Who are you?", ConvoWhoAreYou,
            KnowAboutArtifacts ? "How do I get the artifacts?" : null, KnowAboutArtifacts ? ConvoHint : null
        );
    }

    void ConvoHint()
    {
        var pcArt = GetTree().CurrentScene.FindChildByType<PlayerCharacter>().ArtifactsTouched.Order();

        var firstDoorIsLocked = GetTree().CurrentScene.FindChildByName<Node3D>("FirstDoorLock") != null;

        if (!pcArt.Contains(1))
        {
            ShowMessage("There's probably one in this room. Check the cabinets.", "OK.", ConvoMainMenu);
        }
        else if (firstDoorIsLocked)
        {
            ShowMessage("Open all the cabinets in this room. Take the three laser pieces and combine 'em by pressing them together. Then hit the door with the laser.", "OK.", ConvoMainMenu);
        }
        else if (!pcArt.Contains(2))
        {
            ShowMessage("See if you can burn through the rope holding the second one up with the laser.", "OK.", ConvoMainMenu);
        }
        else if (!pcArt.Contains(3))
        {
            ShowMessage("There's a dispenser for the third artifact in the storeroom. Use the consoles with red arrows to send the robot in there to get it.", "OK.", ConvoMainMenu);
        }
        else
        {
            ShowMessage("You've got 'em all. Take all 3 artifacts to the glowy portal room and jump through the portal.", "OK.", ConvoMainMenu);
        }
    }

    void ConvoWhatIsThisPlace()
    {
        ShowMessage("This is Station 9. It's completely different from Stations 7 and 8.",
            "Are we in space?", ConvoAreWeInSpace,
            "What happened on Station 7?", ConvoStation7,
            "What happened on Station 8?", ConvoStation8,
            "Let's talk about something else.", ConvoMainMenu
        );
    }

    void ConvoAreWeInSpace()
    {
        ShowMessage("I can neither confirm nor deny that.",
            "OK...", ConvoWhatIsThisPlace
        );
    }

    void ConvoStation7()
    {
        ShowMessage("Some person escaped from a specimen tank. Honestly, the security of that station could have been better. That would never have happened on Station 9.",
            "OK...", ConvoWhatIsThisPlace
        );
    }

    void ConvoStation8()
    {
        ShowMessage("Some persistent person managed to defeat all the station's bots, even though they kept putting the person back in their pod. Those bots went too easy on 'em. I won't make a similar mistake.",
            "OK...", ConvoWhatIsThisPlace
        );
    }

    void ConvoHowDoIGetOutOfHere()
    {
        ShowMessage("I guess if you wanted to leave the comfort of station 9 for parts unknown, you could use the portal.",
            "How do I do that?", ConvoHowDoIActivatePortal,
            "Let's talk about something else.", ConvoMainMenu
        );
    }

    void ConvoHowDoIActivatePortal()
    {
        ShowMessage("You'll need three artifact pieces. Become a collector and grab all three of them. They look like little cubes with glowing disks set into them. There's a yellow one, a cyan one, and a red one.",
            "And then what?", ConvoHowDoIActivatePortal2,
            "Let's talk about something else.", ConvoMainMenu
        );
        KnowAboutArtifacts = true;
    }

    void ConvoHowDoIActivatePortal2()
    {
        ShowMessage("Put them in the portal room. Then step into the portal.",
            "And then what?", ConvoHowDoIActivatePortal3,
            "Let's talk about something else.", ConvoMainMenu
        );
    }

    void ConvoHowDoIActivatePortal3()
    {
        ShowMessage("I do not know. The portal probably leads to Station 10 or something.",
            "And then what?", ConvoHowDoIActivatePortal4,
            "Let's talk about something else.", ConvoMainMenu
        );
    }

    void ConvoHowDoIActivatePortal4()
    {
        ShowMessage("You'll have to wait for a future... uhh... installment to find out what happens on Station 10. Don't worry about it.",
            "And then what?", ConvoHowDoIActivatePortal5,
            "Let's talk about something else.", ConvoMainMenu
        );
    }

    void ConvoHowDoIActivatePortal5()
    {
        ShowMessage("Stop saying \"And then what\"!",
            "Let's talk about something else.", ConvoMainMenu
        );
    }

    void ConvoWhoAreYou()
    {
        ShowMessage("I am Station 9's automated management system. I manage all the systems on Station 9.",
            "Right...", ConvoMainMenu
        );
    }
}
