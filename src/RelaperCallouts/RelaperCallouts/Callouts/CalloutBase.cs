using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    public abstract class CalloutBase : Callout
    {
        protected Vector3 SpawnPoint { get; set; }
        protected abstract string Name { get; }
        protected abstract string ScannerCrimeName { get; }
        protected CalloutResponseType ResponseType { get; set; }
        protected bool ReportedByUnits { get; set; }

        public override bool OnBeforeCalloutDisplayed()
        {
            this.CalloutPosition = SpawnPoint;
            this.CalloutMessage = Name;

            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 50f);

            string audioString = "RC_ATTENTION ";
            audioString += ReportedByUnits ? "OFFICERS_REPORT" : "CITIZENS_REPORT";
            audioString += " " + ScannerCrimeName + " IN_OR_ON_POSITION";

            Functions.PlayScannerAudioUsingPosition(audioString, SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            ScannerMessages.DisplayResponseCode(ResponseType);

            return base.OnCalloutAccepted();
        }

        protected void EndSuccess()
        {
            ScannerMessages.EndCall(Name);
            End();
        }
    }
}
