using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Exceptions;
using RelaperCallouts.Util;

namespace RelaperCallouts.WorldEvents.Framework
{
    public abstract class WorldEvent
    {
        protected WorldEvent(Ped ped)
        {
            if (!ped) throw new InvalidHandleableException();

            Ped = ped;
            Blip = ped.AttachBlip();
            Blip.Sprite = BlipSprite.Enemy;
            Blip.SetColor(BlipColor.Red);
            Blip.Scale = 0.5f;
        }

        public abstract void Initialize();
        public abstract void Process();
        public virtual void End()
        {
            if (Ped && !Functions.IsPedArrested(Ped)) Ped.Dismiss();
            if (Blip) Blip.Delete();
        }

        protected Ped Ped { get; set; }
        protected Blip Blip { get; set; }
        public bool IsRunning { get; internal set; }

    }
}
