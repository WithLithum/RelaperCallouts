﻿using System;
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
        private Blip vehicleBlip;
        private readonly List<Ped> robbers = new List<Ped>();
        private LHandle pursuit;
        private bool inPursuit;
        private bool nearlyDone;
        private readonly MusicEvent bgm = new MusicEvent("FH2B_NOOSE_FIGHT");

        private static readonly Model[] models = new Model[]
        {
            "s_m_y_robber_01",
            "hc_gunman",
            "hc_driver",
            "g_m_y_lost_01",
            "g_m_y_lost_02",
            "g_m_y_lost_03",
            "g_m_y_ballaeast_01",
            "g_m_y_ballaorig_01",
            "g_m_y_famca_01",
            "g_m_y_famdnf_01",
            "g_m_y_famfor_01",
            "g_m_y_strpunk_01",
            "g_m_y_strpunk_02"
        };

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
                var ped = new Ped(models[MathHelper.GetRandomInteger(models.Length + 1)], SpawnPoint.Around(15f), 0f)
                {
                    IsPersistent = true,
                    BlockPermanentEvents = true
                };

                // TODO a bit of randomization?
                ped.Inventory.GiveNewWeapon(WeaponHash.MicroSMG, short.MaxValue, true);
                
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

                robbers.Add(ped);
            }

            vehicleBlip = new Blip(armoredCar)
            {
                Sprite = BlipSprite.Enemy, // For some reason RPH uses GTAO blip (No 270)
                IsFriendly = false,
                IsRouteEnabled = true
            };

            vehicleBlip.SetColor(BlipColor.Red); // use native blip color to match vanilla design :)

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
            if (vehicleBlip) vehicleBlip.Delete();

            base.End();
        }

        public override void Process()
        {
            base.Process();

            int counter = 0;

            // End as in promise
            if (Game.IsKeyDown(Keys.End)) EndSuccess();

            if (!armoredCar.Exists())
            {
                EndSuccess();
            }

            if (!nearlyDone)
            {
                foreach (var robber in robbers)
                {
                    if (!robber.Exists() || robber.IsDead || Functions.IsPedArrested(robber))
                    {
                        counter++;
                        Game.DisplayNotification($"~r~{Functions.GetPersonaForPed(robber).FullName} has been dealt with. ~y~{counter}/{robbers.Count} Remaining.");
                    }
                }

                if (counter == robbers.Count) this.nearlyDone = true;

                if (!inPursuit && pursuit != null && Game.LocalPlayer.Character.Position.DistanceTo2D(armoredCar) < 30f && armoredCar.IsOnScreen)
                {
                    inPursuit = true;
                    bgm.Prepare();
                    vehicleBlip.Flash(200, -1); // make it flash
                    vehicleBlip.IsRouteEnabled = false;

                    pursuit = Functions.CreatePursuit();
                    foreach (var ped in robbers)
                    {
                        if (ped.Exists() && ped.IsAlive && !Functions.IsPedArrested(ped))
                        {
                            Functions.AddPedToPursuit(pursuit, ped);
                        }
                    }

                    Functions.SetPursuitAsCalledIn(pursuit);
                    Functions.SetPursuitCopsCanJoin(pursuit, true);
                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                    Functions.SetPursuitLethalForceForced(pursuit, true);

                    ScannerMessages.DisplayDispatchText("Armored Car Robbery", "Suspect fleeing.");
                    bgm.Trigger();
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.NooseAirUnit);
                }

                if (inPursuit && !Functions.IsPursuitStillRunning(pursuit))
                {
                    vehicleBlip.StopFlashing();
                    nearlyDone = true;
                    bgm.Dismiss();
                }
            }
        }
    }
}
