using Verse;

namespace MGRRimworld.MGRComps
{
    class HediffCompAdjustPower : HediffComp
    {

        private HediffCompProperties_AdjustPower Props => (HediffCompProperties_AdjustPower)this.props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
             if (dinfo != null)
            {
                float pwrAbsorbed = ((DamageInfo)dinfo).Amount;
                if (pwrAbsorbed > 1)
                {
                    Log.Message("Power Absorbed from all power grids: " + pwrAbsorbed);
                    Props.stage.capMods.ForEach(i => i.offset = pwrAbsorbed / 2400.0f);
                    Log.Message("Stage " + Props.stage);
                    //Pawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower).sourceHediffDef.stages.Clear();
                    //Pawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower).sourceHediffDef.stages.Add(Props.stage);
                }
             }
        }
    }
}
