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

                GenExplosion.DoExplosion(position, map, 54.0f, DamageDefOf.Bomb, (Thing)null, postExplosionSpawnThingDef: ThingDefOf.Gas_Smoke, postExplosionSpawnChance: 1f);
                return true;

            }
            return false;
        }

        public void SearchForTargets(IntVec3 center, float radius, Map map, Pawn pawn)
        {
/*            Pawn victim = (Pawn)null;
            IEnumerable<IntVec3> source = GenRadial.RadialCellsAround(center, radius, true);
            for (int index = 0; index < source.Count<IntVec3>(); ++index)
            {
                IntVec3 intVec3 = source.ToArray<IntVec3>()[index];
                FleckMaker.ThrowDustPuff(intVec3, map, 0.2f);
                if (intVec3.InBounds(map) && intVec3.IsValid)
                    victim = intVec3.GetFirstPawn(map);
*//*                if (victim != null)
                {
                    this.DrawStrike(center, victim.Position.ToVector3(), map);
                    this.damageEntities(victim, (BodyPartRecord)null, this.dmgNum, DamageDefOf.Cut);
                }*//*
                source.GetEnumerator().MoveNext();
            }*/
        }
    }
}
