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
    
    class ShieldOverLoadAbility : PawnAbility
    {

        public MGRCompAbilityUser AbilityUser => (MGRCompAbilityUser)this.AbilityUser;


        public ShieldOverLoadAbility() { 
            
        }

        public override bool ShouldShowGizmo()
        {

            return true;

        }
    }
}
