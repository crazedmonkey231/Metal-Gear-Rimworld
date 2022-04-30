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
    
    class NanomachineCoreAbility : PawnAbility
    {

        public MGRCompAbilityUser AbilityUser => (MGRCompAbilityUser)this.AbilityUser;


        public NanomachineCoreAbility() { 
            
        }

        public override bool ShouldShowGizmo()
        {

            return true;

        }
    }
}
