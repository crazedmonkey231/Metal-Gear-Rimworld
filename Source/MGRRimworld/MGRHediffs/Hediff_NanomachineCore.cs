using AbilityUser;
using MGRRimworld.MGRComps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MGRRimworld.MGRHediffs
{
    class Hediff_NanomachineCore : HediffWithComps
    {

        private float totalTime = 3000f;
        private float timeRemaining = 3000f;
        private float deltaTicks = 1f;
        private bool timerIsRunning = false;

        public int level = 1;
        public override string Label => (string)(this.def.label + " (" + "Level".Translate() + " ") + (object)this.level + ")";
        public override bool ShouldRemove => this.level == 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.level, "level");
            Scribe_Values.Look<bool>(ref this.timerIsRunning, "timerIsRunning");
            Scribe_Values.Look<float>(ref this.deltaTicks, "deltaTicks");
            Scribe_Values.Look<float>(ref this.timeRemaining, "timeRemaining");
            Scribe_Values.Look<float>(ref this.totalTime, "totalTime");
            if (Scribe.mode != LoadSaveMode.PostLoadInit || this.Part != null)
                return;
            Log.Error(this.GetType().Name + " has null part after loading.");
            this.pawn.health.hediffSet.hediffs.Remove((Hediff)this);
        }

        public override void Tick()
        {

            base.Tick();
            this.UpdateTimer();
            if (this.Severity > 1 && !timerIsRunning)
            {
                Log.Message("Resetting");
                Reset();
            }

        }

        public void ChangeLevel(int levelOffset)
        {
            this.level += levelOffset;
        }

        public void SetLevelTo(int targetLevel)
        {
            if (targetLevel == this.level)
                return;
            this.ChangeLevel(targetLevel - this.level);
        }

        private void Reset()
        {
            timerIsRunning = true;
        }
        private void UpdateTimer()
        {
            if (timerIsRunning)
            {
                if(timeRemaining > 0)
                {
                    timeRemaining -= deltaTicks;
                }
                else
                {
                    Log.Message("TimerFinished");
                    timeRemaining = totalTime;
                    timerIsRunning = false;
                    ChangeLevel(-1);
                }
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {

            if(dinfo != null)
            {

                float pwrAbsorbed = ((DamageInfo)dinfo).Amount;
                Log.Message("Power Absorbed from all power grids: " + pwrAbsorbed);
                ChangeLevel((int)pwrAbsorbed);

                Log.Message("Added Power");
            }

            base.PostAdd(null);
        }

        public override void PostRemoved()
        {

            base.PostRemoved();

        }
    }
}
