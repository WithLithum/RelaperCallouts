using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Callouts.Framework;
using RelaperCallouts.Extern;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    [CalloutInfo("Stolen Vehicle", CalloutProbability.Medium)]
    public class StolenVehicle : CalloutBase
    {
        private Vehicle car;
        private Ped thief;

        protected override string Name => "Stolen Vehicle";

        protected override string ScannerCrimeName => "CRIME_GRAND_THEFT_AUTO";

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = SpawnUtil.GenerateSpawnPointAroundPlayer(300f, 600f);
            ResponseType = CalloutResponseType.Code2;
            ReportedByUnits = false;

            // Don't let it get too far away!
            this.AddMaximumDistanceCheck(650f, SpawnPoint);
            this.AddMinimumDistanceCheck(280f, SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            var model = SpawnUtil.GetRandomCivilianCarModel();
            car = new Vehicle(model, SpawnPoint)
            {
                IsPersistent = true
            };

            car.Windows[0].Smash(); // so it's stolen recently
            car.DirtLevel = MathHelper.GetRandomSingle(0f, 15f); // add some dirt
            car.IsStolen = true;

            // Only if STP installed
            if (ExternManager.StopThePedInstalled && MathHelper.GetRandomInteger(0, 5) == 3)
            {
                StopThePedFunctions.SetVehicleHasNoRegistration(car);
            }

            thief = car.CreateRandomDriver();
            thief.IsPersistent = true;
            thief.BlockPermanentEvents = true;

            if (MathHelper.GetRandomInteger(0, 4) == 2)
            {
                Functions.AddPedContraband(thief, ContrabandType.Misc, "Brick");
            }

            Blip = car.AttachBlip();
            Blip.Sprite = BlipSprite.Enemy;
            Blip.Scale = 0.5f;
            Blip.SetColor(BlipColor.Red);
            Blip.SetRouteColor(BlipColor.Red);
            Blip.IsRouteEnabled = true;

            var attribute = Functions.GetPedPursuitAttributes(thief);
            attribute.SurrenderChanceCarBadlyDamaged = 20.5f;
            attribute.BurstTireMaxDrivingSpeedMult = 0.30f;
            attribute.SurrenderChancePitted = 34.2f;
            attribute.SurrenderChanceTireBurst = 20.2f;
            attribute.SurrenderChanceTireBurstAndCrashed = 50.2f;
            attribute.SurrenderChancePittedAndCrashed = 42.2f;

            Functions.SetPedResistanceChance(thief, 80f);
            thief.Tasks.CruiseWithVehicle(30f);

            ScannerMessages.DisplayDispatchText("Stolen Vehicle", "Track down the ~g~vehicle~w~ and arrest the ~r~suspect.");

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!car) EndSuccess();
            if (!thief || thief.IsDead || Functions.IsPedArrested(thief)) EndSuccess();
        }

        public override void End()
        {
            if (thief && !Functions.IsPedArrested(thief)) thief.Dismiss();
            if (car) car.Dismiss();

            base.End();
        }
    }
}
