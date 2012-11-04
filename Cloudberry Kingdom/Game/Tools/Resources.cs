﻿using System.Reflection;
using System;
using System.Threading;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using CoreEngine;
using CoreEngine.Random;

using CloudberryKingdom;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public static class Resources
    {
        /// <summary>
        /// True when we are still loading resources during the game's initial load.
        /// This is wrapped in a class so that it can be used as a lock.
        /// </summary>
        public static WrappedBool LoadingResources;

        /// <summary>
        /// Tracks how many resources have been loaded.
        /// This is wrapped in a class so that it can be used as a lock.
        /// </summary>
        public static WrappedFloat ResourceLoadedCountRef;

        public static EzFont Font_Grobold42, Font_Grobold42_2;
        public static EzFont LilFont;

        /// <summary>
        /// Load the necessary fonts.
        /// </summary>
        static void FontLoad()
        {
            Resources.Font_Grobold42 = new EzFont("Fonts/Grobold_42", "Fonts/Grobold_42_Outline", -50, 40);
            Resources.Font_Grobold42_2 = new EzFont("Fonts/Grobold_42", "Fonts/Grobold_42_Outline2", -50, 40);

            Resources.LilFont = new EzFont("Fonts/LilFont");

            Tools.Write("Fonts done...");
        }

        static void LoadInfo()
        {
            Tools.Write("Starting to load info...");
            var t = new System.Diagnostics.Stopwatch();
            t.Start();

            PieceQuad c;

            // Moving block
            c = PieceQuad.MovingBlock = new PieceQuad();
            c.Center.MyTexture = "blue_small";

            // Falling block
            var Fall = new AnimationData_Texture("FallingBlock", 1, 4);
            PieceQuad.FallingBlock = new PieceQuad();
            PieceQuad.FallingBlock.Center.SetTextureAnim(Fall);
            PieceQuad.FallGroup = new BlockGroup();
            PieceQuad.FallGroup.Add(100, PieceQuad.FallingBlock);
            PieceQuad.FallGroup.SortWidths();

            // Bouncy block
            var Bouncy = new AnimationData_Texture("BouncyBlock", 1, 2);
            PieceQuad.BouncyBlock = new PieceQuad();
            PieceQuad.BouncyBlock.Center.SetTextureAnim(Bouncy);
            PieceQuad.BouncyGroup = new BlockGroup();
            PieceQuad.BouncyGroup.Add(100, PieceQuad.BouncyBlock);
            PieceQuad.BouncyGroup.SortWidths();

            // Moving block
            PieceQuad.MovingGroup = new BlockGroup();
            PieceQuad.MovingGroup.Add(100, PieceQuad.MovingBlock);
            PieceQuad.MovingGroup.SortWidths();

            // Elevator
            PieceQuad.Elevator = new PieceQuad();
            PieceQuad.Elevator.Center.Set("palette");
            PieceQuad.Elevator.Center.SetColor(new Color(210, 210, 210));
            PieceQuad.ElevatorGroup = new BlockGroup();
            PieceQuad.ElevatorGroup.Add(100, PieceQuad.Elevator);
            PieceQuad.ElevatorGroup.SortWidths();


            //#if INCLUDE_EDITOR
            //if (LoadDynamic)
            {
                //if (!AlwaysSkipDynamicArt)
                //    Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Art);
                //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Backgrounds);
                //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Animations);
                //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Tilesets);
            }
            //#endif

            t.Stop();
            Tools.Write("... done loading info!");
            Tools.Write("Total seconds: {0}", t.Elapsed.TotalSeconds);
        }

        static void LoadMusic(bool CreateNewWad)
        {
            if (CreateNewWad)
                Tools.SongWad = new EzSongWad();

            Tools.SongWad.PlayerControl = Tools.SongWad.DisplayInfo = true;

            string path = Path.Combine(Globals.ContentDirectory, "Music");
            string[] files = Directory.GetFiles(path);

            foreach (String file in files)
            {
                int i = file.IndexOf("Music") + 5 + 1;
                if (i < 0) continue;
                int j = file.IndexOf(".", i);
                if (j <= i) continue;
                String name = file.Substring(i, j - i);
                String extension = file.Substring(j + 1);

                if (extension == "xnb")
                {
                    if (CreateNewWad)
                        Tools.SongWad.AddSong(Tools.GameClass.Content.Load<Song>("Music\\" + name), name);
                    else
                        Tools.SongWad.FindByName(name).song = Tools.GameClass.Content.Load<Song>("Music\\" + name);
                }

                ResourceLoadedCountRef.MyFloat++;
            }


            Tools.Song_Happy = Tools.SongWad.FindByName("Happy");
            Tools.Song_Happy.DisplayInfo = false;

            Tools.Song_140mph = Tools.SongWad.FindByName("140_Mph_in_the_Fog^Blind_Digital");
            Tools.Song_140mph.Volume = .7f;

            Tools.Song_BlueChair = Tools.SongWad.FindByName("Blue_Chair^Blind_Digital");
            Tools.Song_BlueChair.Volume = .7f;

            Tools.Song_Evidence = Tools.SongWad.FindByName("Evidence^Blind_Digital");
            Tools.Song_Evidence.Volume = .7f;

            Tools.Song_GetaGrip = Tools.SongWad.FindByName("Get_a_Grip^Peacemaker");
            Tools.Song_GetaGrip.Volume = 1f;

            Tools.Song_House = Tools.SongWad.FindByName("House^Blind_Digital");
            Tools.Song_House.Volume = .7f;

            Tools.Song_Nero = Tools.SongWad.FindByName("Nero's_Law^Peacemaker");
            Tools.Song_Nero.Volume = 1f;

            Tools.Song_Ripcurl = Tools.SongWad.FindByName("Ripcurl^Blind_Digital");
            Tools.Song_Ripcurl.Volume = .7f;

            Tools.Song_FatInFire = Tools.SongWad.FindByName("The_Fat_is_in_the_Fire^Peacemaker");
            Tools.Song_FatInFire.Volume = .9f;

            Tools.Song_Heavens = Tools.SongWad.FindByName("The_Heavens_Opened^Peacemaker");
            Tools.Song_Heavens.Volume = 1f;

            Tools.Song_TidyUp = Tools.SongWad.FindByName("Tidy_Up^Peacemaker");
            Tools.Song_TidyUp.Volume = 1f;

            Tools.Song_WritersBlock = Tools.SongWad.FindByName("Writer's_Block^Peacemaker");
            Tools.Song_WritersBlock.Volume = 1f;

            // Create the standard playlist
            Tools.SongList_Standard.AddRange(Tools.SongWad.SongList);
            Tools.SongList_Standard.Remove(Tools.Song_Happy);
            Tools.SongList_Standard.Remove(Tools.Song_140mph);
        }

        static void LoadSound(bool CreateNewWad)
        {
            ContentManager manager = new ContentManager(Tools.GameClass.Content.ServiceProvider, Tools.GameClass.Content.RootDirectory);

            if (CreateNewWad)
            {
                Tools.SoundWad = new EzSoundWad(4);
                Tools.PrivateSoundWad = new EzSoundWad(4);
            }

            string path = Path.Combine(Globals.ContentDirectory, "Sound");
            string[] files = Directory.GetFiles(path);
            foreach (String file in files)
            {
                int i = file.IndexOf("Sound") + 5 + 1;
                if (i < 0) continue;
                int j = file.IndexOf(".", i);
                if (j <= i) continue;
                String name = file.Substring(i, j - i);
                String extension = file.Substring(j + 1);

                if (extension == "xnb")
                {
                    if (CreateNewWad)
                        Tools.SoundWad.AddSound(manager.Load<SoundEffect>("Sound\\" + name), name);
                    else
                    {
                        SoundEffect NewSound = manager.Load<SoundEffect>("Sound\\" + name);

                        EzSound CurSound = Tools.SoundWad.FindByName(name);
                        foreach (EzSound ezsound in Tools.PrivateSoundWad.SoundList)
                            if (ezsound.sound == CurSound.sound)
                                ezsound.sound = NewSound;
                        CurSound.sound = NewSound;
                    }
                }

                ResourceLoadedCountRef.MyFloat++;
            }

            if (Tools.SoundContentManager != null)
            {
                Tools.SoundContentManager.Unload();
                Tools.SoundContentManager.Dispose();
            }

            Tools.SoundContentManager = manager;
        }

        public static void LoadAssets(bool CreateNewWads)
        {
            ResourceLoadedCountRef = new WrappedFloat();
            LoadingResources = new WrappedBool(false);
            LoadingResources.MyBool = true;

            // Fonts
            FontLoad();

            // Load the art!
            PreloadArt();

            // Load the music!
            LoadMusic(CreateNewWads);

            // Load the sound!
            LoadSound(CreateNewWads);
        }

        static void PreloadArt()
        {
            String path = Path.Combine(Globals.ContentDirectory, "Art");
            string[] files = Tools.GetFiles(path, true);

            foreach (String file in files)
            {
                if (Tools.GetFileExt(path, file) == "xnb")
                {
                    Tools.TextureWad.AddTexture(null, "Art\\" + Tools.GetFileName(path, file));
                }
            }
        }

        public static void LoadResources()
        {
            LoadThread = Tools.EasyThread(5, "LoadThread", _LoadThread);
        }

        public static Thread LoadThread;
        static void _LoadThread()
        {
            var Ck = Tools.TheGame;

            Tools.Write(string.Format("Load thread starts at {0}", System.DateTime.Now));

            Thread.SpinWait(100);

            Tools.Write("Start");

            // Initialize the Gamepads
            Tools.GamepadState = new GamePadState[4];
            Tools.PrevGamepadState = new GamePadState[4];

            // Fireball texture
            Fireball.PreInit();

            // Load art
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Environments");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Bob");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Buttons");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Characters");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Coins");
            //Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Effects");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "HeroItems");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "LoadScreen_Initial");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "LoadScreen_Level");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Menu");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Old_Art_Holdover");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Title");
            
            Tools.Write("ArtMusic done...");

            // Load the tile info
            LoadInfo();
            TileSets.Init();

            Fireball.InitRenderTargets(Ck.MyGraphicsDevice, Ck.MyGraphicsDevice.PresentationParameters, 300, 200);

            ParticleEffects.Init();

            Awardments.Init();

#if NOT_PC && (XBOX || XBOX_SIGNIN)
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
#endif


#if PC_VERSION
            // Mouse pointer
            Ck.MousePointer = new QuadClass();
            Ck.MousePointer.Quad.MyTexture = Tools.TextureWad.FindByName("Hand_Open");
            Ck.MousePointer.ScaleYToMatchRatio(70);

            // Mouse back icon
            Ck.MouseBack = new QuadClass();
            Ck.MouseBack.Quad.MyTexture = Tools.TextureWad.FindByName("charmenu_larrow_1");
            Ck.MouseBack.ScaleYToMatchRatio(40);
            Ck.MouseBack.Quad.SetColor(new Color(255, 150, 150, 100));
#endif

            Prototypes.LoadObjects();
            ObjectIcon.InitIcons();

            Console.WriteLine("Total resources: {0}", ResourceLoadedCountRef.MyFloat);

            // Note that we are done loading.
            LoadingResources.MyBool = false;
            Tools.Write("Loading done!");

            Tools.Write(string.Format("Load thread done at {0}", System.DateTime.Now));
        }
    }
}