﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        /// <summary>
        /// Extra functions that allow a user to better debug/test/
        /// </summary>
        /// <returns>Return true if the calling method should return.</returns>
        private bool DebugModePhsx()
        {
#if WINDOWS
            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze)
            {
                // Test title screen
                if (Tools.keybState.IsKeyDown(Keys.G) && !Tools.PrevKeyboardState.IsKeyDown(Keys.G))
                {
                    //TitleGameFactory = TitleGameData_Intense.Factory;
                    TitleGameFactory = TitleGameData_MW.Factory;
                    //TitleGameFactory = TitleGameData_Forest.Factory;

                    Tools.SongWad.Stop();
                    Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory();
                    return true;
                }

                // Test title screen
                if (Tools.keybState.IsKeyDown(Keys.H) && !Tools.PrevKeyboardState.IsKeyDown(Keys.H))
                {
                    //TitleGameFactory = TitleGameData_Intense.Factory;
                    //TitleGameFactory = TitleGameData_MW.Factory;
                    TitleGameFactory = TitleGameData_Forest.Factory;

                    Tools.SongWad.Stop();
                    Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory();
                    return true;
                }

                if (Tools.keybState.IsKeyDown(Keys.J) && !Tools.PrevKeyboardState.IsKeyDown(Keys.J))
                {
                    Tools.CurGameData.FadeToBlack();
                }
            }

            //// Give award
            //if (Tools.keybState.IsKeyDown(Keys.S) && !Tools.PrevKeyboardState.IsKeyDown(Keys.S))
            //{
            //    Awardments.GiveAward(Awardments.UnlockHeroRush2);
            //}

            // Game Obj Viewer
            if (!Tools.ViewerIsUp && (!KeyboardExtension.Freeze || Tools.CntrlDown()) && (Tools.gameobj_viewer == null || Tools.gameobj_viewer.IsDisposed)
                && Tools.keybState.IsKeyDown(Keys.B) && !Tools.PrevKeyboardState.IsKeyDown(Keys.B))
            {
                Tools.gameobj_viewer = new Viewer.GameObjViewer();
                Tools.gameobj_viewer.Show();
            }
            if (Tools.gameobj_viewer != null)
            {
                if (Tools.gameobj_viewer.IsDisposed)
                    Tools.gameobj_viewer = null;
                else
                    Tools.gameobj_viewer.Input();
            }

            // Background viewer
            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze && (Tools.background_viewer == null || Tools.background_viewer.IsDisposed)
                && Tools.keybState.IsKeyDown(Keys.V) && !Tools.PrevKeyboardState.IsKeyDown(Keys.V))
            {
                Tools.background_viewer = new Viewer.BackgroundViewer();
                Tools.background_viewer.Show();
            }
            if (Tools.background_viewer != null)
            {
                if (Tools.background_viewer.IsDisposed)
                    Tools.background_viewer = null;
                else
                    Tools.background_viewer.Input();
            }

            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze && Tools.keybState.IsKeyDownCustom(Keys.F) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.F))
                ShowFPS = !ShowFPS;
#endif

#if PC_DEBUG
            if (Tools.FreeCam)
            {
                Vector2 pos = Tools.CurLevel.MainCamera.Data.Position;
                if (Tools.keybState.IsKeyDownCustom(Keys.Right)) pos.X += 130;
                if (Tools.keybState.IsKeyDownCustom(Keys.Left)) pos.X -= 130;
                if (Tools.keybState.IsKeyDownCustom(Keys.Up)) pos.Y += 130;
                if (Tools.keybState.IsKeyDownCustom(Keys.Down)) pos.Y -= 130;
                Tools.CurLevel.MainCamera.EffectivePos += pos - Tools.CurLevel.MainCamera.Data.Position;
                Tools.CurLevel.MainCamera.Data.Position = Tools.CurLevel.MainCamera.Target = pos;
                Tools.CurLevel.MainCamera.Update();
            }
#endif

            // Reload some dynamic data (tileset info, animation specifications).
            if (Tools.keybState.IsKeyDownCustom(Keys.X) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.X))
            {
#if INCLUDE_EDITOR
                if (LoadDynamic)
                {
                    ////Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Art);
                    ////Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Backgrounds);
                    //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Tilesets);
                    //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Animations);
                    TileSets.LoadSpriteEffects();
                    TileSets.LoadCode();
                }
#endif

                // Make blocks in the current level reset their art to reflect possible changes in the reloaded tileset info.
                foreach (BlockBase block in Tools.CurLevel.Blocks)
                {
                    NormalBlock nblock = block as NormalBlock;
                    if (null != nblock) nblock.ResetPieces();

                    MovingBlock mblock = block as MovingBlock;
                    if (null != mblock) mblock.ResetPieces();
                }
            }

