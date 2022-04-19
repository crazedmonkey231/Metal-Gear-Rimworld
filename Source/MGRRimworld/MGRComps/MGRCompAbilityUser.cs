using AbilityUser;
using MGRRimworld.MGRDefOf;

namespace MGRRimworld.MGRComps
{
    class MGRCompAbilityUser : CompAbilityUser
    {
        public override void CompTick()
        {
            if(Initialized)
                base.CompTick();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            // add Abilities
            this.AddPawnAbility(MGRDefOf.MGRDefOf.TrueDamageAbility);
            this.AddPawnAbility(MGRDefOf.MGRDefOf.ShieldOverLoad);
        }

        public override bool TryTransformPawn()
        {
            // if true, transforms this pawn into this AbilityUser (i.e. initialize)
            // So, you can have it check for a trait before initializing 
            // by default, it returns true
            return true;
        }
    }
}
