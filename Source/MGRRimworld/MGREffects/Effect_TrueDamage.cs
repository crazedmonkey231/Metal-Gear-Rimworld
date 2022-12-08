using MGRRimworld.MGRMoteMaker;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace MGRRimworld
{
    [StaticConstructorOnStartup]
    class Effect_TrueDamage : Verb_LaunchProjectile
    {
        private Pawn sourcePawn;
        private Map sourceMap;
        private bool casterIsColonist;
        private IntVec3 targetLocation;
        private bool targetLocationIsValid;

        private static readonly Material bladeMat = MaterialPool.MatFrom("UI/BloodMist", false);

        public override bool Available()
        {
            if (CasterIsPawn)
            {
                Pawn casterPawn = CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && !verbProps.ai_ProjectileLaunchingIgnoresMeleeThreats && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            return base.CanHitTargetFrom(root, targ);
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return base.ValidateTarget(target, showMessages) && target.HasThing && target.Thing.Faction != Faction.OfPlayer && target.Thing.Faction.HostileTo(Faction.OfPlayer);
        }

        protected override bool TryCastShot()
        {
            if (!currentTarget.HasThing || currentTarget.Thing.Map != caster.Map || !CurrentTarget.Pawn.HostileTo(CasterPawn))
                return false;

            Effect_TrueDamage effectTrueDamage = this;
            sourcePawn = effectTrueDamage.CasterPawn;
            sourceMap = sourcePawn.Map;
            casterIsColonist = effectTrueDamage.CasterPawn.IsColonist;
            targetLocation = effectTrueDamage.CurrentTarget.Cell;
            targetLocationIsValid = targetLocation.IsValid;

            if (targetLocationIsValid && casterIsColonist)
            {
                CreateEffect(sourcePawn, FleckDefOf.ExplosionFlash, new Vector3(-0.5f, 0, -0.5f), 3);
                sourcePawn.Position = targetLocation;
                SearchForTargets(targetLocation, 3, sourceMap, sourcePawn);
                return true;
            }
            return false;
        }

        private static void CreateEffect(Pawn sourcePawn, FleckDef fleckDef, Vector3 offset, int scale = 1)
        {
            FleckCreationData dataAttachedOverlay = FleckMaker.GetDataAttachedOverlay(sourcePawn, fleckDef, offset, scale);
            dataAttachedOverlay.link.detachAfterTicks = 5;
            sourcePawn.Map.flecks.CreateFleck(dataAttachedOverlay);
        }

        public void SearchForTargets(IntVec3 center, float radius, Map map, Pawn pawn)
        {
            Pawn victim = (Pawn)null;
            IEnumerable<IntVec3> source = GenRadial.RadialCellsAround(center, radius, true);
            DrawBlade(center.ToVector3(), 1);
            foreach (IntVec3 target in source)
            {
                FleckMaker.ThrowDustPuff(target, map, 0.2f);
                if (target.InBounds(map) && target.IsValid)
                    victim = target.GetFirstPawn(map);
                if (victim != null && victim.HostileTo(CasterPawn))
                {
                    int dmgNum = GetWeaponDmg(pawn);
                    Vector3 location = victim.Position.ToVector3();
                    if (Rand.Chance((float)(0.05 + 0.15 * pawn.skills.GetSkill(SkillDefOf.Melee).levelInt) / 2))
                    {
                        dmgNum *= 10;
                        MoteMaker.ThrowText(victim.DrawPos, victim.Map, "Critical Hit");
                    }
                    MGR_MoteMaker.ThrowCrossStrike(location, map, 1f);
                    MGR_MoteMaker.ThrowBloodSquirt(location, map, 1.5f);
                    DamageEntities(pawn, victim, null, dmgNum, DamageDefOf.Cut);
                }
            }
        }

        private void DrawBlade(Vector3 center, float magnitude)
        {
            Vector3 pos = center;
            pos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            Vector3 s = new Vector3(magnitude, magnitude, 1.5f * magnitude);
            Matrix4x4 matrix = new Matrix4x4();
            for (int index = 0; index < 6; ++index)
            {
                float angle = (float)Rand.Range(0, 360);
                matrix.SetTRS(pos, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, bladeMat, 0);
            }
        }

        public void DamageEntities(Pawn instigatorPawn, Pawn victim, BodyPartRecord hitPart, int amt, DamageDef type)
        {
            amt = (int)((double)amt * (double)Rand.Range(1f, 3f));
            DamageInfo dinfo = new DamageInfo(type, (float)amt, 100, instigator: instigatorPawn, hitPart: hitPart);
            dinfo.SetAllowDamagePropagation(false);
            try { victim.TakeDamage(dinfo); }
            catch { }
        }

        public static int GetWeaponDmg(Pawn pawn)
        {
            ThingWithComps primary = pawn.equipment.Primary;
            float num = primary.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS, false) * 0.7f;
            float statValue1 = primary.GetStatValue(StatDefOf.MeleeWeapon_DamageMultiplier, false);
            float statValue2 = pawn.GetStatValue(StatDefOf.MeleeDPS, false);
            return Mathf.RoundToInt((float)((double)(0.6f) * (double)statValue1 * ((double)statValue2 + (double)num)));
        }

    }
}
