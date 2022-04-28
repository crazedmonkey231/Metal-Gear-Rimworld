using AbilityUser;
using MGRRimworld.MGRComps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MGRRimworld.MGRHediffs
{
    class Hediff_NanomachinePower : HediffWithComps
    {

        public override void ExposeData()
        {

            base.ExposeData();
            if (Scribe.mode != LoadSaveMode.PostLoadInit || this.Part != null)
                return;
            Log.Error(this.GetType().Name + " has null part after loading.");
            this.pawn.health.hediffSet.hediffs.Remove((Hediff)this);

        }

        public override void Tick()
        {

            base.Tick();


        }

        public override void PostAdd(DamageInfo? dinfo)
        {

            if(dinfo != null)
            {

                float pwrAbsorbed = ((DamageInfo)dinfo).Amount;
                Log.Message("Power Absorbed from all power grids: " + pwrAbsorbed);
                Log.Message("Added Power");
                this.Severity += pwrAbsorbed;

            }

            base.PostAdd(null);
        }

        public override void PostRemoved()
        {

            base.PostRemoved();

        }
    }
}
