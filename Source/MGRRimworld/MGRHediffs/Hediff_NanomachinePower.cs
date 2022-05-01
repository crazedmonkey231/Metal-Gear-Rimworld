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
