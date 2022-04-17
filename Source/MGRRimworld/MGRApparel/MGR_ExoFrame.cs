﻿using MGRApparel.MGRGizmo;
using MGRRimworld;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MGRApparel
{
    class MGR_ExoFrame : Apparel
    {
        private float energy;
        private int ticksToReset = -1;
        private int lastKeepDisplayTick = -9999;
        private Vector3 impactAngleVect;
        private int lastAbsorbDamageTick = -9999;
        private const float MinDrawSize = 1.2f;
        private const float MaxDrawSize = 1.55f;
        private const float MaxDamagedJitterDist = 0.05f;
        private const int JitterDurationTicks = 8;
        private int StartingTicksToReset = 3200;
        private float EnergyOnReset = 0.2f;
        private float EnergyLossPerDamage = 0.033f;
        private int KeepDisplayingTicks = 1000;
        private float ApparelScorePerEnergyMax = 0.25f;

        private bool canFire = true;



        //private static readonly Material BubbleMat = MaterialPool.MatFrom("Shields/shield_a", ShaderDatabase.Transparent);

        private float EnergyMax => this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);

        private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;

        public float Energy => energy;

        public CompExplosive compExplosive => this.GetComp<CompExplosive>();


        public ShieldState ShieldState
        {
            get
            {
                if (ticksToReset > 0)
                {
                    return ShieldState.Resetting;
                }
                return ShieldState.Active;
            }
        }

        private bool ShouldDisplay
        {
            get
            {
                Pawn wearer = this.Wearer;
                if (!wearer.Spawned || wearer.Dead || wearer.Downed)
                    return false;
                return wearer.InAggroMentalState || wearer.Drafted || wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref energy, "energy", 0f, false);
            Scribe_Values.Look(ref ticksToReset, "ticksToReset", -1, false);
            Scribe_Values.Look(ref lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            if (Find.Selector.SingleSelectedThing != base.Wearer)
            {
                yield break;
            }
            yield return (Gizmo)new MGR_Gizmo_ExoFrame
            {
                exoFrame = this
            };
            ;
        }

        public override float GetSpecialApparelScoreOffset()
        {
            return EnergyMax * ApparelScorePerEnergyMax;
        }

        public override void Tick()
        {
            base.Tick();
            if(energy == EnergyMax)
            {
                canFire = true;
            }
            if (base.Wearer == null)
            {
                energy = 0f;
            }
            else if (ShieldState == ShieldState.Resetting)
            {
                ticksToReset--;
                if (ticksToReset <= 0)
                {
                    Reset();
                }
            }
            else if (ShieldState == ShieldState.Active)
            {
                energy += EnergyGainPerTick;
                if (energy > EnergyMax)
                {
                    energy = EnergyMax;
                }
            }
        }

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (ShieldState == ShieldState.Resetting)
            {
                return false;
            }
            if (dinfo.Def == DamageDefOf.EMP)
            {
                energy = 0f;
                Break();
                return false;
            }

            energy -= dinfo.Amount * EnergyLossPerDamage;
            energy = Math.Min(Math.Max(energy, 0), this.EnergyMax);
            if (energy == 0 && canFire)
            {
                List<Thing> things = new List<Thing>();
                things.Add(this);
                things.Add(this.Wearer);

                GenSpawn.Spawn(ThingDefOf.Shell_HighExplosive, this.Wearer.DutyLocation(), dinfo.Instigator.Map);

/*              this.compExplosive.destroyedThroughDetonation = false;
                this.compExplosive.AddThingsIgnoredByExplosion(things);
                this.def.destroyable = false;
                this.compExplosive.StartWick(dinfo.Instigator);*/
                Break();
            }else if (energy > 0)
            {
                AbsorbedDamage(dinfo);
            }
            return true;
        }

        public void KeepDisplaying()
        {
            lastKeepDisplayTick = Find.TickManager.TicksGame;
        }

        private void AbsorbedDamage(DamageInfo dinfo)
        {

            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = base.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            FleckMaker.Static(loc, base.Wearer.Map, FleckDefOf.ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                FleckMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
            this.KeepDisplaying();

        }

        private void Break()
        {
            SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
            FleckMaker.Static(base.Wearer.TrueCenter(), base.Wearer.Map, FleckDefOf.ExplosionFlash, 12f);
            for (int i = 0; i < 6; i++)
            {
                Vector3 loc = base.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
                FleckMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
            }
            energy = 0f;
            ticksToReset = StartingTicksToReset;
        }

        private void Reset()
        {
            if (base.Wearer.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
                FleckMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
            }
            ticksToReset = -1;
            energy = EnergyOnReset;
            canFire = false;
        }

        public override void DrawWornExtras()
        {
/*            if (ShouldDisplay)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, energy);
                Vector3 vector = base.Wearer.Drawer.DrawPos;
                vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                if (num2 < 8)
                {
                    float num3 = (float)(8 - num2) / 8f * 0.05f;
                    vector += impactAngleVect * num3;
                    num -= num3;
                }
                float angle = (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);

                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }*/
        }

        public override bool AllowVerbCast(Verb verb)
        {
            return true;
        }
    }
}
