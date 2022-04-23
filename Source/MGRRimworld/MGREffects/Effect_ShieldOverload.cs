using AbilityUser;
using RimWorld;
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

            Pawn casterPawn = this.CasterPawn;
            Map map = this.CasterPawn.Map;
            if (casterPawn.IsColonist)
            {

                if (map.GameConditionManager.GetActiveCondition(GameConditionDefOf.SolarFlare) == null)
                {

                    map.GameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration: 3000));

                }

                casterPawn.health.hediffSet.GetFirstHediffOfDef(MGRDefOf.MGRDefOf.NanomachineCore).PostAdd(null);

                return true;

            }
            return false;
        }
    }
}
