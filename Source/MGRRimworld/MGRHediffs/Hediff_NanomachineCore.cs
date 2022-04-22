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
    class Hediff_NanomachineCore : Hediff_Level
    {

        private float totalTime = 3000f;
        private float timeRemaining = 3000f;
        private float deltaTicks = 1f;
        private bool timerIsRunning = false;


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

        public override void ChangeLevel(int levelOffset) => this.level = (int)Mathf.Clamp((float)(this.level + levelOffset), 1, 100);

        public override void SetLevelTo(int targetLevel)
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

            base.PostAdd(dinfo);
            if(pawn.abilities.GetAbility(MGRDefOf.MGRDefOf.NanomachineCoreUnleashed) == null)
                pawn.abilities.GainAbility(MGRDefOf.MGRDefOf.NanomachineCoreUnleashed);
            else
                ChangeLevel(1);
            Log.Message("Adding severity");

        }

        public override void PostRemoved()
        {

            base.PostRemoved();

        }
    }
}
