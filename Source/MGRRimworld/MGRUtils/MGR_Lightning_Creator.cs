using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.Sound;
using Verse;

namespace MGRRimworld.MGRUtils
{
    [StaticConstructorOnStartup]
    internal static class MGR_Lightning_Creator
    {
        public static Mesh boltMesh;
        public static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

        public static void DoStrike(IntVec3 strikeLoc, Map map, List<Thing> thingsToIgnore = null)
        {
            SoundDefOf.Thunder_OffMap.PlayOneShotOnCamera(map);
            boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            if (!strikeLoc.Fogged(map))
            {
                GenExplosion.DoExplosion(strikeLoc, map, 1.9f, DamageDefOf.Flame, (Thing)null, ignoredThings: thingsToIgnore, postExplosionSpawnThingDef: ThingDefOf.Filth_Ash);
                Vector3 vector3Shifted = strikeLoc.ToVector3Shifted();
                for (int index = 0; index < 4; ++index)
                {
                    FleckMaker.ThrowSmoke(vector3Shifted, map, 1.5f);
                    FleckMaker.ThrowMicroSparks(vector3Shifted, map);
                    FleckMaker.ThrowLightningGlow(vector3Shifted, map, 1.5f);
                }
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(strikeLoc, map));
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
            Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, 1), 0);
        }
    }
}
