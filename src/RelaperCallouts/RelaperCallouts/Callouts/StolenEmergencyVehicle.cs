using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Callouts.Framework;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    [CalloutInfo("Stolen Emergency Vehicle RC", CalloutProbability.Low)]
    public class StolenEmergencyVehicle : CalloutBase
    {
        private Vehicle vehicle;
        private Ped thief;
        private LHandle pursuit;

        protected override string Name => "Stolen Emergency Vehicle";

        protected override string ScannerCrimeName => "CRIME_PERSON_IN_A_STOLEN_VEHICLE";

        public override bool OnBeforeCalloutDisplayed()
        {
            ResponseType = CalloutResponseType.Code3;
            ReportedByUnits = MathHelper.GetRandomInteger(11) != 5;
            SpawnPoint = SpawnUtil.GenerateSpawnPointAroundPlayer(200f, 500f);

            this.AddMinimumDistanceCheck(150f, SpawnPoint);
            this.AddMaximumDistanceCheck(550f, SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            vehicle = new Vehicle(SpawnUtil.GetRandomEmergencyCarModel(), SpawnPoint);
            vehicle.IsPersistent = true;

            thief = new Ped(SpawnPoint.Around(1.5f))
            {
                IsPersistent = true,
                BlockPermanentEvents = true
            };

            thief.WarpIntoVehicle(vehicle, -1);

            if (MathHelper.GetRandomInteger(5) != 3)
            {
                vehicle.Windows[0].Smash();
                vehicle.IsStolen = true;
            }
            else
            {
                thief.Inventory.GiveNewWeapon(WeaponHash.Pistol, 90, true);
            }

            return base.OnCalloutAccepted();
        }
    }
}
