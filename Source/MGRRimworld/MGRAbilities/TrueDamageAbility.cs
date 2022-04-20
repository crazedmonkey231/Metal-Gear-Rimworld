using AbilityUser;
using MGRRimworld.MGRComps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MGRRimworld.MGRAbilities
{
    [StaticConstructorOnStartup]
    class TrueDamageAbility : PawnAbility
    {
        private bool showGizmo = true;

        public MGRCompAbilityUser AbilityUser => (MGRCompAbilityUser)this.AbilityUser;

        public TrueDamageAbility() { 
            
        }

        public override bool ShouldShowGizmo()
        {

            return showGizmo;

        }
    }
}
