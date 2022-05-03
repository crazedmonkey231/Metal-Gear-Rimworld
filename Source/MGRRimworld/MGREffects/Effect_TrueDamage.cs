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
    class Effect_TrueDamage : Verb_LaunchProjectile
    {

        private int verVal = 5;
        private int dmgNum = 100;
        private IntVec3 center;
        private bool isInBounds;
        private bool flag1;
        private static readonly Material bladeMat = MaterialPool.MatFrom("UI/BloodMist", false);

        public override bool Available()
        {

            return true;

        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {

            return true;

        }

        public static int GetWeaponDmg(Pawn pawn)
        {
            ThingWithComps primary = pawn.equipment.Primary;
            float num = primary.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS, false) * 0.7f;
            float statValue1 = primary.GetStatValue(StatDefOf.MeleeWeapon_DamageMultiplier, false);
            float statValue2 = pawn.GetStatValue(StatDefOf.MeleeDPS, false);
            return Mathf.RoundToInt((float)((double)(0.6f) * (double)statValue1 * ((double)statValue2 + (double)num)));
        }

        protected override bool TryCastShot()
        {
            bool flag1 = false;

            if (((Effect_TrueDamage)this).CasterPawn.equipment.Primary != null && this.CasterPawn.IsColonist)
            {
                bool flag2;
                if (((Effect_TrueDamage)this).currentTarget != (LocalTargetInfo)(Thing)null && (((Effect_TrueDamage)this).CasterPawn) != null)
                {
                    this.center = ((Effect_TrueDamage)this).currentTarget.Cell;
                    Vector3 centerVector3 = ((Effect_TrueDamage)this).currentTarget.CenterVector3;
                    flag2 = ((Effect_TrueDamage)this).currentTarget.Cell.IsValid;
                    this.isInBounds = centerVector3.InBounds((((Effect_TrueDamage)this).CasterPawn).Map);
                    this.flag1 = true;
                }
                else
                    flag2 = false;
                bool flag3 = flag2;
                bool flag4 = this.isInBounds;
                bool flag5 = this.flag1;
                if (flag3)
                {
                    if (flag4 & flag5)
                    {
                        Pawn casterPawn = ((Effect_TrueDamage)this).CasterPawn;
                        Map map = ((Effect_TrueDamage)this).CasterPawn.Map;
                        IntVec3 originalPosition = ((Effect_TrueDamage)this).CasterPawn.Position;
                        bool drafted = casterPawn.Drafted;
                        if (casterPawn.IsColonist)
                        {
                            try
                            {
                                Log.Message("Can hit target");
                                ThingSelectionUtility.SelectNextColonist();
                                ((Effect_TrueDamage)this).CasterPawn.DeSpawn(DestroyMode.WillReplace);
                                this.SearchForTargets(this.center, 3, map, casterPawn);
                                GenSpawn.Spawn(casterPawn, this.currentTarget.Cell, map);
                                this.DrawBlade(this.CasterPawn.Position.ToVector3Shifted(), 3);
                                casterPawn.drafter.Drafted = drafted;
                                MGR_MoteMaker.ThrowGenericMote(MGRDefOf.MGRDefOf.MGR_Mote_BladeSweep, ((Effect_TrueDamage)this).CasterPawn.DrawPos, ((Effect_TrueDamage)this).CasterPawn.Map, (float)(1.39999997615814 + 0.400000005960464), 0.04f, 0.0f, 0.18f, 1000, 0.0f, 0.0f, (float)Rand.Range(0, 360));
                            }
                            catch
                            {
                                if (!((Effect_TrueDamage)this).CasterPawn.Spawned)
                                {
                                    GenSpawn.Spawn((Thing)casterPawn, originalPosition, map);
                                    casterPawn.drafter.Drafted = true;
                                    Log.Message("Threw exception - despawned pawn has been recovered at casting location");
                                }
                            }
                        }
                        else
                        {
                            IntVec3 location = new IntVec3(1, 1, 1);

                            location = location + ((Effect_TrueDamage)this).currentTarget.Pawn.Position - ((Effect_TrueDamage)this).CurrentDestination.Pawn.Position;


                            ((Effect_TrueDamage)this).CasterPawn.DeSpawn(DestroyMode.Vanish);
                            this.SearchForTargets(center, 3, map, casterPawn);
                            GenSpawn.Spawn((Thing)casterPawn, ((Effect_TrueDamage)this).currentTarget.Cell, map);
                            this.DrawBlade(casterPawn.Position.ToVector3Shifted(), 4f + (float)this.verVal);
                        }
                        flag1 = true;
                    }
                    else
                        Messages.Message((string)"InvalidTargetLocation".Translate(), MessageTypeDefOf.RejectInput);
                }
                ((Effect_TrueDamage)this).burstShotsLeft = 0;
                CameraJumper.TryJumpAndSelect((GlobalTargetInfo)(Thing)CasterPawn);
                return flag1;
            }
            return false;
        }

        public void SearchForTargets(IntVec3 center, float radius, Map map, Pawn pawn)
        {
            Pawn victim = (Pawn)null;
            IEnumerable<IntVec3> source = GenRadial.RadialCellsAround(center, radius, true);
            for (int index = 0; index < source.Count<IntVec3>(); ++index)
            {
                IntVec3 intVec3 = source.ToArray<IntVec3>()[index];
                FleckMaker.ThrowDustPuff(intVec3, map, 0.2f);
                if (intVec3.InBounds(map) && intVec3.IsValid)
                    victim = intVec3.GetFirstPawn(map);
                if (victim != null && victim.Faction != pawn.Faction && !victim.Faction.HostileTo(pawn.Faction))
                {
                    if (Rand.Chance((float)(0.05 + 0.15 * pawn.skills.GetSkill(SkillDefOf.Melee).levelInt)/2))
                    {
                        this.dmgNum *= 10;
                        MoteMaker.ThrowText(victim.DrawPos, victim.Map, "Critical Hit");
                    }
                    this.DrawStrike(center, victim.Position.ToVector3(), map);
                    Log.Message("searching for targets to damage");
                    this.damageEntities(pawn, victim, (BodyPartRecord)null, this.dmgNum, DamageDefOf.Cut);
                }
                source.GetEnumerator().MoveNext();
            }
        }

        public void DrawStrike(IntVec3 center, Vector3 strikePos, Map map)
        {
            MGR_MoteMaker.ThrowCrossStrike(strikePos, map, 1f);
            MGR_MoteMaker.ThrowBloodSquirt(strikePos, map, 1.5f);
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
                Graphics.DrawMesh(MeshPool.plane10, matrix, Effect_TrueDamage.bladeMat, 0);
            }
        }

        public void damageEntities(Pawn instigatorPawn, Pawn victim, BodyPartRecord hitPart, int amt, DamageDef type)
        {

            amt = (int)((double)amt * (double)Rand.Range(1f, 3f));
            DamageInfo dinfo = new DamageInfo(type, (float)amt, this.dmgNum, instigator: instigatorPawn, hitPart: hitPart);
            dinfo.SetAllowDamagePropagation(false);
            try { victim.TakeDamage(dinfo); }
            catch { }
        }

    }
}
