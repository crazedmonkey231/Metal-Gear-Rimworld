using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MGRRimworld.MGRHediffs
{
    class Hediff_DuelistObjectifier : HediffWithComps
    {
        public override string Label => def.label;
        public override bool ShouldRemove => false;

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

            base.PostAdd(null);

        }

        public override void PostRemoved()
        {

            base.PostRemoved();

        }
    }
}
