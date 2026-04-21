using System.Linq;

public class InputTextToTMPIcon
{
    public static bool TryConvertToXboxIcon(out string output ,string input)
    {
        string startStringFilter = "Press ";

        if (input.StartsWith(startStringFilter))
        {
            input = input.Remove(0, startStringFilter.Count());
        }

        switch (input)
        {
            //Face Buttons
            case "A":
                output = "<sprite=82>";
                return true;
            case "B":
                output = "<sprite=84>";
                return true;
            case "X":
                output = "<sprite=86>";
                return true;
            case "Y":
                output = "<sprite=88>";
                return true;
            //Bumpers Triggers
            case "RT":
                output = "<sprite=27>";
                return true;
            case "LT":
                output = "<sprite=21>";
                return true;
            case "RB":
                output = "<sprite=23>";
                return true;
            case "LB":
                output = "<sprite=37>";
                return true;
            //D-Pad
            case "D-Pad Right":
                output = "<sprite=54>";
                return true;
            case "D-Pad Left":
                output = "<sprite=51>";
                return true;
            case "D-Pad Up":
                output = "<sprite=44>";
                return true;
            case "D-Pad Down":
                output = "<sprite=67>";
                return true;
            //Right Stick
            case "RS Right":
                output = "<sprite=2>";
                return true;
            case "RS Left":
                output = "<sprite=0>";
                return true;
            case "RS Up":
                output = "<sprite=3>";
                return true;
            case "RS Down":
                output = "<sprite=18>";
                return true;
            case "Right Stick Press":
                output = "<sprite=6>";
                return true;
            case "RS":
                output = "<sprite name=\"xbox_right_stick\">";
                return true;
            //Left Stick
            case "LS Right":
                output = "<sprite=14>";
                return true;
            case "LS Left":
                output = "<sprite=12>";
                return true;
            case "LS Up":
                output = "<sprite=15>";
                return true;
            case "LS Down":
                output = "<sprite=10>";
                return true;
            case "Left Stick Press":
                output = "<sprite=5>";
                return true;
            case "LS":
                output = "<sprite name=\"xbox_left_stick\">";
                return true;
            //Misc
            case "Start":
                output = "<sprite=77>";
                return true;
            case "Select":
                output = "<sprite=81>";
                return true;

            default:
                output = null;
                return false;
        }
    }
}
