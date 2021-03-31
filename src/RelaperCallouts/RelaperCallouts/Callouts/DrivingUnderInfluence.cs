using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RelaperCallouts.Callouts.Framework;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    [CalloutInfo("DUI RC", CalloutProbability.High)]
    public class DrivingUnderInfluence : CalloutBase
    {
        private Vehicle vehicle;
        private Ped driver;
        private bool routeDisabled;

        protected override string Name => "Driving under the Influence";

        protected override string ScannerCrimeName => "CRIME_DUI";

        public override bool OnBeforeCalloutDisplayed()
        {
            ResponseType = CalloutResponseType.Code2;
            ReportedByUnits = false;
            SpawnPoint = SpawnUtil.GenerateSpawnPointAroundPlayer(250, 550);

            this.AddMaximumDistanceCheck(650f, SpawnPoint);
            this.AddMinimumDistanceCheck(200f, SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            vehicle = new Vehicle(SpawnUtil.GetRandomCivilianCarModel(), SpawnPoint)
            {
                IsPersistent = true
            };

            driver = vehicle.CreateRandomDriver();
            driver.IsPersistent = true;
            driver.BlockPermanentEvents = true;
            // Set the ped drunk
            NativeFunction.Natives.SET_PED_CONFIG_FLAG(driver, 100, true);

            driver.Tasks.CruiseWithVehicle(214f, VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundObjects | VehicleDrivingFlags.AllowWrongWay | VehicleDrivingFlags.AllowMedianCrossing |
                VehicleDrivingFlags.AvoidHighways | VehicleDrivingFlags.DriveAroundObjects);

            Blip = driver.AttachBlip();
            Blip.Sprite = BlipSprite.Enemy;
            Blip.Scale = 0.5f;
            Blip.SetColor(BlipColor.Red);
            Blip.SetRouteColor(BlipColor.LightRed);

            ScannerMessages.DisplayDispatchText("Driver Under The Influence", "Reports of a ~y~driver~w~ under the ~r~influence~w~. Test the driver for ~r~alcohol~w~ and ~r~narcotics.");

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (!routeDisabled && Game.LocalPlayer.Character.Position.DistanceTo(driver) < 50f && driver.IsOnScreen)
            {
                routeDisabled = true;
                Blip.IsRouteEnabled = false;
            }

            if (!driver || !vehicle || driver.IsDead || Functions.IsPedArrested(driver))
            {
                EndSuccess();
            }

            base.Process();
        }

        public override void End()
        {
            if (driver && !Functions.IsPedArrested(driver)) driver.Dismiss();
            if (vehicle) vehicle.Dismiss();

            base.End();
        }
    }
}
