using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using Rage;
using RelaperCallouts.Callouts;

namespace RelaperCallouts
{
    public class Main : Plugin
    {
        public override void Finally()
        {
        }

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {
                Game.LogTrivial("Rel.C: Registering callouts...");
                Functions.RegisterCallout(typeof(ArmoredCarRobbery));
                Functions.RegisterCallout(typeof(StolenVehicle));
                Functions.RegisterCallout(typeof(Mugging));

                Game.DisplayNotification("web_lossantospolicedept", "web_lossantospolicedept", "RelaperCallouts", "~y~by RelaperCrystal", "~b~has been loaded. ~g~Enjoy!");

                Functions.OnOnDutyStateChanged -= Functions_OnOnDutyStateChanged;
                Game.LogTrivial("Rel.C: End initializing RelaperCallouts");
            }
        }
    }
}
