using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    [CalloutInfo("Mugging", CalloutProbability.Medium)]
    public class Mugging : CalloutBase
    {
        private Ped robber;
        private Ped victim;
        private Blip blip;
        private bool spooked;
        private LHandle pursuit;

        protected override string Name => "Mugging";

        protected override string ScannerCrimeName => "CRIME_MUGGING";

        public override bool OnBeforeCalloutDisplayed()
        {
            Vector3 outP;
            if (!SpawnUtil.TryGenerateSpawnPointOnPedWalk(300f, 600f, false, out outP))
            {
                Game.LogTrivial("Rel.C: Failed to find spawn point");
                return false;
            }

            SpawnPoint = outP;

            this.AddMinimumDistanceCheck(250f, SpawnPoint);
            this.AddMaximumDistanceCheck(650f, SpawnPoint);
            this.ResponseType = CalloutResponseType.Code3;
            this.ReportedByUnits = false;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            robber = new Ped(SpawnPoint.Around(0.8f))
            {
                IsPersistent = true,
                BlockPermanentEvents = true
            };

            robber.Inventory.GiveNewWeapon(WeaponHash.Pistol, 40, true);

            victim = new Ped(SpawnPoint.Around(0.4f))
            {
                IsPersistent = true,
                BlockPermanentEvents = true
            };

            robber.Tasks.AimWeaponAt(victim, -1);
            victim.Tasks.PutHandsUp(-1, robber);

            blip = new Blip(robber.Position.Around(30f), 80f);
            blip.SetColor(BlipColor.YellowDynamic);
            blip.Alpha = 0.5f;
            blip.IsRouteEnabled = true;

            ScannerMessages.DisplayDispatchText("Mugging", "Find the ~r~robber ~w~and save the ~g~victim~w~ in the ~y~area~w~!");

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (!robber.Exists() || robber.IsDead || Functions.IsPedArrested(robber)) EndSuccess();
            if (!victim.Exists()) EndSuccess();

            if (!spooked && Game.LocalPlayer.Character.DistanceTo2D(robber) < 5f && robber.IsOnScreen)
            {
                spooked = true;
                if (blip) blip.Delete();

                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, robber);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                // let the player call it in!
            }

            if (spooked && !Functions.IsPursuitStillRunning(pursuit))
            {
                EndSuccess();
            }

            base.Process();
        }

        public override void End()
        {
            if (blip) blip.Delete();
            if (pursuit != null && Functions.IsPursuitStillRunning(pursuit))
            {
                Functions.ForceEndPursuit(pursuit);
            }

            if (robber && !Functions.IsPedArrested(robber)) robber.Dismiss();
            if (victim && !Functions.IsPedArrested(victim)) victim.Dismiss();

            base.End();
        }
    }
}
