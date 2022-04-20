using AbilityUser;
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
    class Effect_TrueDamage : Verb_UseAbility
    {

        private int verVal = 5;
        private int dmgNum = 100;
        private bool validTarg;
        private IntVec3 arg_29_0;
        private bool arg_41_0;
        private bool arg_42_0;
        private static readonly Material bladeMat = MaterialPool.MatFrom("UI/BloodMist", false);

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            this.validTarg = targ.IsValid && targ.CenterVector3.InBounds((((Effect_TrueDamage)this).CasterPawn).Map) && !targ.Cell.Fogged((((Effect_TrueDamage)this).CasterPawn).Map) && targ.Cell.Walkable((((Effect_TrueDamage)this).CasterPawn).Map) && (double)(root - targ.Cell).LengthHorizontal < (double)((Effect_TrueDamage)this).verbProps.range;
            return this.validTarg;
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


            if (((Effect_TrueDamage)this).CasterPawn.equipment.Primary != null)
            {

                bool flag2;
                if (((Effect_TrueDamage)this).currentTarget != (LocalTargetInfo)(Thing)null && (((Effect_TrueDamage)this).CasterPawn) != null)
                {

                    this.arg_29_0 = ((Effect_TrueDamage)this).currentTarget.Cell;
                    Vector3 centerVector3 = ((Effect_TrueDamage)this).currentTarget.CenterVector3;
                    flag2 = ((Effect_TrueDamage)this).currentTarget.Cell.IsValid;
                    this.arg_41_0 = centerVector3.InBounds((((Effect_TrueDamage)this).CasterPawn).Map);
                    this.arg_42_0 = true;
                }
                else
                    flag2 = false;
                bool flag3 = flag2;
                bool flag4 = this.arg_41_0;
                bool flag5 = this.arg_42_0;
                if (flag3)
                {
                    if (flag4 & flag5)
                    {
                        Pawn casterPawn = ((Effect_TrueDamage)this).CasterPawn;
                        Map map = ((Effect_TrueDamage)this).CasterPawn.Map;
                        IntVec3 position = ((Effect_TrueDamage)this).CasterPawn.Position;
                        bool drafted = casterPawn.Drafted;
                        if (casterPawn.IsColonist)
                        {
                            try
                            {
                                ThingSelectionUtility.SelectNextColonist();
                                ((Effect_TrueDamage)this).CasterPawn.DeSpawn(DestroyMode.WillReplace);
                                this.SearchForTargets(this.arg_29_0, 2f + 0.5f * (float)this.verVal, map, casterPawn);
                                GenSpawn.Spawn(casterPawn, this.currentTarget.Cell, map);
                                this.DrawBlade(casterPawn.Position.ToVector3Shifted(), 4f + (float)this.verVal);
                                casterPawn.drafter.Drafted = drafted;
                                CameraJumper.TryJumpAndSelect((GlobalTargetInfo)(Thing)casterPawn);
                                MGR_MoteMaker.ThrowGenericMote(MGRDefOf.MGRDefOf.MGR_Mote_BladeSweep, ((Effect_TrueDamage)this).CasterPawn.DrawPos, ((Effect_TrueDamage)this).CasterPawn.Map, (float)(1.39999997615814 + 0.400000005960464), 0.04f, 0.0f, 0.18f, 1000, 0.0f, 0.0f, (float)Rand.Range(0, 360));
                            }
                            catch
                            {
                                if (!((Effect_TrueDamage)this).CasterPawn.Spawned)
                                {
                                    GenSpawn.Spawn((Thing)casterPawn, position, map);
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
                            this.SearchForTargets(this.arg_29_0, 2f + 0.5f * (float)this.verVal, map, casterPawn);
                            GenSpawn.Spawn((Thing)casterPawn, ((Effect_TrueDamage)this).currentTarget.Cell, map);
                            this.DrawBlade(casterPawn.Position.ToVector3Shifted(), 4f + (float)this.verVal);
                        }
                        flag1 = true;
                    }
                    else
                        Messages.Message((string)"InvalidTargetLocation".Translate(), MessageTypeDefOf.RejectInput);
                }
              ((Effect_TrueDamage)this).burstShotsLeft = 0;
                return flag1;
            }
            Messages.Message((string)"MustHaveMeleeWeapon".Translate((NamedArgument)((Effect_TrueDamage)this).CasterPawn.LabelCap), MessageTypeDefOf.RejectInput);
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
                if (victim != null )//&& victim.Faction != pawn.Faction)
                {
                    if (Rand.Chance((float)(0.100000001490116 + 0.150000005960464 * pawn.skills.GetSkill(SkillDefOf.Melee).levelInt)))
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
