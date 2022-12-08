using RimWorld;
using System;
using Verse;
using Verse.Noise;

namespace MGRRimworld.MGRComps
{
    class HediffCompProperties_AdjustPower : HediffCompProperties
    {
        public float efficiency;

        public HediffCompProperties_AdjustPower() => this.compClass = typeof(HediffCompAdjustPower);
    }

    class HediffCompAdjustPower : HediffComp
    {
        private float power;

        private HediffCompProperties_AdjustPower Props => (HediffCompProperties_AdjustPower)this.props;

        public override string CompLabelPrefix => "x" + Power.ToString();

        public float Power { get => power; set => power = value; }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if (dinfo != null)
            {
                float pwrAbsorbed = ((DamageInfo)dinfo).Amount;
                if (pwrAbsorbed > 1)
                {
                    float morePower = (float)Math.Round((double)((float)(pwrAbsorbed / 2400.0f) * Props.efficiency), 2);
                    Log.Message("Power Absorbed from all power grids: " + morePower);
                    power += morePower;
                }
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref power, "power");
        }
    }
}
