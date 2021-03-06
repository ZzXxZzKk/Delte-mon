﻿using BattleDelts.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BattleDelts.Battle
{
    public class BattleSetUp
    {
        TrainerAI TrainerAI;
        WildDeltAI WildAI;
        BattleState State;

        public BattleSetUp(BattleState state)
        {
            TrainerAI = new TrainerAI(state);
            WildAI = new WildDeltAI(state);
            State = state;
        }

        void PrepareBattleState(BattleType type, BattleAI AI)
        {
            // Do setup common to all battles
            State.Reset();
            State.Type = type;
            State.OpponentAI = AI;
            
            // REFACTOR_TODO: Queue the beginning of battle
            BattleAnimator animator = BattleManager.Inst.Animator;
            BattleManager.AddToBattleQueue(enumerator: animator.DeltAnimation("SlideOut", true));
            BattleManager.AddToBattleQueue(enumerator: animator.DeltAnimation("SlideOut", false));
        }

        // REFACTOR_TODO: These specific battle cases should have their own music, background selection, etc.
        public void WildDeltBattleSetup()
        {
            PrepareBattleState(BattleType.Wild, WildAI);
        }

        public void TrainerBattleSetup()
        {
            PrepareBattleState(BattleType.Trainer, TrainerAI);
        }

        public void GymLeaderBattleSetup()
        {
            PrepareBattleState(BattleType.GymLeader, TrainerAI);
        }
        
        // Function to initialize a new battle, for trainers and wild Delts
        public void InitializeBattle()
        {
            GameQueue.WorldAdd(action: () => GameQueue.Inst.ChangeQueueType(GameQueue.QueueType.Battle));

            BattleManager.Inst.BattleUI.LoadBackgroundAndPodium();
            
            PlayBattleMusic();

            // Clear temp battle stats for player and opponent
            State.PlayerState.ResetStatAdditions();
            State.OpponentState.ResetStatAdditions();
            
            State.PlayerState.Delts = GameManager.Inst.deltPosse;

            // Select current battling Delts, update UI
            DeltemonClass startingDelt = State.PlayerState.Delts.Find(delt => delt.curStatus != statusType.DA);
            State.RegisterAction(true, new SwitchDeltAction(State, startingDelt));
            State.PlayerState.ChosenAction.ExecuteAction();
        }
        
        // Initializes battle for a player vs. NPC battle
        public void StartTrainerBattle(NPCInteraction oppTrainer, bool isGymLeader)
        {
            InitializeBattle();

            InitializeTrainerAI(oppTrainer);

            State.Type = isGymLeader ? BattleType.GymLeader : BattleType.Trainer;
            
            BattleManager.Inst.BattleUI.UpdateTrainerPosseBalls();

            // Select current battling Delts, update UI
            new SwitchDeltAction(State, oppTrainer.oppDelts[0]).ExecuteAction();

            // End NPC Messages and start turn
            //BattleManager.AddToBattleQueue(action: () => UIManager.Inst.EndNPCMessage());
            BattleManager.Inst.TurnProcess.StartTurn();

            // Remove NPC's notification chat bubble
            //BattleManager.AddToBattleQueue(action: () => UIManager.Inst.EndNPCMessage());
        }

        public void InitializeTrainerAI(NPCInteraction trainer)
        {
            TrainerAI.Trainer = trainer;

            // Set opp Delts going into battle
            State.OpponentState.Delts = trainer.oppDelts;

            // Set victory coins
            TrainerAI.CoinReward = trainer.coins;

            // Set trainer items and name
            TrainerAI.trainerItems = trainer.trainerItems;
            TrainerAI.TrainerName = trainer.NPCName;

            State.OpponentAI = TrainerAI;
        }

        // Start a battle originating from TallGrass.cs
        public void StartWildBattle(DeltemonClass oppDeltSpawn)
        {
            InitializeBattle();

            State.OpponentAI = WildAI;

            // Ensure Delt doesn't start with status affliction
            // REFACTOR_TODO: Put this in battle UI
            oppDeltSpawn.curStatus = statusType.None;
            oppDeltSpawn.statusImage = BattleManager.Inst.BattleUI.noStatus;

            // REFACTOR_TODO: Serialize field
            BattleManager.Inst.BattleUI.transform.GetChild(2).GetChild(4).gameObject.SetActive(false);
            
            BattleManager.AddToBattleQueue("A wild " + oppDeltSpawn.deltdex.nickname + " appeared!");
            BattleManager.Inst.TurnProcess.StartTurn();
        }
        
        void PlayBattleMusic()
        {
            AudioSource source = MusicManager.Inst.audiosource;
            BattleManager.Inst.sceneMusic = source.clip;
            source.clip = BattleManager.Inst.BattleMusic;
            source.Play();
        }
    }
}