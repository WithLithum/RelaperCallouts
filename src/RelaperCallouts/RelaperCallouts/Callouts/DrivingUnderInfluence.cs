using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RelaperCallouts.Callouts.Framework;
using RelaperCallouts.Util;

namespace RelaperCallouts.Callouts
{
    [CalloutInfo("DUI RC", CalloutProbability.High)]
    public class DrivingUnderInfluence : CalloutBase
    {
        private Vehicle vehicle;
        private Ped driver;

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


            return base.OnCalloutAccepted();
        }
    }
}
