using LSPD_First_Response.Mod.API;
using Rage;
using RelaperCallouts.Callouts.Framework;

namespace RelaperCallouts.Util
{
    internal static class ScannerMessages
    {
        internal static void DisplayResponseCode(CalloutResponseType code)
        {
            string codeDisplay;
            string codeSound = "RC_";

            switch (code)
            {
                default:
                    codeSound += "CODE2";
                    codeDisplay = "~g~Code 2";
                    break;
                case CalloutResponseType.Code3:
                    codeSound += "CODE3";
                    codeDisplay = "~r~Code 3";
                    break;
                case CalloutResponseType.Code99:
                    codeSound += "CODE99";
                    codeDisplay = "~r~Code 99";
                    break;
            }

            GameFiber.StartNew(() =>
            {
                GameFiber.Sleep(4500);
                Functions.PlayScannerAudio(codeSound);
                Game.DisplayNotification("Respond " + codeDisplay);
            });
        }

        internal static void EndCall(string name)
        {
            Functions.PlayScannerAudio("RC_STANDDOWN", true);
            DisplayDispatchText(name, "We're ~g~Code 4~w~. All units stand down, return to ~b~patrol~w~.");
        }

        internal static void DisplayDispatchText(string caption, string text)
        {
            Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "Dispatch", caption, text);
        }
    }
}
