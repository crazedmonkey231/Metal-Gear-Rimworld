using MGRApparel.MGRGizmo;
using MGRRimworld;
using MGRRimworld.MGRAbilities;
using MGRRimworld.MGRComps;
using MGRRimworld.MGRDefOf;
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
        private int StartingTicksToReset = 500;
        private float EnergyOnReset = 0.2f;
        private float EnergyLossPerDamage = 0.033f;
        private int KeepDisplayingTicks = 1000;
        private float ApparelScorePerEnergyMax = 0.25f;



        //private static readonly Material BubbleMat = MaterialPool.MatFrom("Shields/shield_a", ShaderDatabase.Transparent);

        private float EnergyMax => this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);

        private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;

        public float Energy => energy;


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

            foreach (Gizmo gizmo in this.GetGizmos())
                yield return gizmo;

            if (Find.Selector.SingleSelectedThing == this.Wearer)
            {

                yield return (Gizmo)new MGR_Gizmo_ExoFrame() { exoFrame = this };

            }
        }

        public override float GetSpecialApparelScoreOffset()
        {
            return EnergyMax * ApparelScorePerEnergyMax;
        }

        public override void Tick()
        {
            base.Tick();
            if (this.Wearer == null)
                this.energy = 0.0f;
            else if (this.ShieldState == ShieldState.Resetting)
            {
                --this.ticksToReset;
                if (this.ticksToReset > 0)
                    return;
                this.Reset();
            }
            else
            {
                if (this.ShieldState != ShieldState.Active)
                    return;
                this.energy += this.EnergyGainPerTick;
                if ((double)this.energy <= (double)this.EnergyMax)
                    return;
                this.energy = this.EnergyMax;
            }
        }


        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
            {
            if (this.ShieldState != ShieldState.Active)
                return false;
            if (dinfo.Def == DamageDefOf.EMP)
            {
                this.energy = 0.0f;
                this.Break();
                return false;
            }
            this.energy -= dinfo.Amount * this.EnergyLossPerDamage;
            if ((double)this.energy <= 0.0)
            {
                Log.Message("broken " + dinfo.ToString() + " : " + dinfo.Weapon.IsMeleeWeapon);
                if (dinfo.Weapon == null || dinfo.Weapon.IsMeleeWeapon || dinfo.Weapon.defName.Equals(MGRDefOf.MeleeWeapon_MGR_Katana.defName))
                {
                    Log.Message("instigator " + dinfo.Instigator + " : target" + dinfo.IntendedTarget);

                    DamageInfo recoil = new DamageInfo(DamageDefOf.Bomb, 100, armorPenetration:200, instigator: dinfo.Instigator, hitPart: dinfo.HitPart);
                    recoil.SetAllowDamagePropagation(false);
                    recoil.Instigator.TakeDamage(dinfo);
                    GenExplosion.DoExplosion(dinfo.Instigator.Position, dinfo.Instigator.Map, 1f, DamageDefOf.Smoke, dinfo.Instigator, damAmount: 0, postExplosionSpawnThingDef: ThingDefOf.Gas_Smoke, postExplosionSpawnChance: 1f);
                }

                this.Break();
            }
            else
            {
                this.AbsorbedDamage(dinfo);
            }
            return true;
        }

        public void KeepDisplaying() => this.lastKeepDisplayTick = Find.TickManager.TicksGame;

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
            this.energy = 0.0f;
            this.ticksToReset = this.StartingTicksToReset;
        }

        private void Reset()
        {
            if (base.Wearer.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
                FleckMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
            }
            this.ticksToReset = -1;
            this.energy = this.EnergyOnReset;
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

        public override bool AllowVerbCast(Verb verb) => true;

    }
}
