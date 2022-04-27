using AbilityUser;
using RimWorld;
using System;
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
                    map.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration: 3000));
                }

                DamageInfo dinfo = new DamageInfo();
                dinfo.SetAmount(RemoveAllMapBatteriesCharge());

                if (casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower) != null)
                    casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower).PostAdd(dinfo);
                else
                    casterPawn.health.AddHediff(MGRDefOf.MGRDefOf.NanomachineCorePower, dinfo: dinfo);

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
    }
}
