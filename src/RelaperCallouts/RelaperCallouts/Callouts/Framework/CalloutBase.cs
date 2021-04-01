using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts.Framework
{
    public abstract class CalloutBase : Callout
    {
        // Any normal exit behavior (via EndSuccess() or not accepting)
        // will end the call-out without setting this flag to false.
        // So we know the call has been terminated abnormally, and promotes the user
        // to check the log and report the error.
        private bool terminated = true;

        protected Vector3 SpawnPoint { get; set; }
        protected abstract string Name { get; }
        protected abstract string ScannerCrimeName { get; }
        protected CalloutResponseType ResponseType { get; set; }
        protected bool ReportedByUnits { get; set; }
        protected Blip Blip { get; set; }
        protected virtual float BeforeAcceptBlipRange => 50f;

        public override bool OnBeforeCalloutDisplayed()
        {
            this.CalloutPosition = SpawnPoint;
            this.CalloutMessage = Name;

            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, BeforeAcceptBlipRange);

            string audioString = "RC_ATTENTION ";
            audioString += ReportedByUnits ? "OFFICERS_REPORT" : "CITIZENS_REPORT";
            audioString += " " + ScannerCrimeName + " IN_OR_ON_POSITION";

            Functions.PlayScannerAudioUsingPosition(audioString, SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override void OnCalloutNotAccepted()
        {
            Game.LogTrivial($"Rel.C: {Name} ending - not accepted");
            terminated = false;
            base.OnCalloutNotAccepted();
        }

        public override bool OnCalloutAccepted()
        {
            ScannerMessages.DisplayResponseCode(ResponseType);

            return base.OnCalloutAccepted();
        }

        /// <summary>
        /// Reports Code 4 to the player, then ends the <see cref="Callout"/>.
        /// </summary>
        protected void EndSuccess()
        {
            terminated = false;
            Game.LogTrivial($"Rel.C: {Name} ending - successful");
            ScannerMessages.EndCall(Name);
            End();
        }

        public override void End()
        {
            Game.LogTrivial($"Rel.C: cleaning up {Name}");
            base.End();
            if (Blip) Blip.Delete();

            if (terminated)
            {
                Game.LogTrivial($"Rel.C: !!! CALLOUT ABORTED - {Name} !!!");
                Game.DisplayNotification($"{Name} exited abnormally. Please check your log file.");
            }
        }
    }
}
