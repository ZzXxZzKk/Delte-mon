﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleDelts.UI;

namespace BattleDelts {
    public class FidelTombstone : MonoBehaviour
    {

        public Sprite inactive, active1, active2, active3, ghostRise1, ghostRise2, ghostRise3, largeFidel;
        public NPCInteraction fidelTrainer;
        public SpriteRenderer tomb, ghost;
        bool isAnimating, hasTriggered;


        void Start()
        {
            hasTriggered = fidelTrainer.hasTriggered;
        }

        // Use this for initialization
        void OnTriggerEnter2D(Collider2D player)
        {
            if (!hasTriggered)
            {
                hasTriggered = true;
                isAnimating = true;
                PlayerMovement.Inst.StopMoving();

                // Set stats for Fidel's Delts
                foreach (DeltemonClass fidelt in fidelTrainer.oppDelts)
                {
                    fidelt.initializeDelt();
                }

                //			SoundEffectManager.SEM.PlaySoundImmediate ("FidelGhost");
                StartCoroutine(AnimateTomb());
            }
        }

        // Animates ghost coming out of tomb, starts battle
        IEnumerator AnimateTomb()
        {
            byte count = 0;

            while (isAnimating)
            {

                tomb.sprite = active1;
                yield return new WaitForSeconds(0.1f);
                tomb.sprite = active2;
                yield return new WaitForSeconds(0.1f);
                tomb.sprite = active3;
                yield return new WaitForSeconds(0.1f);

                count++;
                if (count == 1)
                {
                    ghost.sprite = ghostRise1;
                }
                else if (count == 2)
                {
                    ghost.sprite = ghostRise2;
                }
                else if (count == 3)
                {
                    ghost.sprite = ghostRise3;
                }
                else if (count == 4)
                {
                    StartInteraction();
                    isAnimating = false;
                }
            }
        }

        // Start fidel interaciton
        void StartInteraction()
        {
            UIManager.Inst.StartMessage(null, UIManager.Inst.characterSlideIn(largeFidel));
            UIManager.Inst.StartNPCMessage("OOoooooOOooOOOOOO", "Dead Fidel");
            UIManager.Inst.StartNPCMessage("You! You let this happen to me!", "Dead Fidel");
            UIManager.Inst.StartNPCMessage("I'm was just a hamster...", "Sad Dead Fidel");
            UIManager.Inst.StartNPCMessage("AND NOW I AM DEAD", "Angry Dead Fidel");
            UIManager.Inst.StartMessage(null, UIManager.Inst.characterSlideOut());
            UIManager.Inst.StartTrainerBattle(fidelTrainer, false);
            UIManager.Inst.StartMessage(null, null, () => EndInteraction());
        }

        // End fidel interaction
        void EndInteraction()
        {
            ghost.sprite = null;
            tomb.sprite = inactive;
        }
    }
}
