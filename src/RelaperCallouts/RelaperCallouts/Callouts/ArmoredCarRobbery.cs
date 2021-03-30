using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    [CalloutInfo("Armored Car Robbery", CalloutProbability.Low)]
    public class ArmoredCarRobbery : CalloutBase
    {
        private Vehicle armoredCar;
        private readonly List<Ped> robbers = new List<Ped>();

        protected override string Name => "Armored Truck Robbery";

        protected override string ScannerCrimeName => "CRIME_ARMORED_TRUCK_ROBBERY";

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(200f, 500f));
            ResponseType = CalloutResponseType.Code3;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            armoredCar = new Vehicle("stockade", SpawnPoint);
            armoredCar.IsPersistent = true;

            bool driver = false;

            for (int i = 0; i < MathHelper.GetRandomInteger(2, 4); i++)
            {
                var ped = new Ped(SpawnPoint.Around(15f))
                {
                    IsPersistent = true,
                    BlockPermanentEvents = true
                };

                ped.Inventory.GiveNewWeapon(SpawnUtil.GetRandomArmedRobberWeapon(), short.MaxValue, true);
                
                if (!driver) // make them warp into correct location
                {
                    driver = true;
                    ped.WarpIntoVehicle(armoredCar, -1);
                    ped.Tasks.CruiseWithVehicle(armoredCar, 250f, VehicleDrivingFlags.Emergency);
                }
                else
                {
                    ped.WarpIntoVehicle(armoredCar, -2);
                }

                var attribute = Functions.GetPedPursuitAttributes(ped);
                attribute.SurrenderChancePittedAndCrashed = 10.2f;
                attribute.SurrenderChancePittedAndSlowedDown = 5.2f;
                attribute.SurrenderChanceCarBadlyDamaged = 23.2f;
                attribute.AverageSurrenderTime = 8;
                attribute.AverageFightTime = 2;
                attribute.SurrenderChancePitted = 2.5f;
                attribute.SurrenderChanceTireBurst = 1.2f;
                attribute.SurrenderChanceTireBurstAndCrashed = 5.1f;
                attribute.BurstTireMaxDrivingSpeedMult = 0.10f;
                attribute.BurstTireSurrenderMult = 10;

                Functions.SetPedResistanceChance(ped, 85.9f);

                robbers.Add(ped);
            }

            Blip = new Blip(armoredCar)
            {
                Sprite = BlipSprite.Enemy, // For some reason RPH uses GTAO blip (No 270)
                IsFriendly = false,
                IsRouteEnabled = true
            };

            Blip.SetColor(BlipColor.Red); // use native blip color to match vanilla design :)

            return base.OnCalloutAccepted();
        }

        public override void End()
        {
            foreach (var robber in robbers)
            {
                // we don't want arrested peds to be dismissed
                // since they can walk away and even take player's car
                if (robber.Exists() && !Functions.IsPedArrested(robber))
                {
                    robber.Dismiss();
                }
            }

            if (armoredCar) armoredCar.Dismiss();

            base.End();
        }

        public override void Process()
        {
            base.Process();

            if (!armoredCar.Exists())
            {
                EndSuccess();
            }

            for (int i = 0; i < robbers.Count; i++)
            {
                GameFiber.Yield();
                if (!robbers[i].Exists() || robbers[i].IsDead || Functions.IsPedArrested(robbers[i]))
                {
                    robbers.RemoveAt(i);
                }
            }

            if (robbers.Count == 0)
            {
                EndSuccess();
            }
        }
    }
}
