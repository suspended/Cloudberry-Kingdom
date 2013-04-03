﻿using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class TitleGameData_MW : TitleGameData
    {
        public static GameData Factory()
        {
            return new TitleGameData_MW();
        }

        public override void Release()
        {
            base.Release();
        }

        public TitleGameData_MW() : base()
        {
        }

        public override void SetToReturnTo(int code)
        {
            base.SetToReturnTo(code);
        }

        public override void ReturnTo(int code)
        {
            if (SetToRepaly)
            {
                Tools.SongWad.SetPlayList(Tools.Song_Heavens);
                Tools.SongWad.Restart(true, false);
            }

            base.ReturnTo(code);
        }
      
        protected override Level MakeLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();

            level.TimeLimit = -1;

            level.CurPiece = level.StartNewPiece(0, null, 4);

            Vector2 Center = new Vector2(0, 0);
            
            level.MainCamera.MakeFancyPos();
            level.MainCamera.FancyPos.RelVal = Center;

            level.CurPiece.CamStartPos = Center;

            // Camera Zone
            CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init(Vector2.Zero, new Vector2(100000, 100000));
            CamZone.Start = Center;
            CamZone.End = Center;
            CamZone.FreeY = false;
            level.AddObject(CamZone);
            level.MainCamera.MyZone = CamZone;

            return level;
        }

        public StartMenu_MW_Backpanel BackPanel;
        public StartMenu_MW_PressStart PressStart;
        public override void Init()
        {
            base.Init();

            Tools.CurGameType = TitleGameData_MW.Factory;
            
            Tools.TitleGame = this;

            // Backdrop
            BackPanel = new StartMenu_MW_Backpanel();
            AddGameObject(BackPanel);
            BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene);
            BackPanel.InitialZoomIn();

            // Music
            Tools.Warning();
            //Tools.MusicVolume.Val = 0f;
            Tools.SongWad.SuppressNextInfoDisplay = true;
            //Tools.SongWad.SetPlayList(Tools.Song_WritersBlock);
            Tools.SongWad.SetPlayList(Tools.Song_Heavens);
            Tools.SongWad.Start(true);

            // Fade in
            FadeIn(.0175f);
            WaitThenDo(18, _Init);
        }

        void _Init()
        {
            // Press Start
            PressStart = new StartMenu_MW_PressStart(this);
            AddGameObject(PressStart);

            /*
            // Menu
            GUI_Panel panel;

            if (CloudberryKingdomGame.StartAsFreeplay)
                panel = new CustomLevel_GUI();
            else
            {
                panel = new StartMenu_MW(this);

                //panel = new StartMenu();

                //panel = new AdvancedCustomGUI();
                //panel = new SimpleCustomGUI();

                //panel = new StatsMenu();
                //panel = new AwardmentMenu();
                //panel = new CategoryMenu();
                //panel = new CampaignMenu();
                //panel = new ArcadeMenu();
            }

            AddGameObject(panel);
             */
        }

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            MyLevel.MainCamera.MyZone = CamZone;            
        }

        int XButtonPressCount = 0;
        int NotPressedCount = 0;
        public override void PhsxStep()
        {
            if (CloudberryKingdomGame.DigitalDayBuild)
            {
				//if (ButtonCheck.State(ControllerButtons.X, -2).Down)
				//{
				//    if (ButtonCheck.State(ControllerButtons.X, -2).Pressed)
				//    {
				//        XButtonPressCount++;
				//        NotPressedCount = 0;
				//    }
				//}
				//else
				//{
				//    NotPressedCount++;
				//    if (NotPressedCount > 60)
				//    {
				//        XButtonPressCount = 0;
				//    }
				//}
				//
				//if (XButtonPressCount > 3)
				//{
				//    // Start at Screen Saver
				//    ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
				//}

				if (ButtonCheck.State(ControllerButtons.X, -2).Down)
				{
					XButtonPressCount++;

					if (XButtonPressCount > 50)
					{
						// Start at Screen Saver
						ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
					}
				}
				else
				{
					XButtonPressCount = 0;
				}
            }

            base.PhsxStep();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }
}