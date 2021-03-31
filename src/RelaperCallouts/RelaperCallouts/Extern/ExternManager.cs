using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using Rage;

namespace RelaperCallouts.Extern
{
    internal static class ExternManager
    {
        internal static bool StopThePedInstalled { get; private set; }

        internal static void Init()
        {
            Game.LogTrivial("Rel.C: Initializing external plugins");
            var plugins = Functions.GetAllUserPlugins();
            foreach (var plugin in plugins)
            {
                switch (plugin.GetName().Name)
                {
                    case "StopThePed":
                        Game.LogTrivial("Rel.C: Found Stop The Ped");
                        StopThePedInstalled = true;
                        break;
                }
            }
        }
    }
}
