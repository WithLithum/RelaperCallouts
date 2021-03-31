using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;

namespace RelaperCallouts.Util
{
    internal static class SpawnUtil
    {
        private static readonly Model[] civilianCarModels = new Model[]
        {
            "BALLER",
            "STAINER",
            "CAVALCADE",
            "GRANGER",
            "BJXL",
            "CAVALCADE2",
            "SURGE",
            "ASEA",
            "PREMIER",
            "EMPEROR",
            "SCHAFTER2"
        };

        private static readonly Model[] emergencyCarModels = new Model[]
        {
            "POLICE",
            "POLICE2",
            "POLICE3",
            "POLICE4",
            "POLICEB",
            "FBI",
            "FBI2",
            "AMBULANCE",
            "FIRETRUCK",
            "RIOT"
        };

        internal static WeaponHash GetRandomArmedRobberWeapon()
        {
            switch (MathHelper.GetRandomInteger(3))
            {
                case 0:
                    return WeaponHash.PumpShotgun;

                case 1:
                    return WeaponHash.Smg;

                default:
                    return WeaponHash.MicroSMG;
            }
        }

        internal static Vector3 GenerateSpawnPointAroundPlayer(float min, float max)
        {
            return World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(min, max));
        }

        internal static bool TryGenerateSpawnPointOnPedWalk(float min, float max, bool pavement, out Vector3 result)
        {
            var pos = Vector3.Zero;
            var streetPos = GenerateSpawnPointAroundPlayer(min, max);
            bool success = NativeFunction.Natives.GET_SAFE_COORD_FOR_PED<bool>(streetPos.X, streetPos.Y, streetPos.Z, pavement, ref pos, 16);
            result = pos;
            return success;
        }

        internal static Model GetRandomCivilianCarModel()
        {
            return civilianCarModels[MathHelper.GetRandomInteger(civilianCarModels.Length + 1)];
        }

        internal static Model GetRandomEmergencyCarModel()
        {
            return emergencyCarModels[MathHelper.GetRandomInteger(emergencyCarModels.Length + 1)];
        }
    }
}
