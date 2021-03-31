using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using StopThePed.API;

namespace RelaperCallouts.Extern
{
    internal static class StopThePedFunctions
    {
        internal static void SetVehicleHasNoRegistration(Vehicle vehicle)
        {
            if (ExternManager.StopThePedInstalled)
            {
                Functions.setVehicleRegistrationStatus(vehicle, STPVehicleStatus.None);
            }
        }
    }
}
