using MGRApparel.MGRGizmo;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MGRRimworld.MGRComps
{
    class CompItemAddAbility : CompCauseHediff_Apparel

    {
        public CompProperties_ItemAddAbility Props => (CompProperties_ItemAddAbility)this.props;

        public override void Notify_Equipped(Pawn pawn)
        {

            pawn.health.AddHediff(this.Props.hediff);

        }

        public override void Notify_Unequipped(Pawn pawn)
        {

            pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff));

        }

    }
}
