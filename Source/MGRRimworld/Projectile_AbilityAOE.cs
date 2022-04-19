using AbilityUser;
using MGRRimworld.MGRMoteMaker;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MGRRimworld
{
    class Projectile_AbilityAOE : Projectile_AbilityLaser
    {
        private int pwrVal;
        private int verVal;
        private float arcaneDmg;
        private int age;
        private int duration;
        private float radius;
        private bool initialized;
        private int healDelay;
        private int waveDelay;
        private int lastHeal;
        private int lastWave;
        private Pawn caster;
        private float angle;
        private float innerRing;
        private float outerRing;
        private float ringFrac;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", true);
            Scribe_Values.Look<int>(ref this.age, "age", -1);
            Scribe_Values.Look<int>(ref this.duration, "duration", 1200);
            Scribe_Values.Look<int>(ref this.healDelay, "healDelay", 30);
            Scribe_Values.Look<int>(ref this.lastHeal, "lastHeal");
            Scribe_Values.Look<int>(ref this.waveDelay, "waveDelay", 240);
            Scribe_Values.Look<int>(ref this.lastWave, "lastWave");
            Scribe_Values.Look<float>(ref this.radius, "radius", 6f);
            Scribe_References.Look<Pawn>(ref this.caster, "caster");
        }

        private int TicksLeft => this.duration - this.age;

        protected override void Impact(Thing hitThing)
        {
            // ISSUE: explicit non-virtual call
            Map map = (((Thing)this).Map);
            base.Impact(hitThing);
            ThingDef def = ((Thing)this).def;
            this.caster = ((Projectile_AbilityAOE)this).launcher as Pawn;
            this.pwrVal = 3;
            this.verVal = 3;
            this.radius = ((Thing)this).def.projectile.explosionRadius;
            this.angle = (float)Rand.Range(-12, 12);
            this.duration += 180 * (this.pwrVal + this.verVal);
            this.initialized = true;
            if (this.age < this.lastWave)
                return;
            if (this.age >= this.lastHeal + this.healDelay)
            {
                float ringFrac = this.ringFrac;
                if (!0.0f.Equals(ringFrac))
                {
                    if (!1f.Equals(ringFrac))
                    {
                        if (!2f.Equals(ringFrac))
                        {
                            if (!3f.Equals(ringFrac))
                            {
                                if (!4f.Equals(ringFrac))
                                {
                                    if (5f.Equals(ringFrac))
                                    {
                                        this.innerRing = this.outerRing;
                                        this.outerRing = this.radius;
                                        this.lastWave = this.age + this.waveDelay;
                                    }
                                }
                                else
                                {
                                    this.innerRing = this.outerRing;
                                    this.outerRing = this.radius * (this.ringFrac / 5f);
                                }
                            }
                            else
                            {
                                this.innerRing = this.outerRing;
                                this.outerRing = this.radius * (this.ringFrac / 5f);
                            }
                        }
                        else
                        {
                            this.innerRing = this.outerRing;
                            this.outerRing = this.radius * (this.ringFrac / 5f);
                        }
                    }
                    else
                    {
                        this.innerRing = this.outerRing;
                        this.outerRing = this.radius * (this.ringFrac / 5f);
                    }
                }
                else
                {
                    this.innerRing = 0.0f;
                    this.outerRing = this.radius * (float)(((double)this.ringFrac + 0.100000001490116) / 5.0);
                    MGR_MoteMaker.MakePowerBeamMote((((Thing)this).Position),(((Thing)this).Map), this.radius * 6f, 1.2f, (float)this.waveDelay / 60f, (float)((double)this.healDelay * 3.0 / 60.0), (float)((double)this.healDelay * 2.0 / 60.0));
                }
                ++this.ringFrac;
                this.lastHeal = this.age;
                this.Search(map);
            }
            if ((double)this.ringFrac >= 6.0)
                this.ringFrac = 0.0f;
        }

        public void Search(Map map)
        {
            // ISSUE: explicit non-virtual call
            IEnumerable<IntVec3> source1 = GenRadial.RadialCellsAround((((Thing)this).Position), this.radius, true);
            // ISSUE: explicit non-virtual call
            IEnumerable<IntVec3> source2 = GenRadial.RadialCellsAround((((Thing)this).Position), this.innerRing, true);
            // ISSUE: explicit non-virtual call
            IEnumerable<IntVec3> source3 = GenRadial.RadialCellsAround((((Thing)this).Position), this.outerRing, true);
            for (int index = source2.Count<IntVec3>(); index < source3.Count<IntVec3>(); ++index)
            {
                Pawn pawn = (Pawn)null;
                IntVec3 c = source1.ToArray<IntVec3>()[index];
                if (c.InBounds(map) && c.IsValid)
                    pawn = c.GetFirstPawn(map);
                if (pawn != null && (pawn.Faction == this.caster.Faction || pawn.IsPrisoner || pawn.Faction == null || pawn.Faction != null && !pawn.Faction.HostileTo(this.caster.Faction)))
                    this.Heal(pawn);
                /*                if (pawn != null && TM_Calc.IsUndead(pawn))
                                    TM_Action.DamageUndead(pawn, (float)(10.0 + (double)this.pwrVal * 3.0) * this.arcaneDmg, ((Projectile)this).launcher);*/
            }
        }

        public void Heal(Pawn pawn)
        {
            /*if (pawn == null || pawn.Dead)
                return;
            int num1 = 1 + this.verVal;
            foreach (BodyPartRecord injuredPart in pawn.health.hediffSet.GetInjuredParts())
            {
                BodyPartRecord rec = injuredPart;
                if (num1 > 0)
                {
                    int num2 = 1 + this.verVal;
                    foreach (Hediff_Injury hd in pawn.health.hediffSet.GetHediffs<Hediff_Injury>().Where<Hediff_Injury>((Func<Hediff_Injury, bool>)(injury => injury.Part == rec)))
                    {
                        if (num2 > 0 && (hd.CanHealNaturally() && !hd.IsPermanent()))
                        {
                            if (Rand.Chance(0.8f))
                            {
                                hd.Heal((float)(10.0 + (double)this.pwrVal * 2.0) * this.arcaneDmg);
                                MGR_MoteMaker.ThrowRegenMote(pawn.Position.ToVector3Shifted(), pawn.Map, 1.2f);
                            }
                            --num1;
                            --num2;
                        }
                    }
                }
            }*/
        }

        public override void Tick()
        {
            base.Tick();
            ++this.age;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.age <= this.duration)
                return;
            // ISSUE: explicit non-virtual call
            ((ThingWithComps)this).Destroy(mode);
        }

    }
}
