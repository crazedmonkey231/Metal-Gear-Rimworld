using AbilityUser;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MGRRimworld
{
    class Effect_NanomachineCore : Verb_UseAbility
    {
        private Pawn casterPawn;
        private Map map;

        protected string letterTitle = "Nanomachines Unleashed";
        protected string letterLabel = "Nanomachines Unleashed";
        protected string letterText = "Nanomachine core has been unleashed, initating solar flare.";
        protected ChoiceLetter letter;

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

                Unleash();
                CastingEffect(casterPawn.Position, 2, map, casterPawn);

                return true;

            }
            return false;
        }

        private void Unleash()
        {
            if (map.GameConditionManager.GetActiveCondition(GameConditionDefOf.SolarFlare) == null)
            {
                SendLetter();
                map.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration: 30000));
            }
            float totalEngery = RemoveAllMapBatteriesCharge();
            if (totalEngery > 0)
            {
                DamageInfo dinfo = new DamageInfo();
                dinfo.SetAmount(totalEngery);
                if (casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower) != null)
                {
                    casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower).PostAdd(dinfo);
                }
                else
                {
                    casterPawn.health.AddHediff(MGRDefOf.MGRDefOf.NanomachineCorePower, dinfo: dinfo);
                }
            }
        }

        private float RemoveAllMapBatteriesCharge()
        {
            var pns = map.powerNetManager.AllNetsListForReading;
            float totalEnergy = 0.0f;
            pns.ForEach(i =>
            {
                if (i.batteryComps.Any<CompPowerBattery>((Predicate<CompPowerBattery>)(x => (double)x.StoredEnergy > 0.0)))
                {
                    i.batteryComps.ForEach(j =>
                    {
                        totalEnergy += j.StoredEnergy;
                        j.DrawPower(j.StoredEnergy);
                    });
                }
            });
            return totalEnergy;
        }
        public void CastingEffect(IntVec3 center, float radius, Map map, Pawn pawn)
        {

            IEnumerable<IntVec3> source = GenRadial.RadialCellsAround(center, radius, true);
            for (int index = 0; index < source.Count<IntVec3>(); ++index)
            {
                IntVec3 intVec3 = source.ToArray<IntVec3>()[index];
                FleckMaker.ThrowDustPuff(intVec3, map, 0.2f);
                GenExplosion.DoExplosion(intVec3, map, radius, DamageDefOf.Smoke, pawn, damAmount: 0, postExplosionSpawnThingDef: ThingDefOf.Gas_Smoke, postExplosionSpawnChance: 0f);
                source.GetEnumerator().MoveNext();
            }
        }
        private void SendLetter()
        {
            this.letter = (ChoiceLetter)LetterMaker.MakeLetter((TaggedString)this.letterLabel, (TaggedString)this.letterText, LetterDefOf.NeutralEvent);
            this.letter.title = this.letterTitle;
            this.letter.lookTargets = new LookTargets((Thing)this.CasterPawn);
            Find.LetterStack.ReceiveLetter((Letter)this.letter);
        }
    }
}
