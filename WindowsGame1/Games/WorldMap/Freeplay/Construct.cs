﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_Construct : Challenge_Escalation
    {
        static int[] NumLives = { 15, 12, 10, 5, 1 };

        static readonly Challenge_Construct instance = new Challenge_Construct();
        public static new Challenge_Construct Instance { get { return instance; } }

        static bool GoalMet = false;
        public override bool GetGoalMet() { return GoalMet; }
        public override void SetGoalMet(bool value) { GoalMet = value; }

        protected Challenge_Construct()
        {
            ID = new Guid("1a2221a2-234b-4e1e-7298-0c36b6ebdcc3");
            Name = "Construct";
            MenuPic = "menupic_escalation";
            HighScore = SaveGroup.ConstructHighScore;
            HighLevel = SaveGroup.ConstructHighLevel;
        }

        protected override void PreStart_Tutorial()
        {
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new Escalation_Tutorial(this, Campaign.PrincessPos.CenterToUp));
        }

        int LevelLength_Short = 5050;
        int LevelLength_Long = 8300;
        static TileSet[] tilesets = {
            TileSet.Castle, TileSet.Dungeon, TileSet.Terrace,
            TileSet.Castle, TileSet.Dungeon, TileSet.Rain,
            TileSet.Castle, TileSet.Dungeon, TileSet._Night };

        static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[] {
            BobPhsxDouble.Instance, BobPhsxJetman.Instance, BobPhsxSmall.Instance });

        protected override BobPhsx GetHero(int i)
        {
            return HeroTypes[i % HeroTypes.Count];
        }

        protected override TileSet GetTileSet(int i)
        {
            return tilesets[(i / LevelsPerTileset) % tilesets.Length];
        }

        protected override LevelSeedData Make(int Index, float Difficulty)
        {
            BobPhsx hero = GetHero(Index);

            // Adjust the length. Longer for higher levels.
            int Length;
            float t = ((Index - StartIndex) % LevelsPerTileset) / (float)(LevelsPerTileset - 1);
            Length = Tools.LerpRestrict(LevelLength_Short, LevelLength_Long, t);

            // Create the LevelSeedData
            LevelSeedData data = Campaign.HeroLevel(Difficulty, hero, Length, 1, LevelGeometry.Up);
            data.SetBackground(GetTileSet(Index - StartIndex));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                // Shorten the initial computer delay
                piece.Style.ComputerWaitLengthRange = new Vector2(8, 35);

                piece.Style.MyModParams = (level, p) =>
                {
                    Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
                    Params.StartFrame = 90;
                    Params.FillType = Coin_Parameters.FillTypes.Regular;
                };
            }

            return data;
        }
    }
}