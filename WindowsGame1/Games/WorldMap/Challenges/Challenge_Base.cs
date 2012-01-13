﻿using System;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public delegate LevelSeedData MakeSeed();

    public class AftermathData
    {
        public bool Success;
        public bool EarlyExit;
        public bool Retry = false;
    }

    public class Challenge
    {
        protected virtual void ShowEndScreen()
        {
        }

        public static DifficultyFunc D { get { return Campaign.D; } }

        /// <summary>
        /// The last difficulty selected via the difficulty select menu
        /// </summary>
        public static int PreviousMenuIndex = 0;


        public string MenuPic;
        public string Name, MenuName;
        public Guid ID;
        
        public int Goal;

        static bool GoalMet = false;
        public virtual bool GetGoalMet() { return GoalMet; }
        public virtual void SetGoalMet(bool value) { GoalMet = value; }

        public ScoreList HighScore, HighLevel;

        public Challenge(string Name)
        {
            this.Name = Name;
            ID = new Guid();
        }

        public Challenge() { }

        /// <summary>
        /// If true then this meta-game is not part of the campaign.
        /// </summary>
        public bool NonCampaign = true;
        public virtual void Start(int Difficulty)
        {
            if (NonCampaign)
                PlayerManager.CoinsSpent = 0;

            DifficultySelected = Difficulty;
        }

        /// <summary>
        /// The difficulty selected for this challenge.
        /// </summary>
        public int DifficultySelected;

        /// <summary>
        /// Called immediately after the end of the challenge.
        /// </summary>
        public void Aftermath()
        {
            AftermathData data = Tools.CurrentAftermath;

            if (data.Success)
                foreach (PlayerData player in PlayerManager.Players)
                    player.ChallengeStars[ID] = Math.Max(player.ChallengeStars[ID], DifficultySelected + 1);
        }
        
        protected virtual void SetGameParent(GameData game)
        {
            game.ParentGame = Tools.CurGameData;
            Tools.WorldMap = Tools.CurGameData = game;
            Tools.CurLevel = game.MyLevel;
        }

        protected virtual List<MakeSeed> MakeMakeList(int Difficulty) { return null; }
        protected virtual List<MakeSeed> MakeMoreMakeList(int Difficulty) { return null; }

        public virtual LevelSeedData GetSeed(int Difficulty, int Index)
        {
            return MakeMakeList(Difficulty)[Index]();
        }

        public virtual List<LevelSeedData> GetMoreSeeds()
        {
            List<LevelSeedData> seeds = new List<LevelSeedData>();

            var MakeMore = MakeMoreMakeList(DifficultySelected);

            if (MakeMore == null) return null;

            foreach (MakeSeed make in MakeMore)
                seeds.Add(make());

            return seeds;
        }

        public virtual List<LevelSeedData> GetSeeds(int Difficulty)
        {
            DifficultySelected = Difficulty;

            List<LevelSeedData> seeds = new List<LevelSeedData>();
            
            foreach (MakeSeed make in MakeMakeList(Difficulty))
                seeds.Add(make());

            return seeds;
        }

        public virtual List<LevelSeedData> GetSeeds()
        {
            return GetSeeds(DifficultySelected);
        }
    }
}