using MGRRimworld.MGRComps;
using MGRRimworld.MGRUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace MGRRimworld
{
    [StaticConstructorOnStartup]
    class Effect_NanomachineCore : Verb_LaunchProjectile
    {
        private Pawn casterPawn;
        private Map map;

        protected string letterTitle = "Nanomachines Unleashed";
        protected string letterLabel = "Nanomachines Unleashed";
        protected string letterText = "Nanomachine core has been unleashed, initating a solar flare.";
        protected ChoiceLetter letter;

        private List<IntVec3> targets;

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
                float power = Unleash();
                CastingEffect(power, casterPawn.Position, map, casterPawn);
                return true;

            }
            return false;
        }


        private float Unleash()
        {
            float power = 0;
            if (map.GameConditionManager.GetActiveCondition(GameConditionDefOf.SolarFlare) == null)
            {
                SendLetter();
                map.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration: 30000));
            }
            float totalEnergy = RemoveAllMapBatteriesCharge();
            if (totalEnergy > 0)
            {
                DamageInfo dinfo = new DamageInfo();
                dinfo.SetAmount(totalEnergy);
                Log.Message("Adding hediff");
                if (casterPawn.health.hediffSet.HasHediff(MGRDefOf.MGRDefOf.NanomachineCorePower))
                {
                    HediffCompAdjustPower comp = casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCorePower).TryGetComp<HediffCompAdjustPower>();
                    comp.CompPostPostAdd(dinfo);                
                }
                else
                    casterPawn.health.AddHediff(MGRDefOf.MGRDefOf.NanomachineCorePower, dinfo: dinfo);
                power = (float)Math.Round((double)((float)(totalEnergy / 600.0f) * 0.15), 2);
            }
            return power;
        }

        private float RemoveAllMapBatteriesCharge()
        {
            var pns = map.powerNetManager.AllNetsListForReading;
            float totalEnergy = 0.0f;
            pns.ForEach(i =>
            {
                if (i.batteryComps.Any(x => (double)x.StoredEnergy > 0.0))
                {
                    i.batteryComps.ForEach(j =>
                    {
                        totalEnergy += j.StoredEnergy;
                        j.DrawPower(j.StoredEnergy);
                        MGR_Lightning_Creator.DoStrike(j.parent.Position, map);
                    });
                }
            });
            return totalEnergy;
        }

        public void CastingEffect(float power, IntVec3 center, Map map, Pawn pawn)
        {
            int maxExpDepth = (int)Math.Min(Math.Max(power, 0.0f), 54.0f);
            float extraExp = 0;
            if (power >= 54)
                extraExp = power / maxExpDepth;

            int maxExtraExp = (int)Math.Min(Math.Max(extraExp, 0.0f), 3.0f);

            List<Thing> ignoreThings = new List<Thing>() { pawn };
            
            IEnumerable<IntVec3> cells = GenRadial.RadialCellsAround(center, maxExpDepth, true).Where(c => c.IsValid && c.InBounds(map) && !c.Equals(center));
            for(int i = 0; i < maxExpDepth; i++)
            {
                IntVec3 fireLoc = cells.RandomElement();
                MGR_Lightning_Creator.DoStrike(fireLoc, map, ignoreThings);
                GenExplosion.DoExplosion(fireLoc, map, 3 + maxExtraExp, DamageDefOf.Flame, pawn, 5, 35, ignoredThings: ignoreThings, postExplosionSpawnThingDef: ThingDefOf.Filth_Ash, postExplosionSpawnChance: 1, postExplosionSpawnThingCount: 1, postExplosionGasType: GasType.BlindSmoke);
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
