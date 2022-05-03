using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MGRRimworld.MGRComps
{

    class CompProperties_ItemAddAbility : CompProperties_CauseHediff_Apparel
    {

        public CompProperties_ItemAddAbility() => this.compClass = typeof(CompItemAddAbility);

    }
}
