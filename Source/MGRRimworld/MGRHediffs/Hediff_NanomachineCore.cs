using Verse;

namespace MGRRimworld.MGRHediffs
{
    class Hediff_NanomachineCore : HediffWithComps
    {
        public override string Label => def.label;
        public override bool ShouldRemove => false;

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

            base.PostAdd(null);

        }

        public override void PostRemoved()
        {

            base.PostRemoved();

        }
    }
}
