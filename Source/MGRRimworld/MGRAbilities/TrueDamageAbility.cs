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
    
    class TrueDamageAbility : PawnAbility
    {
        public MGRCompAbilityUser AbilityUser => (MGRCompAbilityUser)this.AbilityUser;
        public TrueDamageAbility() { 
            
        }
        public override bool ShouldShowGizmo()
        {
            if (this.Pawn.equipment.Primary == null)
                return false;
            return this.Pawn.equipment.Primary.def.defName.Equals("MeleeWeapon_MGR_Katana");
        }
    }
}
