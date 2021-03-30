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
        private bool spooked;
        private LHandle pursuit;
        private bool tasking;

        protected override string Name => "Mugging";

        protected override string ScannerCrimeName => "CRIME_MUGGING";

        public override bool OnBeforeCalloutDisplayed()
        {
            if (!SpawnUtil.TryGenerateSpawnPointOnPedWalk(300f, 600f, false, out Vector3 outP))
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

            Blip = new Blip(robber.Position.Around(30f), 80f)
            {
                Alpha = 0.5f,
                IsRouteEnabled = true
            };
            Blip.SetColor(BlipColor.YellowDynamic);
            
            ScannerMessages.DisplayDispatchText("Mugging", "Find the ~r~robber ~w~and save the ~g~victim~w~ in the ~y~area~w~!");

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (!robber.Exists() || robber.IsDead || Functions.IsPedArrested(robber)) EndSuccess();

            // When player is close, the suspect will now aim the player.
            // This is observed and learned from Fighting call-out in United Call-outs.
            if (!tasking && Game.LocalPlayer.Character.DistanceTo(robber) < 50f && robber.IsOnScreen)
            {
                tasking = true;

                robber.Tasks.AimWeaponAt(victim, -1);
                victim.Tasks.PutHandsUp(-1, robber);
            }

            if (!spooked && Game.LocalPlayer.Character.DistanceTo2D(robber) < 5f && robber.IsOnScreen)
            {
                spooked = true;
                // Prevent cheating
                if (Blip) Blip.Delete();

                // 1/9 chance to shoot the victim
                if (MathHelper.GetRandomInteger(10) == 2)
                {
                    Functions.SetPursuitDisableAIForPed(robber, true);
                    // Setting any ped to not block permanent events while they have a weapon
                    // will simply make them fight back
                    robber.BlockPermanentEvents = false;
                    robber.Tasks.FightAgainst(victim, -1);
                    // Victim don't have any weapon...
                    victim.BlockPermanentEvents = false;
                }

                // The victim will ran away as player will be close!
                victim.Tasks.ReactAndFlee(robber);
                victim.Dismiss();

                // Intended to let the player report the crime
                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, robber);
                Functions.SetPursuitAsCalledIn(pursuit, false);
            }

            if (spooked && !Functions.IsPursuitStillRunning(pursuit))
            {
                EndSuccess();
            }

            base.Process();
        }

        public override void End()
        {
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
