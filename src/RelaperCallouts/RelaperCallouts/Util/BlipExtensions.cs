using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;

namespace RelaperCallouts.Util
{
    public static class BlipExtensions
    {
        public static void SetColor(this Blip blip, BlipColor blipColor)
        {
            NativeFunction.Natives.SET_BLIP_COLOUR(blip, (int)blipColor);
        }

        public static void SetRouteColor(this Blip blip, BlipColor blipColor)
        {
            NativeFunction.Natives.SET_BLIP_ROUTE_COLOUR(blip, (int) blipColor);
        }
    }
}
