using System.Collections.Generic;
using Verse;

namespace MGRRimworld.MGRComps
{
    class HediffCompProperties_AdjustPower : HediffCompProperties
    {

        public HediffStage stage;

        public HediffCompProperties_AdjustPower() => this.compClass = typeof(HediffCompAdjustPower);

    }
}
