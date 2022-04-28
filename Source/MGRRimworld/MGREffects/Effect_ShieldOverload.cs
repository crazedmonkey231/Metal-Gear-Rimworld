using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MGRRimworld
{


[StaticConstructorOnStartup]
    class Effect_ShieldOverload : Verb_UseAbility
    {
        private Pawn casterPawn;
        private Map map;

        public override bool Available() 
        {

            return true;

        }
        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {

            return true;

        }

        protected override bool TryCastShot()
        {

            casterPawn = this.CasterPawn;
            map = this.CasterPawn.Map;

            if (casterPawn.IsColonist)
            {

                if (map.GameConditionManager.GetActiveCondition(GameConditionDefOf.SolarFlare) == null)
                {
                    map.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration: 60000));
                }

                DamageInfo dinfo = new DamageInfo();
                dinfo.SetAmount(RemoveAllMapBatteriesCharge());

                if (casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower) != null)
                {
                    casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower).PostAdd(dinfo);
                    SearchForTargets(casterPawn.Position, 2, map, casterPawn);
                }
                else
                {
                    casterPawn.health.AddHediff(MGRDefOf.MGRDefOf.NanomachineCorePower, dinfo: dinfo);
                }

                return true;

            }
            return false;
        }

        private float RemoveAllMapBatteriesCharge()
        {
            var pns = map.powerNetManager.AllNetsListForReading;
            float totalEnergy = 0.0f;
            pns.ForEach(i => {
                if(i.batteryComps.Any<CompPowerBattery>((Predicate<CompPowerBattery>)(x => (double)x.StoredEnergy > 0.0)))
                {
                    i.batteryComps.ForEach(j =>{
                        totalEnergy += j.StoredEnergy;
                        j.DrawPower(j.StoredEnergy);
                    });
                } 
            });
            return totalEnergy;
        }
        public void SearchForTargets(IntVec3 center, float radius, Map map, Pawn pawn)
        {

            IEnumerable<IntVec3> source = GenRadial.RadialCellsAround(center, radius, true);
            for (int index = 0; index < source.Count<IntVec3>(); ++index)
            {
                IntVec3 intVec3 = source.ToArray<IntVec3>()[index];
                //FleckMaker.ThrowDustPuff(intVec3, map, 0.2f);
                GenExplosion.DoExplosion(intVec3, map, radius, DamageDefOf.Bomb, pawn, damAmount: 0, postExplosionSpawnThingDef: ThingDefOf.Explosion, postExplosionSpawnChance: 0f);
                source.GetEnumerator().MoveNext();
            }
        }
    }
}
