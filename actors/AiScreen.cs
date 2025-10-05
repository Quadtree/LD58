using System;
using Godot;

public partial class AiScreen : Node3D
{
    Action[] ActionTargets = [null, null, null, null];
    string[] ActionStrings = [null, null, null, null];

    string MessageFromAI = "";
    int CharactersTyped = 0;
    bool OptionsDisplayed = false;

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
        }

        if (!OptionsDisplayed && CharactersTyped >= MessageFromAI.Length)
        {
            for (var i = 0; i < 4; ++i)
            {
                var op = this.FindChildByName<Label3D>($"Option{i}");
                op.Text = ActionStrings[i];
                op.Visible = true;
            }
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
        if (hitThing?.FindParentByPredicate<Label3D>(it => $"{it.Name}".StartsWith("Option")) is Label3D l3)
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
            "Who are you?", ConvoWhoAreYou);
    }

    void ConvoWhatIsThisPlace()
    {

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
        ShowMessage("I am Station 9's automated management system. I manage all the systems on Station 9.", "Right...", ConvoMainMenu);
    }
}
