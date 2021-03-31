using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Callouts.Framework;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    [CalloutInfo("Foot Pursuit", CalloutProbability.Medium)]
    public class FootPursuit : CalloutBase
    {
        private LHandle pursuit;
        private Ped cop;
        private Ped passenger;
        private Ped suspect;

        protected override string Name => "Foot Pursuit";

        protected override string ScannerCrimeName => "CRIME_RESIST_ARREST";

        public override bool OnBeforeCalloutDisplayed()
        {
            if (!SpawnUtil.TryGenerateSpawnPointOnPedWalk(250, 450, false, out Vector3 spawn))
            {
                Game.LogTrivial("Rel.C: Failed to find spawn point");
                return false;
            }

            SpawnPoint = spawn;
            ResponseType = CalloutResponseType.Code3;
            ReportedByUnits = true;

            this.AddMinimumDistanceCheck(200f, spawn);
            this.AddMaximumDistanceCheck(500f, spawn);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect = new Ped(SpawnPoint.Around(0.5f, 0.8f))
            {
                BlockPermanentEvents = true,
                IsPersistent = true
            };

            var chaser = Functions.RequestBackup(SpawnPoint, EBackupResponseType.Code3, EBackupUnitType.LocalUnit, string.Empty, true, false);

            if (chaser == null)
            {
                Game.LogTrivial("Rel.C: Failed to spawn cop");
                return false;
            }

            chaser.IsPersistent = true;
            chaser.Position = World.GetNextPositionOnStreet(SpawnPoint);

            var openDoor = MathHelper.GetRandomInteger(4) == 2;

            cop = chaser.Driver;
            cop.IsPersistent = true;
            cop.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
            if (openDoor) chaser.Doors[0].Open(true);

            if (!chaser.IsSeatFree(0))
            {
                passenger = chaser.GetPedOnSeat(0);
                passenger.IsPersistent = true;
                passenger.Tasks.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                if (openDoor) chaser.Doors[1].Open(true);
            }

            pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(pursuit, suspect);
            Functions.AddCopToPursuit(pursuit, cop);
            if (passenger) Functions.AddCopToPursuit(pursuit, passenger);
            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
            Functions.SetPursuitCopsCanJoin(pursuit, true);

            ScannerMessages.DisplayDispatchText("Foot Pursuit", "Get to the target and catch the suspect!");
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!cop || !suspect || !Functions.IsPursuitStillRunning(pursuit))
            {
                EndSuccess();
            }
        }

        public override void End()
        {
            if (cop) cop.Dismiss();
            if (passenger) passenger.Dismiss();
            if (suspect && !Functions.IsPedArrested(suspect)) suspect.Dismiss();

            base.End();
        }
    }
}
