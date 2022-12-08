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
        public override HediffStage CurStage
        {
            get
            {
                return new HediffStage() 
                { 
                    capMods = new System.Collections.Generic.List<PawnCapacityModifier> 
                    {
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Consciousness,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Moving,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Sight,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Breathing,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Hearing,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Talking,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Eating,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Manipulation,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.BloodPumping,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.BloodFiltration,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        },
                        new PawnCapacityModifier()
                        {
                            capacity = PawnCapacityDefOf.Metabolism,
                            offset = this.TryGetComp<HediffCompAdjustPower>().Power,
                        }
                    } 
                };
            }
        }

        public override bool TryMergeWith(Hediff other)
        {
            for (int i = 0; i < comps.Count; i++)
            {
                comps[i].CompPostMerged(other);
            }

            return true;
        }

        public override void PostAdd(DamageInfo? dinfo)
        {

            if (dinfo != null)
            {
                float pwrAbsorbed = ((DamageInfo)dinfo).Amount;
                if (pwrAbsorbed > 1) {
                    Log.Message("Power Absorbed from all power grids: " + pwrAbsorbed);
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
