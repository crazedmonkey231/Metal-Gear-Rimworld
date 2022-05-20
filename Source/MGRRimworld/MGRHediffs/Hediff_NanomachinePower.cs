using MGRRimworld.MGRComps;
using RimWorld;
using System;
using System.Text;
using Verse;

namespace MGRRimworld.MGRHediffs
{
    class Hediff_NanomachinePower : HediffWithComps
    {

        public override void ExposeData()
        {

            base.ExposeData();

        }

        public override void Tick()
        {

            base.Tick();


        }

        public override void PostAdd(DamageInfo? dinfo)
        {

            if (dinfo != null)
            {
                float pwrAbsorbed = ((DamageInfo)dinfo).Amount;
                if (pwrAbsorbed > 1) {
                    Log.Message("Power Absorbed from all power grids: " + pwrAbsorbed);
                    //this.Severity += pwrAbsorbed;
                    this.TryGetComp<HediffCompAdjustPower>().CompPostPostAdd(dinfo);
                }
            }

            base.PostAdd(null);
        }
        public override void PostRemoved()
        {

            base.PostRemoved();

        }
    }
}
