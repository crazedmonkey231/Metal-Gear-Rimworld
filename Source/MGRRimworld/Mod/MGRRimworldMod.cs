using UnityEngine;
using Verse;

namespace MGRRimworld
{
    class MGRRimworldMod : Mod
    {
        public static MGRRimworldSettings settings;

        public MGRRimworldMod(ModContentPack content) : base(content)
        {
            MGRRimworldMod.settings = this.GetSettings<MGRRimworldSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);



            base.DoSettingsWindowContents(inRect);
            MGRRimworldMod.settings.Write();
        }

        public override string SettingsCategory()
        {
            return "MGR Rimworld Settings";
        }

        public override void WriteSettings()
        {

            base.WriteSettings();
        }
    }
}
