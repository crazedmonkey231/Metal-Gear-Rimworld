using AbilityUser;
using MGRRimworld.MGRAbilities;
using MGRRimworld.MGRMoteMaker;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MGRRimworld
{
    [StaticConstructorOnStartup]
    class Effect_ShieldOverload : Verb_UseAbility
    {
        private bool validTarg;

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            if (targ.Thing != null && targ.Thing == ((Verb)this).caster)
                return ((Verb)this).verbProps.targetParams.canTargetSelf;
            this.validTarg = targ.IsValid && targ.CenterVector3.InBounds((((Verb)this).CasterPawn).Map) && !targ.Cell.Fogged((((Verb)this).CasterPawn).Map) && targ.Cell.Walkable((((Verb)this).CasterPawn).Map) && ((double)(root - targ.Cell).LengthHorizontal < (double)((Verb)this).verbProps.range && ((Verb)this).TryFindShootLineFromTo(root, targ, out ShootLine _));
            return this.validTarg;
        }


    }
}
