using AbilityUser;
using MGRRimworld.MGRMoteMaker;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MGRRimworld
{
    [StaticConstructorOnStartup]
    class Effect_ShieldOverload : Verb_UseAbility
    {
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

            Pawn casterPawn = ((Effect_ShieldOverload)this).CasterPawn;
            Map map = ((Effect_ShieldOverload)this).CasterPawn.Map;
            IntVec3 position = ((Effect_ShieldOverload)this).CasterPawn.Position;
            bool drafted = casterPawn.Drafted;
            if (casterPawn.IsColonist)
            {

                GenExplosion.DoExplosion(position, map, 54.0f, DamageDefOf.Bomb, (Thing)null, damAmount: 0, postExplosionSpawnThingDef: ThingDefOf.Gas_Smoke, postExplosionSpawnChance: 1f);

                DamageInfo dinfo = new DamageInfo();
                casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCore).PostAdd(null);
                return true;

            }
            return false;
        }
    }
}
