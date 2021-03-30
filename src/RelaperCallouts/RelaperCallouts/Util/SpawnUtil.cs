using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

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

        internal static Vector3 GenerateSpawnPointAroundPlayer(float min, float max)
        {
            return World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(min, max));
        }

        internal static Model GetRandomCivilianCarModel()
        {
            return civilianCarModels[MathHelper.GetRandomInteger(civilianCarModels.Length + 1)];
        }
    }
}
