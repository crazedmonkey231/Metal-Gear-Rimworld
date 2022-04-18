using MGRRimworld.MGRDefOf;
using UnityEngine;
using Verse;

namespace MGRRimworld.MGRMoteMaker
{
    public static class MGR_MoteMaker
    {
        public static void ThrowGenericMote(
          ThingDef moteDef,
          Vector3 loc,
          Map map,
          float scale,
          float solidTime,
          float fadeIn,
          float fadeOut,
          int rotationRate,
          float velocity,
          float velocityAngle,
          float lookAngle)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
                return;
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef);
            moteThrown.Scale = 1.9f * scale;
            moteThrown.rotationRate = (float)rotationRate;
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity(velocityAngle, velocity);
            moteThrown.exactRotation = lookAngle;
            moteThrown.def.mote.solidTime = solidTime;
            moteThrown.def.mote.fadeInTime = fadeIn;
            moteThrown.def.mote.fadeOutTime = fadeOut;
            GenSpawn.Spawn((Thing)moteThrown, loc.ToIntVec3(), map);
        }

        public static void ThrowGenericFleck(
          FleckDef fleckDef,
          Vector3 loc,
          Map map,
          float scale,
          float solidTime,
          float fadeIn,
          float fadeOut,
          int rotationRate,
          float velocity,
          float velocityAngle,
          float lookAngle)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
                return;
            map.flecks.CreateFleck(new FleckCreationData()
            {
                def = fleckDef,
                scale = scale,
                spawnPosition = loc,
                rotationRate = (float)rotationRate,
                velocityAngle = velocityAngle,
                velocitySpeed = velocity,
                solidTimeOverride = new float?(solidTime),
                rotation = lookAngle
            });
        }

        public static Mote MakeOverlay(
          Thing target,
          ThingDef moteDef,
          Map map,
          Vector3 offset,
          float scale,
          float lookAngle,
          float fadeIn,
          float fadeOut,
          float solidTimeOverride,
          float growthRate)
        {
            Mote mote = (Mote)ThingMaker.MakeThing(moteDef);
            mote.Attach((TargetInfo)target);
            mote.Scale = scale;
            mote.exactPosition = target.DrawPos + offset;
            mote.exactRotation = lookAngle;
            mote.def.mote.fadeInTime = fadeIn;
            mote.def.mote.fadeOutTime = fadeOut;
            mote.def.mote.growthRate = growthRate;
            mote.solidTimeOverride = solidTimeOverride;
            GenSpawn.Spawn((Thing)mote, target.Position, map);
            return mote;
        }

        public static Mote MakeOverlay(
          TargetInfo target,
          ThingDef moteDef,
          Map map,
          Vector3 offset,
          float scale,
          float lookAngle,
          float fadeIn,
          float fadeOut,
          float solidTimeOverride,
          float growthRate)
        {
            Mote mote = (Mote)ThingMaker.MakeThing(moteDef);
            mote.Attach(target);
            mote.Scale = scale;
            mote.exactPosition = target.CenterVector3 + offset;
            mote.exactRotation = lookAngle;
            mote.def.mote.fadeInTime = fadeIn;
            mote.def.mote.fadeOutTime = fadeOut;
            mote.def.mote.growthRate = growthRate;
            mote.solidTimeOverride = solidTimeOverride;
            GenSpawn.Spawn((Thing)mote, target.Cell, map);
            return mote;
        }

        public static void ThrowCrossStrike(Vector3 loc, Map map, float scale)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.Saturated)
                return;
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(MGRDefOf.MGRDefOf.MGR_Mote_CrossStrike);
            moteThrown.Scale = 1.9f * scale;
            moteThrown.rotationRate = 0.0f;
            moteThrown.exactRotation = (float)Rand.Range(0, 3);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(0, 360), Rand.Range(0.05f, 0.1f));
            GenSpawn.Spawn((Thing)moteThrown, loc.ToIntVec3(), map);
        }

        public static void ThrowBloodSquirt(Vector3 loc, Map map, float scale)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
                return;
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(MGRDefOf.MGRDefOf.MGR_Mote_BloodSquirt);
            moteThrown.Scale = 1.9f * scale;
            moteThrown.rotationRate = (float)Rand.Range(-60, 60);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(0, 360), Rand.Range(1f, 2f));
            GenSpawn.Spawn((Thing)moteThrown, loc.ToIntVec3(), map);
        }
    }
}