#if DEBUG
            // Reload ALL dynamic data (tileset info, animation specifications, dynamic art, backgrounds).
            if (Tools.keybState.IsKeyDownCustom(Keys.Z) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Z))
            {
                ReloadInfo();

                // Reset blocks
                foreach (BlockBase block in Tools.CurLevel.Blocks)
                {
                    NormalBlock nblock = block as NormalBlock;
                    if (null != nblock) nblock.ResetPieces();

                    MovingBlock mblock = block as MovingBlock;
                    if (null != mblock) mblock.ResetPieces();
                }
            }
#endif

            // Turn on a simple green screen background.
            if (Tools.keybState.IsKeyDownCustom(Keys.D9) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D9))
                Background.GreenScreen = !Background.GreenScreen;

            Tools.ModNums();

            // Load a test level.
            if (Tools.keybState.IsKeyDownCustom(Keys.D5) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D5))
            {
                GameData.LockLevelStart = false;
                LevelSeedData.ForcedReturnEarly = 0;
                MakeTestLevel(); return true;
            }

            // Hide the GUI. Used for video capture.
            if (ButtonCheck.State(Keys.D8).Pressed) HideGui = !HideGui;

            // Hide the foreground. Used for video capture of backgrounds.
            if (ButtonCheck.State(Keys.D7).Pressed) HideForeground = !HideForeground;

            // Turn on/off immortality.
            if (Tools.keybState.IsKeyDownCustom(Keys.O) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.O))
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                {
                    bob.Immortal = !bob.Immortal;
                }
            }

            // Turn on/off graphics.
            if (Tools.keybState.IsKeyDownCustom(Keys.Q) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Q))
                Tools.DrawGraphics = !Tools.DrawGraphics;
            // Turn on/off drawing of collision detection boxes.
            if (Tools.keybState.IsKeyDownCustom(Keys.W) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.W))
                Tools.DrawBoxes = !Tools.DrawBoxes;
            // Turn on/off step control. When activated, this allows you to step forward in the game by pressing <Enter>.
            if (Tools.keybState.IsKeyDownCustom(Keys.E) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.E))
                Tools.StepControl = !Tools.StepControl;
            // Modify the speed of the game.
            if (Tools.keybState.IsKeyDownCustom(Keys.R) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.R))
            {
                Tools.IncrPhsxSpeed();
            }

            // Don't do any of the following if a control box is up.
            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze)
            {
                // Watch the computer make a level during Stage 1 of construction.
                if (Tools.keybState.IsKeyDownCustom(Keys.D3) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D3))
                {
                    GameData.LockLevelStart = false;
                    LevelSeedData.ForcedReturnEarly = 1;
                    MakeTestLevel(); return true;
                }

                // Watch the computer make a level during Stage 2 of construction.
                if (Tools.keybState.IsKeyDownCustom(Keys.D4) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D4))
                {
                    GameData.LockLevelStart = false;
                    LevelSeedData.ForcedReturnEarly = 2;
                    MakeTestLevel(); return true;
                }

                // Zoom in and out.
                if (Tools.keybState.IsKeyDownCustom(Keys.OemComma))
                {
                    Tools.CurLevel.MainCamera.Zoom *= .99f;
                    Tools.CurLevel.MainCamera.EffectiveZoom *= .99f;
                }
                if (Tools.keybState.IsKeyDownCustom(Keys.OemPeriod))
                {
                    Tools.CurLevel.MainCamera.Zoom /= .99f;
                    Tools.CurLevel.MainCamera.EffectiveZoom /= .99f;
                }

                // Turn on/off FreeCam, which allows the user to pan the camera through the level freely.
                if (Tools.keybState.IsKeyDownCustom(Keys.P) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.P))
                    Tools.FreeCam = !Tools.FreeCam;
            }

            // Allow Back to exit the game if we are in test mode
            if (SimpleLoad && ButtonCheck.State(ControllerButtons.Back, -1).Down)
            {
                Exit();
                return true;
            }

            /* XBOX Debug buttons
            if (Tools.padState[0].Buttons.B == ButtonState.Pressed && Tools.PrevpadState[0].Buttons.B != ButtonState.Pressed)
            {
                Tools.CurLevel.ResetAll(false);
                Tools.CurLevel.PlayMode = 0;
            }
            if (Tools.padState[0].Buttons.Y == ButtonState.Pressed && Tools.PrevpadState[0].Buttons.Y != ButtonState.Pressed)
            {
                Tools.CurLevel.ResetAll(false);
                Tools.CurLevel.PlayMode = 1;
            }
            if (Tools.padState[0].Buttons.X == ButtonState.Pressed && Tools.PrevpadState[0].Buttons.X != ButtonState.Pressed)
            {
                Tools.PhsxSpeed += 1;
                if (Tools.PhsxSpeed > 4) Tools.PhsxSpeed = 1;
            }
            */

            return false;
        }

        public static string debugstring = "";
        StringBuilder MainString = new StringBuilder(100, 100);

        /// <summary>
        /// Method for drawing various debug information to the screen.
        /// </summary>
        void DrawDebugInfo()
        {
            if (Tools.ScreenshotMode) return;

            Tools.StartSpriteBatch();

            if (Tools.ShowNums)
            {
                string nums = Tools.Num_0_to_2 + "\n\n" + Tools.Num_0_to_360;

                Tools.StartSpriteBatch();
                Tools.spriteBatch.DrawString(DebugFont,
                        nums,
                        new Vector2(0, 100),
                        Color.Orange, 0, Vector2.Zero, .4f, SpriteEffects.None, 0);
                Tools.EndSpriteBatch();
                return;
            }

#if WINDOWS
            // Grace period for falling
            //string str = "";
            //if (Tools.CurLevel != null && Tools.CurLevel.Bobs.Count > 0)
            //{
            //    //var phsx = Tools.CurLevel.Bobs[0].MyPhsx as BobPhsxNormal;
            //    //if (null != phsx) str = phsx.FallingCount.ToString();

            //    var phsx = Tools.CurLevel.Bobs[0].MyPhsx as BobPhsxMeat;
            //    //if (null != phsx) str = phsx.WallJumpCount.ToString();
            //    if (null != phsx) str = phsx.StepsSinceSide.ToString();
            //}

            // Mouse
            //string str = string.Format("Mouse delta: {0}", Tools.DeltaMouse);
            //string str = string.Format("Mouse in window: {0}", Tools.MouseInWindow);

            // VPlayer
            //string str = "";
            //if (VPlayer1 != null)
            //{
            //    str = VPlayer1.PlayPosition.Ticks.ToString();
            //    //Console.WriteLine(str);
            //}


            // GC
            string str = GC.CollectionCount(0).ToString() + " " + fps.ToString() + "\n"
                + (RunningSlowly ? "SLOW" : "____ FAST") + "\n"
                + debugstring;

            // Phsx count
            //string str  = string.Format("CurLevel PhsxStep: {0}\n", Tools.CurLevel.CurPhsxStep);

            // Score
            //PlayerData p = PlayerManager.Get(0);
            //string str = string.Format("Death {0}, {1}, {2}, {3}, {4}", p.TempStats.TotalDeaths, p.LevelStats.TotalDeaths, p.GameStats.TotalDeaths, p.CampaignStats.TotalDeaths, Campaign.Attempts);
            //Campaign.CalcScore();
            //string str2 = string.Format("Coins {0}, {1}, {2}, {3}, {4}", p.TempStats.Coins, p.LevelStats.Coins, p.GameStats.Coins, p.CampaignStats.Coins, Campaign.Coins);
            //str += "\n\n" + str2;
            //string str3 = string.Format("Total {0}, {1}, {2}, {3}, {4}", p.TempStats.TotalCoins, p.LevelStats.TotalCoins, p.GameStats.TotalCoins, p.CampaignStats.TotalCoins, Campaign.TotalCoins);
            //str += "\n\n" + str3;
            //string str4 = string.Format("Total {0}, {1}, {2}, {3}", p.TempStats.TotalBlobs, p.LevelStats.TotalBlobs, p.GameStats.TotalBlobs, p.CampaignStats.TotalBlobs);
            //str += "\n" + str4;
            //string str5 = string.Format(" {0}, {1}, {2}, {3}", p.TempStats.Blobs, p.LevelStats.Blobs, p.GameStats.Blobs, p.CampaignStats.Blobs);
            //str += "\n" + str5;

            // Coins
            //string str = string.Format("{0}, {1}, {2}, {3}", p.TempStats.Coins, p.LevelStats.Coins, p.GameStats.Coins, p.CampaignStats.Coins);
            //string str2 = string.Format("{0}, {1}, {2}, {3}", p.TempStats.TotalCoins, p.LevelStats.TotalCoins, p.GameStats.TotalCoins, p.CampaignStats.TotalCoins);
            //str += "\n" + str2;
#else
            string str = debugstring;
#endif

            //str = string.Format("{0,-5} {1,-5} {2,-5} {3,-5} {4,-5}", Level.Pre1, Level.Step1, Level.Pre2, Level.Step2, Level.Post);


            Tools.spriteBatch.DrawString(DebugFont,
                    str,
                    new Vector2(0, 100),
                    Color.Orange, 0, Vector2.Zero, .4f, SpriteEffects.None, 0);
            Tools.EndSpriteBatch();
        }
    }
}