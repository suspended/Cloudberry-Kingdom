using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;
using CloudberryKingdom.InGameObjects;
using CloudberryKingdom.Obstacles;

#if PC
using SteamManager;
#endif

namespace CloudberryKingdom
{
    public enum MainMenuTypes { PC, Xbox, PS3, WiiU, Vita };

    public partial class CloudberryKingdomGame
    {
#if PC
        // Load graphic resources.
        // Set this to true if you have all the resources for the full CK release.
        // Set this to false otherwise.
        // The game will crash if you attempt to load resources but do not have them.
        public const bool LoadResources = true;

        // Steam Integration
        // Set this to true to turn on Steam Integrtaion code
        public const bool UsingSteam = false;
        public static bool SteamInitialized = false;
        public static bool SteamAvailable
        {
            get
            {
                return UsingSteam && SteamInitialized;
            }
        }
#endif

#if DEBUG
        public const bool FinalRelease = false;
#else
        public const bool FinalRelease = true;
#endif

        public const bool AllowAsianLanguages = true;

        public const string VersionString = "2.0.0000";
        public const bool CodersEdition = true;
        public static bool AndersSwitch = false;
        public static bool BungeeSwitch = true;
        public const bool AllowReturnToAttractScreen = false;

        public static bool AlwaysBungee
        {
            get
            {
                return BungeeSwitch && PlayerManager.GetNumPlayers() > 1;
            }
        }

        /// <summary>
        /// The version of the game we are working on now (+1 over the last version uploaded to Steam).
        /// MajorVersion is 0 for beta, 1 for release.
        /// MinorVersion increases with substantial change.
        /// SubVersion increases with any pushed change.
        /// </summary>
        public static Version GameVersion = new Version(0, 2, 4);


        public static bool GodMode = !FinalRelease;
        public static bool AsianButtonSwitch = false;

#if PC || MONO
        // Steam Beta
        //public static bool HideLogos = true;
        //public static bool LockCampaign = true;
        //public static bool SimpleMainMenu = true;
        //public static bool PS3MainMenu = false;
        //public static bool PS3MainMenu = false;
        //public static bool SimpleLeaderboards = true;
        //public static bool FakeAwardments = false;

        // PC Beta
#if DEBUG
        public static bool HideLogos = true;
        public static bool QuickStart = true;
#else
        public static bool HideLogos = false;
        public static bool QuickStart = false;
#endif
        public static bool LockCampaign = false;
        public static bool SimpleMainMenu = true;
        public static MainMenuTypes MainMenuType = MainMenuTypes.PC;
        public static bool SimpleLeaderboards = false;
        public static bool FakeAwardments = false;
        public static float GuiSqueeze = 0;

        // Steam Beta
        //public static bool HideLogos = false;
        //public static bool LockCampaign = true;
        //public static bool SimpleMainMenu = true;
        //public static MainMenuTypes MainMenuType = MainMenuTypes.PC;
        //public static bool SimpleLeaderboards = false;
        //public static bool FakeAwardments = false;
        //public static float GuiSqueeze = 0;
#elif XBOX
        public static bool HideLogos = false;
        public static bool LockCampaign = false;
        public static bool SimpleMainMenu = false;
        public static MainMenuTypes MainMenuType = MainMenuTypes.Xbox;
        public static bool SimpleLeaderboards = false;
        public static bool FakeAwardments = false;
        public static float GuiSqueeze = .3f;
#elif CAFE
        public static bool HideLogos = false;
        public static bool LockCampaign = false;
        public static bool SimpleMainMenu = true;
        public static MainMenuTypes MainMenuType = MainMenuTypes.WiiU;
        public static bool SimpleLeaderboards = true;
        public static bool FakeAwardments = false;
        public static float GuiSqueeze = 1f;
#elif PS3
        public static bool HideLogos = false;
        public static bool LockCampaign = false;
        public static bool SimpleMainMenu = false;
        public static MainMenuTypes MainMenuType = MainMenuTypes.PS3;
        public static bool SimpleLeaderboards = false;
        public static bool FakeAwardments = false;
        public static float GuiSqueeze = 1f;
#endif


        public static int SuppressSavingTextDuration = 0;
        public static Text SavingText = null;
        public static int ShowSavingDuration = 0;
        const int ShowSavingLength = 80;

        const float ShowSaving_FadeInLength = 6;
        const float ShowSaving_FadeOutLength = 9;

        public static void ShowSaving()
        {
            if (Localization.CurrentLanguage == null ||
                Tools.TheGame == null || 
                Resources.Font_Grobold42 == null ||
                Resources.Font_Grobold42.HFont == null ||
                Resources.Font_Grobold42.HFont.font == null ||
                Resources.Font_Grobold42.HFont.font.MyTexture == null ||
                Resources.Font_Grobold42.HOutlineFont == null ||
                Resources.Font_Grobold42.HOutlineFont.font == null ||
                Resources.Font_Grobold42.HOutlineFont.font.MyTexture == null)
            {
                return;
            }

            //if (PlayerManager.GetNumPlayers() > 2)
            {
                if (ShowSavingDuration > 0)
                    ShowSavingDuration = (int)(ShowSavingLength - ShowSaving_FadeInLength);
                else
                    ShowSavingDuration = ShowSavingLength;

                SavingText = new Text(Localization.Words.Saving, Resources.Font_Grobold42, false);
                SavingText.FixedToCamera = true;
                SavingText.Pos = new Vector2(1110, -750);
                SavingText.Scale = .37f;
                StartMenu.SetTextSelected_Red(SavingText);
            }
        }

        void DrawSavingText()
        {
            if (SuppressSavingTextDuration > 0)
            {
                SuppressSavingTextDuration--;
                ShowSavingDuration = 0;
                return;
            }

            // Use this to test always drawing the save text
            //ShowSaving();

            if (ShowSavingDuration > 0)
            {
                ShowSavingDuration--;

                if (Tools.CurCamera != null)
                {
                    if (Tools.CurCamera.Zoom.X != .001f)
                    {
                        Tools.CurCamera.SetToDefaultZoom();
                    }

                    if (ShowSavingDuration < ShowSaving_FadeOutLength)
                        SavingText.Alpha = 1f - (ShowSaving_FadeOutLength - ShowSavingDuration) / ShowSaving_FadeOutLength;
                    else if (ShowSavingDuration > ShowSavingLength - ShowSaving_FadeInLength)
                        SavingText.Alpha = 1f - (ShowSaving_FadeInLength - (ShowSavingLength - ShowSavingDuration)) / ShowSaving_FadeInLength;

                    SavingText.Draw(Tools.CurCamera);
                    Tools.QDrawer.Flush();
                }
            }
        }






        public static bool ForFrapsRecording = false;

#if DEBUG
        public static bool AlwaysGiveTutorials = true;
        public static bool Unlock_Customization = true;
        public static bool Unlock_Levels = false;
        public static bool UnlockHeroesAndGames = true;
#else
        public static bool AlwaysGiveTutorials = false;
        public static bool Unlock_Customization = true;
        public static bool Unlock_Levels = false || DigitalDayBuild;
        public static bool UnlockHeroesAndGames = true;
#endif

        public static bool ChoseNotToSave = false;
        public static bool PastPressStart = false;
        public static bool CanSave()
        {
            if (IsDemo) return false;

            if (ChoseNotToSave) return false;

            if (!PastPressStart) return false;

            return true;
        }

        public static bool CanSave(PlayerIndex index)
        {
            if (!CanSave()) return false;

            return true;
        }

        public static void ChangeSaveGoFunc(MenuItem item)
        {
            if (!CloudberryKingdomGame.CanSave())
            {
                //item.Selectable = false;
                item.Go = null;

                item.MyText.MyFloatColor.W = .5f;
                item.MySelectedText.MyFloatColor.W = .5f;
            }
            else
            {
                item.Selectable = true;
                item.Go = Cast.ToItem(ShowError_CanNotSaveLevel_NoSpace);

                item.MyText.MyFloatColor.W = .5f;
                item.MySelectedText.MyFloatColor.W = .5f;
            }
        }

        public static void ShowError_CanNotSaveLevel_NoSpace()
        {
            ShowError(Localization.Words.Err_CanNotSaveLevel_NoSpace_Header, Localization.Words.Err_CanNotSaveLevel_NoSpace, Localization.Words.Err_Ok, null);
        }

        public static void ShowError_CanNotSaveNoDevice()
        {
            ShowError(Localization.Words.Err_StorageDeviceRequired, Localization.Words.Err_NoSaveDevice, Localization.Words.Err_Ok, null);
        }


        public static bool ProfilesAvailable()
        {
            return true;
        }

        public static bool OnlineFunctionalityAvailable(PlayerIndex index)
        {
            Tools.Write("Checking online functionality for player " + index);

            return true;
        }

        public static bool OnlineFunctionalityAvailable()
        {
            return true;
        }

        public static void BeginShowMarketplace()
        {
        }

        public enum Presence { TitleScreen, Escalation, TimeCrisis, HeroRush, HeroRush2, Freeplay, Campaign, Arcade, Madness };
        public static Presence CurrentPresence = Presence.TitleScreen;
        public static void SetPresence(Presence presence)
        {
            CurrentPresence = presence;

            Tools.Warning();
            return;
        }

        public static bool GuideIsTrial_Override = true && !FinalRelease;
        public static bool Fake_GuideIsTrial = true && !FinalRelease;

        public static bool DoTrialUnlockEvent = false;
        public static bool WasNotDemoOnce = false;
        public static bool FakeDemo = false && !FinalRelease;
        public static bool IsDemo
        {
            get
            {
                // Always do full version
                //if (!FinalRelease) { Tools.Warning(); IsTrial = false; return false; }
                //Tools.Warning(); IsTrial = false; return false;


#if DEBUG
                return FakeDemo;
#endif

                if (WasNotDemoOnce) return false;
                if (FakeDemo) return true;

                WasNotDemoOnce = true; // Once this is set to true the game will always think it is a full version until restarted.
                return false;
            }
        }
        public static int Freeplay_Count = 0;
        public static int Freeplay_Max = 3;

        public static void OfferToBuy()
        {
        }

        /// <summary>
        /// The command line arguments.
        /// </summary>
        public static string[] args;

        public static bool StartAsBackgroundEditor = false;
        public static bool StartAsTestLevel = false;
        public static bool StartAsBobAnimationTest = false;
        public static bool StartAsFreeplay = false;
#if INCLUDE_EDITOR
        public static bool LoadDynamic = true;
#else
        public static bool LoadDynamic = false;
#endif
        public static bool ShowSongInfo = true;

        public static string TileSetToTest = "cave";
        public static string ModRoot = "Standard";
        public static bool AlwaysSkipDynamicArt = false;

        public static bool HideGui = false;
        public static bool HideForeground = false;
        public static bool UseNewBob = false;

#if DEBUG
        public static bool OutputLoadingInfo = false;
#endif

        //public static SimpleGameFactory TitleGameFactory = TitleGameData_Intense.Factory;
        //public static SimpleGameFactory TitleGameFactory = TitleGameData_MW.Factory;
        public static SimpleGameFactory TitleGameFactory = TitleGameData_Clouds.Factory;
        //public static SimpleGameFactory TitleGameFactory = TitleGameData_Forest.Factory;

        public static float fps;

        public int DrawCount, PhsxCount;

        public ResolutionGroup Resolution;
        public ResolutionGroup[] Resolutions = new ResolutionGroup[4];

#if PC
        public QuadClass MousePointer, MouseBack;
        bool _DrawMouseBackIcon = false;
        public bool DrawMouseBackIcon { get { return _DrawMouseBackIcon; } set { _DrawMouseBackIcon = value; } }
#endif

        bool LogoScreenUp;

        /// <summary>
        /// When true the initial loading screen is drawn even after loading is finished
        /// </summary>
        public bool LogoScreenPropUp;

        /// <summary>
        /// The game's initial loading screen. Different than the in-game loading screens seen before levels.
        /// </summary>
        public InitialLoadingScreen LoadingScreen;

        public GraphicsDevice MyGraphicsDevice;
        public GraphicsDeviceManager MyGraphicsDeviceManager;

        int ScreenWidth, ScreenHeight;

        Camera MainCamera;

        public event EventHandler<EventArgs> Exiting
        {
            add
            {
                Tools.GameClass.Exiting += value;
            }
            remove
            {
                Tools.GameClass.Exiting -= value;
            }
        }

        public void Exit()
        {
            if (UsingSteam)
            {
                SteamCore.Shutdown();
            }

            Tools.GameClass.Exit();
        }

        /// <summary>
        /// Process the command line arguments.
        /// This is used to load different tools, such as the background editor, instead of the main game.
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessArgs(string[] args)
        {
#if DEBUG
            // Artifically simulate different command line arguments.
            //args = new string[] { "test_bob_animation", "mod_root", "Bob" };
            //args = new string[] { "test_level" }; AlwaysSkipDynamicArt = true;
            //args = new string[] { "background_editor" }; //AlwaysSkipDynamicArt = true;
            //args = new string[] { "test_all" }; AlwaysSkipDynamicArt = false;
            //StartAsTestLevel = true;
#endif
            //args = new string[] { "test_all" }; AlwaysSkipDynamicArt = false;
            
            LoadDynamic = true;
            AlwaysSkipDynamicArt = false;


            CloudberryKingdomGame.args = args;

            var list = new List<string>(args); list.Reverse();
            var stack = new Stack<string>(list);
            
            while (stack.Count > 0)
            {
                var arg = stack.Pop();

                switch (arg)
                {
                    case "background_editor": StartAsBackgroundEditor = true; LoadDynamic = true; break;
                    case "test_level": StartAsTestLevel = true; LoadDynamic = true; break;
                    case "test_bob_animation": StartAsBobAnimationTest = true; LoadDynamic = false; break;
                    case "test_all":
                        ShowSongInfo = false;
                        UseNewBob = true;
                        StartAsFreeplay = true; LoadDynamic = true; break;
                    case "test_all_old_bob":
                        ShowSongInfo = false;
                        StartAsFreeplay = true; LoadDynamic = true; break;

                    case "mod_root":
                        LoadDynamic = true;
                        if (stack.Count > 0)
                            ModRoot = stack.Pop();
                        break;

                    default: break;
                }
            }
        }

        public CloudberryKingdomGame()
        {
            MyGraphicsDeviceManager = new GraphicsDeviceManager(Tools.GameClass);
            MyGraphicsDeviceManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            CoreGamepad.Initialize(Tools.GameClass.Services, Tools.GameClass.Components, Tools.GameClass.Window.Handle);

            Tools.GameClass.Content.RootDirectory = "Content";

            Tools.TheGame = this;
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
        }

        static bool ExitingEarly = false;
        public void Initialize()
        {
            if (CloudberryKingdomGame.UsingSteam)
            {
                Console.WriteLine("Using Steam, checking if restart is needed.");

                if (SteamCore.RestartViaSteamIfNecessary(210870))
                {
                    Console.WriteLine("Restart is needed.");

                    ExitingEarly = true;
                    Tools.GameClass.Exit();
                    return;
                }

                Console.WriteLine("Initializing Steam.");
                SteamInitialized = SteamCore.Initialize();
                Console.WriteLine("Steam initialization: {0}", SteamInitialized ? "Success" : "Failed");
            }

#if WINDOWS
            KeyboardHandler.EventInput.Initialize(Tools.GameClass.Window);
#endif
            Globals.ContentDirectory = Tools.GameClass.Content.RootDirectory;

            Tools.LoadEffects(Tools.GameClass.Content, true);
        }

        public void InitialResolution()
        {
            Tools.Write("InitialResolution");
#if PC
            // The PC version let's the player specify resolution, key mapping, and so on.
            // Try to load these now.
            PlayerManager.RezData rez;

            PlayerManager.Players = new PlayerData[4];
            PlayerManager.Players[0] = new PlayerData();
            PlayerManager.Players[0].Init();

            PlayerManager.Player.ContainerName = "SaveData";
            PlayerManager.Player.FileName = "SaveData.bam";
            PlayerManager.Player.Load(PlayerManager.Player.MyPlayerIndex);

            SaveGroup.LoadAll();
            PlayerManager.Player.Load(PlayerIndex.One);

            rez = PlayerManager.LoadRezAndKeys();

            if (!rez.Custom)
            {
                rez.Width = 1280;
                rez.Height = 720;
                rez.Mode = WindowMode.Borderless;

                rez.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                rez.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

#if DEBUG || INCLUDE_EDITOR
            rez.Mode = WindowMode.Windowed;
            rez.Width = 1280;
            rez.Height = 720;
#endif

            Resolution = new ResolutionGroup();
            Resolution.Backbuffer = new IntVector2(rez.Width, rez.Height);
            Resolution.Bob = new IntVector2(135, 0);
            Resolution.TextOrigin = Vector2.Zero;
            Resolution.LineHeightMod = 1f;

            MyGraphicsDeviceManager.PreferredBackBufferWidth = rez.Width;
            MyGraphicsDeviceManager.PreferredBackBufferHeight = rez.Height;
            MyGraphicsDeviceManager.IsFullScreen = rez.Mode == WindowMode.Fullscreen;

            if (rez.Mode == WindowMode.Windowed)
            {
                MyGraphicsDeviceManager.PreferredBackBufferHeight = (int)((720f / 1280f) * rez.Width);
            }

#endif

            Tools.Mode = rez.Mode;

            fps = 0;
            Tools.Write("BackBuffer set");
        }

        public void LoadContent()
        {
            if (ExitingEarly)
                return;

            Tools.Write("Inside LoadContent");
            
            MyGraphicsDevice = MyGraphicsDeviceManager.GraphicsDevice;
            Tools.Write("MyGraphicsDevice set");

            AdditiveColor_NormalAlpha = new BlendState();
            AdditiveColor_NormalAlpha.ColorBlendFunction = BlendFunction.Add;
            AdditiveColor_NormalAlpha.ColorDestinationBlend = Blend.One;            
            AdditiveColor_NormalAlpha.ColorSourceBlend = Blend.One;
            AdditiveColor_NormalAlpha.AlphaBlendFunction = BlendFunction.Add;
            AdditiveColor_NormalAlpha.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            AdditiveColor_NormalAlpha.AlphaSourceBlend = Blend.One;
            Tools.Write("BlendState made");

            Tools.LoadBasicArt(Tools.GameClass.Content);
            Tools.Write("LoadBasicArt done");

            Tools.Render = new MainRender(MyGraphicsDevice);
            Tools.Write("Render made");

            Tools.QDrawer = new QuadDrawer(MyGraphicsDevice, 2000);
            Tools.QDrawer.DefaultEffect = Tools.EffectWad.FindByName("NoTexture");
            Tools.QDrawer.DefaultTexture = Tools.TextureWad.FindByName("White");
            Tools.Write("QDrawer made");

            Tools.Device = MyGraphicsDevice;
            Tools.t = 0;

            LogoScreenUp = true;

            ScreenWidth = MyGraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = MyGraphicsDevice.PresentationParameters.BackBufferHeight;

            MainCamera = new Camera(ScreenWidth, ScreenHeight);
            Tools.Write("Camera made");

            MainCamera.Update();
            Tools.Write("Camera updated");

            CoreGamepad.OnLoad();

            Tools.Write("Gamepads made");

            Preload ();
        }

        public static void MaxCampaign()
        {
            if (!SteamInitialized)
            {
                return;
            }

            SteamStats.FindLeaderboard("Story Mode", (handle, failed) => OnFindLeaderboard_Read(handle, failed));
        }

        static bool ReadingInProgress = false;
        static void OnFindLeaderboard_Read(LeaderboardHandle Handle, bool failed)
        {
            Console.WriteLine("Find Leaderboard to read from. Failed? : {0}", failed);

            if (failed)
            {
                ReadingInProgress = false;
            }
            else
            {
                SteamStats.RequestEntries(Handle, SteamStats.LeaderboardDataRequestType.GlobalAroundUser, 0, 10000,
                    b => OnInfo(Handle, b));
            }
        }

        static void OnInfo(LeaderboardHandle Handle, bool failed)
        {
            if (failed)
            {
                ReadingInProgress = false;

                return;
            }

            int NumEntriesFound = SteamStats.NumEntriesFound();

            int max_campaign = 0;
            for (int i = 0; i < NumEntriesFound; i++)
            {
                int rank = SteamStats.Results_GetRank(i);
                int val = SteamStats.Results_GetScore(i);
                Gamer gamer = new Gamer(SteamStats.Results_GetName(i), SteamStats.Results_GetId(i));

                if (gamer.Gamertag == SteamCore.PlayerName())
                {
                    max_campaign = Math.Max(max_campaign, val);
                }
            }

            int max_index = 0;
            if (max_campaign > 0)
            {
                foreach (var pair in CampaignSequence.Instance.LevelIndexPairs)
                {
                    if (max_campaign > pair.Item1)
                        max_index = Math.Max(max_index, pair.Item2);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var player = PlayerManager.Players[i];
                if (player == null) continue;

                player.CampaignLevel = Math.Max(player.CampaignLevel, max_campaign);
                player.CampaignIndex = Math.Max(player.CampaignIndex, max_index);
                player.Changed = true;
            }

            ReadingInProgress = false;
        }

        void Preload()
        {
            Tools.Write("Preload");

            ButtonString.Init();
            ButtonCheck.Reset();

            // Fill the pools
            ComputerRecording.InitPool();

            // Initialize players
            PlayerManager.Init();

            // Localization
            Tools.Write("Language ISO code    : " + System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            Tools.Write("Language ISO code(3) : " + System.Globalization.CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
            Localization.Language default_language = Localization.IsoCodeToLanguage(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            Tools.Write("Default language is " + default_language);
            Localization.SetLanguage(default_language);
            Tools.Write("Language loaded");

            // Load saved files
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            SaveGroup.Initialize();

#if PC
            StartLogoSalad();
#else
            StartLogoSalad();
#endif

            // Pre load. This happens before anything appears.
            Resources.LoadAssets();
            Tools.Write("Asset preload complete.");

            MaxCampaign();

            // Create the initial loading screen
            LoadingScreen = new InitialLoadingScreen(Tools.GameClass.Content, Resources.ResourceLoadedCountRef);

            // Initialize heroes
            BobPhsx.CustomPhsxData.InitStatic();

            HookSignInAndOut();
            Tools.Write("Sign in and out now hooked.");

            // Fireball texture
            Fireball.PreInit();

            // Set textures to be transparent until loaded.
            Tools.Write("Dump dummy texture into each texture.");
            for (int i = 0; i < Tools.TextureWad.TextureList.Count; i++)
            {
                var tex = Tools.TextureWad.TextureList[i];
                tex.Tex = Tools.Transparent.Tex;

                Resources.ResourceLoadedCountRef.Val++;
            }
            Tools.Write("Dummy fill complete.");

            // Load resource thread
            Resources.LoadResources();
        }

        private void StartLogoSalad()
        {
            if (!HideLogos)
            {
                XnaVideo.StartVideo("LogoSalad", false, 3.45f);
            }

            PreloadDone = true;
        }

        public static bool PreloadDone = false;

        void HookSignInAndOut()
        {
        }

        protected void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        void DoQuickSpawn()
        {
            if (Tools.CurLevel.ResetEnabled() && Tools.CurLevel.PlayMode == 0 && !Tools.CurLevel.Watching && !Tools.CurGameData.PauseGame && Tools.CurGameData.QuickSpawnEnabled())
            {
                // Note that quickspawn was used, hence we should not give hints to the player to use Quick Spawn in the future.
                Hints.SetQuickSpawnNum(999);

                // Don't count reset against player if it happens within the first half second
                if (Tools.CurLevel.CurPhsxStep < 30)
                    Tools.CurLevel.FreeReset = true;

                // Update player stats
                Tools.CurLevel.CountReset();

                Tools.CurLevel.SetToReset = true;
            }
        }

        /// <summary>
        /// The current game being played.
        /// </summary>
        GameData Game { get { return Tools.CurGameData; } }

        /// <summary>
        /// Update the current game.
        /// </summary>
        void DoGameDataPhsx()
        {
#if INCLUDE_EDITOR
            if (Tools.EditorPause) return;
#endif
            Tools.PhsxCount++;

            if (Tools.WorldMap != null)
                Tools.WorldMap.BackgroundPhsx();

            if (Game != null)
            {
                for (int i = 0; i < Game.PhsxStepsToDo; i++)
                {
                    CoreSoundWad.SuppressSounds = (Game.SuppressSoundForExtraSteps && i < Game.PhsxStepsToDo - 1);
                    Game.PhsxStep();
                }
                Game.PhsxStepsToDo = 1;
            }
        }

        /// <summary>
        /// A list of actions to perform. Each time an action is peformed it is removed from the list.
        /// </summary>
        public List<Action> ToDo = new List<Action>();

        private void DoToDoList()
        {
            foreach (Action todo in ToDo)
                todo();
            ToDo.Clear();
        }

        protected void GodModePhxs()
        {
            //return;

            // Write to leaderboard
            Tools.Warning();
            if (ButtonCheck.State(ControllerButtons.RJ, -2).Down && ButtonCheck.State(ControllerButtons.LS, -2).Pressed)
            {
                var se = new ScoreEntry(null, 0, 100, 200, 300, 400, 500, 600);
                Leaderboard.WriteToLeaderboard(se);
            }


            // Give 100,000 points to each player
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.I) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.I))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.RJ, -2).Pressed)
#endif
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                    bob.MyStats.Score += 100000;
            }

            // Kill everyone but Player One
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.U) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.U))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.X, -2).Down)
#endif
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                {
                    if (bob.MyPlayerIndex != 0 && !(bob.Dead || bob.Dying))
                    {
                        //Fireball.Explosion(bob.Core.Data.Position, bob.Core.MyLevel);
                        //Fireball.ExplodeSound.Play();

                        //bob.Core.Show = false;
                        //bob.Dead = true;

                        bob.Die(Bob.BobDeathType.Other, true, false);
                    }
                }
            }

            // Turn on/off trial.
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.D) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.D))
#else
            if (ButtonCheck.State(ControllerButtons.RJ, -2).Down && ButtonCheck.State(ControllerButtons.LS, -2).Pressed)
#endif
            {
                FakeDemo = !FakeDemo;
            }

            // Turn on/off flying.
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.O) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.O))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.A, -2).Pressed)
#endif
            {
                if (!Tools.ViewerIsUp)
                {
                    foreach (Bob bob in Tools.CurLevel.Bobs)
                    {
                        bob.Flying = !bob.Flying;
                        bob.Immortal = !bob.Immortal;
                    }
                }
            }

            if (Tools.Keyboard.IsKeyDownCustom(Keys.T) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.T))
            {
                Tools.HidGui = !Tools.HidGui;
            }

            // Go to last door
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.P) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.P))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.B, -2).Down)
#endif
            {
                // Find last door
                if (Tools.CurLevel != null && !Tools.ViewerIsUp)
                {
                    Door door = Tools.CurLevel.FindIObject(LevelConnector.EndOfLevelCode) as Door;

                    if (null != door)
                    {
                        foreach (Bob bob in Tools.CurLevel.Bobs)
                        {
                            bob.Immortal = true;
                            Tools.MoveTo(bob, door.Pos);
                        }

                        foreach (ObjectBase obj in Tools.CurLevel.Objects)
                        {
                            Coin coin = obj as Coin;
                            if (null != coin)
                            {
                                Tools.MoveTo(coin, door.Pos);
                            }

                            CameraZone zone = obj as CameraZone;
                            if (null != zone)
                            {
                                if (Tools.CurLevel.MainCamera.MyZone == null ||
                                    Tools.CurLevel.MainCamera.MyZone.Box.Current.BL.X <= zone.Box.Current.BL.X)
                                {
                                    Tools.CurLevel.MainCamera.MyZone = zone;
                                    Tools.CurLevel.MainCamera.Pos = zone.End;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void PhsxStep()
        {
            DoToDoList();
#if WINDOWS
    #if PC_DEBUG || (WINDOWS && DEBUG) || INCLUDE_EDITOR
            // Debug tools
            if (DebugModePhsx())
                return;
    #endif

    #if DEBUG
            GodModePhxs();
    #else
            if (GodMode)
                GodModePhxs();
    #endif

            // Do game update.
            if (!Tools.StepControl || (Tools.Keyboard.IsKeyDownCustom(Keys.Enter) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.Enter)))
            {
                DoGameDataPhsx();
            }
            else if (Tools.CurLevel != null)
                Tools.CurLevel.IndependentDeltaT = 0;

    #if WINDOWS
            // Quick Spawn
            CheckForQuickSpawn_PC();
    #endif
#else
    #if DEBUG
            GodModePhxs();
    #else
            if (GodMode)
                GodModePhxs();
    #endif

            DoGameDataPhsx();
#endif

            // Quick Spawn: Note, we must check this for PC version too, since PC players may use game pads.
            CheckForQuickSpawn_Xbox();

            // Finish updating the controlls; swap current to previous.
            ButtonCheck.UpdateControllerAndKeyboard_EndOfStep(Resolution);

            // Update the fireball textures.
            Fireball.TexturePhsx();
        }
        
#if WINDOWS
        private void CheckForQuickSpawn_PC()
        {
            // Should implement a GameObject that marshalls quickspawns instead.
            Tools.Warning();

            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze && Tools.CurLevel.ResetEnabled() && !ButtonCheck.PreventNextInput &&
                Tools.Keyboard.IsKeyDownCustom(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey) && !Tools.PrevKeyboard.IsKeyDownCustom(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey))
                DoQuickSpawn();
        }
#endif

        private void CheckForQuickSpawn_Xbox()
        {
            // Check for quick spawn on Xbox. This allows the player to reset a level rapidly.
            // For XBox this is done by holding both shoulder buttons.
            bool ShortReset = false;
            for (int i = 0; i < 4; i++)
            {
                if (PlayerManager.Get(i).Exists)
                {
                    if (CoreGamepad.IsPressed(i, ControllerButtons.LS) && CoreGamepad.IsPressed(i, ControllerButtons.RS) &&
                        !(CoreGamepad.IsPreviousPressed(i, ControllerButtons.LS) && CoreGamepad.IsPreviousPressed(i, ControllerButtons.RS)))
                        ShortReset = true;
                }
            }

            // Do the quick spawn if it was chosen by the player.
            if (ShortReset)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (PlayerManager.Get(i).Exists && PlayerManager.Get(i).IsAlive)
                    {
                        if (!CoreGamepad.IsPressed(i, ControllerButtons.LS) && !CoreGamepad.IsPressed(i, ControllerButtons.RS))
                            ShortReset = false;
                    }
                }

                if (ShortReset && Tools.CurLevel.ResetEnabled())
                    DoQuickSpawn();
                else
                {
                    // Otherwise do individual quickspawns
                    if (Tools.CurLevel != null)
                    for (int i = 0; i < 4; i++)
                    {
                        if (PlayerManager.Get(i).Exists && PlayerManager.Get(i).IsAlive)

                        if (CoreGamepad.IsPressed(i, ControllerButtons.LS) && CoreGamepad.IsPressed(i, ControllerButtons.RS) &&
                            !(CoreGamepad.IsPreviousPressed(i, ControllerButtons.LS) && CoreGamepad.IsPreviousPressed(i, ControllerButtons.RS)))
                        {
                            foreach (Bob bob in Tools.CurLevel.Bobs)
                            {
                                if (bob.MyPlayerIndex == PlayerManager.Get(i).MyPlayerIndex && bob.ImmortalCountDown <= 0)
                                {
                                    ParticleEffects.PopOut(Tools.CurLevel, bob.Pos);

                                    //bob.Die(Bob.BobDeathType.Other);
                                    bob.Die(Bob.BobDeathType.Other, null, false, false);
                                    Tools.SetVibration(PlayerManager.Get(i).MyPlayerIndex, 0, 0, 0);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

#if WINDOWS
        /// <summary>
        /// Whether the mouse should be allowed to be shown, usually only when a menu is active.
        /// </summary>
        public bool ShowMouse = false;

        /// <summary>
        /// Draw the mouse cursor.
        /// </summary>
        void MouseDraw()
        {
            if (!ButtonCheck.MouseInUse) return;
            if (MousePointer == null) return;

            Vector2 Pos = Tools.MouseWorldPos();

            // Draw the mouse hand
            //MousePointer.Pos = Pos + new Vector2(.905f * MousePointer.Size.X, -.705f * MousePointer.Size.Y);

            MousePointer.ScaleYToMatchRatio(33.3f);

            MousePointer.Pos = Pos + new Vector2(.91f * MousePointer.Size.X, -.93f * MousePointer.Size.Y);

            MousePointer.Draw();

            // Draw the mouse dot
            //Tools.QDrawer.DrawSquareDot(Pos, Color.Black, 8);

            // Draw the mouse back icon
            //if (DrawMouseBackIcon)
            //{
            //    MouseBack.Pos = MousePointer.Pos + new Vector2(44, 98);
            //    MouseBack.Draw();
            //}

            Tools.QDrawer.Flush();
        }
#endif

        /// <summary>
        /// Whether a song was playing prior to the game window going inactive
        /// </summary>
        public bool MediaPlaying_HoldState = false;

        /// <summary>
        /// Whether a video was playing prior to the game window going inactive
        /// </summary>
        public bool VideoPlaying_HoldState = false;

        /// <summary>
        /// Whether this is the first frame the window has been inactive
        /// </summary>
        public bool FirstInactiveFrame = true;

        /// <summary>
        /// Whether this is the first frame the window has been active
        /// </summary>
        public bool FirstActiveFrame = true;

        public double DeltaT = 0;

        public bool RunningSlowly = false;

        TimeSpan TargetElapsedTime_58fps = new TimeSpan(0, 0, 0, 0, (int)(1000f / 58f));
        TimeSpan TargetElapsedTime_60fps = new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f));

        public void Update()
        {
            if (MyGraphicsDeviceManager.IsFullScreen)
            {
                //Tools.GameClass.TargetElapsedTime = TargetElapsedTime_58fps;
                //Tools.GameClass.IsFixedTimeStep = true;

                Tools.GameClass.TargetElapsedTime = TargetElapsedTime_60fps;
                Tools.GameClass.IsFixedTimeStep = false;
            }
            else
            {
                Tools.GameClass.TargetElapsedTime = TargetElapsedTime_60fps;
                Tools.GameClass.IsFixedTimeStep = false;
            }
        }

        public static bool ShowMarketplace = false;

        static bool ShowErrorMessage;

        public static void ShowError_LoadError()
        {
#if PC
#else
            ShowError(Localization.Words.Err_CorruptLoadHeader, Localization.Words.Err_CorruptLoad, Localization.Words.Err_Ok, null);
#endif
        }

        public static void ShowError_MustBeSignedIn(Localization.Words word)
        {
#if PC
#else
            ShowError(Localization.Words.Err_MustBeSignedIn_Header, word, Localization.Words.Err_Ok, null);
#endif
        }

        public static void ShowError_MustBeSignedInToLive(Localization.Words word)
        {
#if PC
#else
            ShowError(Localization.Words.Err_MustBeSignedInToLive_Header, word, Localization.Words.Err_Ok, null);
#endif
        }

        public static void ShowError_MustBeSignedInToLiveForLeaderboard()
        {
#if PC
#else
            if (CloudberryKingdomGame.IsDemo) return;
            
            ShowError(Localization.Words.Err_MustBeSignedInToLive_Header, Localization.Words.Err_MustBeSignedInToLiveForLeaderboards, Localization.Words.Err_Ok, null);
#endif
        }

        public static bool IsNetworkCableUnplugged()
        {
            return true;
        }

        static void ShowError(Localization.Words Header, Localization.Words Text, Localization.Words Option1, AsyncCallback callback)
        {
            ShowErrorMessage = true;

            Err_Header = Header;
            Err_Text = Text;
            Err_Callback = callback;
            Err_Options = new string[] { Localization.WordString(Option1) };
        }

        static Localization.Words Err_Header, Err_Text;
        static string[] Err_Options;
        static AsyncCallback Err_Callback;

        static void _ShowError()
        {
            ShowErrorMessage = false;
        }

        bool DisconnectedController()
        {
#if PC || DEBUG && WINDOWS
            if (ButtonCheck.MouseInUse || !ButtonCheck.ControllerInUse) return false;
#endif
            // True if an existing player is disconnected.
            for (int i = 0; i < 4; i++)
            {
                if (PlayerManager.Players[i] != null && PlayerManager.Players[i].Exists && !CoreGamepad.IsConnected(i))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ForceSuperPause = false;
        public static bool SuperPause
        {
            get
            {
                return SmallErrorMessage != null || ForceSuperPause;
            }
        }
        static SmallErrorMenu SmallErrorMessage;
        static void ShowSmallError()
        {
            if (SmallErrorMessage != null)
            {
                if (SmallErrorMessage.Core.Released)
                {
                    SmallErrorMessage = new SmallErrorMenu(Localization.Words.Err_ControllerNotConnected);
                }

                if (SmallErrorMessage.MyGame == null && Tools.CurGameData != null)
                {
                    Tools.CurGameData.AddGameObject(SmallErrorMessage);
                }

                return;
            }
            if (Tools.CurGameData == null) return;

            SmallErrorMessage = new SmallErrorMenu(Localization.Words.Err_ControllerNotConnected);
            Tools.CurGameData.AddGameObject(SmallErrorMessage);
        }

        public static bool CustomMusicPlaying = false;
        void UpdateCustomMusic()
        {
        }

        /// <summary>
        /// If a gamer has no save device selected, ask them to select one.
        /// </summary>
        public static void PromptForDeviceIfNoneSelected()
        {
            return;
        }

        void DrawWatermark()
        {
            return;

            if (FinalRelease) return;
            if (Tools.QDrawer == null) return;
            if (Resources.Font_Grobold42 == null) return;
            if (Resources.Font_Grobold42.HFont == null) return;
            if (Resources.Font_Grobold42.HOutlineFont == null) return;
            if (Tools.CurCamera == null) return;

            Camera cam = new Camera();
            cam.SetVertexCamera();
            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, "Version 0.9.9", new Vector2(1200, 870), Color.Black.ToVector4(),   new Vector2(.8f));
            Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, "Version 0.9.9", new Vector2(1200, 870), Color.SkyBlue.ToVector4(), new Vector2(.8f));
            Tools.QDrawer.Flush();
        }

        static bool LastGuideIsUp = false;
        static int GuidSave_SeedMark = 0;

#if PC
        bool created = false;
#endif

        static double DelayAmount = 0;

        /// <summary>
        /// The main draw loop.
        /// Sets all the rendering up and determines which sub-function to call (game, loading screen, nothing, etc).
        /// Also updates the game logic. TODO: Seperate this from the draw function?
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
        //if (Resources.LoadThread != null)
        //Resources.LoadThread.Join ();

#if PC
            if (CloudberryKingdomGame.SteamAvailable)
            {
                SteamCore.Update();
            }
#endif

#if DEBUG_OBJDATA
            ObjectData.UpdateWeak();
#endif
            DeltaT = gameTime.ElapsedGameTime.TotalSeconds;

            const double DelayIncr = 1.0 / 1000.0f;

            int TargetFps = 60;
            double TargetDelay = 1.0 / TargetFps;
            
            if (DeltaT < TargetDelay - 2 * DelayIncr)
            {
                DelayAmount += DelayIncr;
            }
            else
            {
                if (DeltaT > TargetDelay + 1 * DelayIncr)
                {
                    DelayAmount -= DelayIncr;
                }
            }

            if (DelayAmount > 1 * DelayIncr)
            {
                Thread.Sleep((int)(1000 * DelayAmount));
                //return;
            }

            // Stop now if the initial preload (and logosalad) hasn't started yet.
            if (!PreloadDone) { SetupToRender(); return; }

            // Prepare to draw
            Tools.DrawCount++;
            if (SetupToRender()) { DrawWatermark(); return; }

            // Main Video
            if (XnaVideo.Draw())
            {
                DrawWatermark();
                return;
            }

            // Do not proceed if players do not exist yet.
            if (PlayerManager.Players == null) return;

            // Fps
            UpdateFps(gameTime);

            // If the full game was just unlocked, return to the title screen
            if (DoTrialUnlockEvent)
            {
                DoTrialUnlockEvent = false;
            }

            // Draw nothing if Steam input overlay is up
#if PC
            if (SteamTextInput.OverlayActive) return;
#endif

            LastGuideIsUp = false;

            //CheckForSignInState();
            UpdateCustomMusic();

            // What to do
            if (LogoScreenUp)
            {
                if (LoadingScreen == null) return;
                LogoPhsx();
            }
            else if (LogoScreenPropUp)
            {
                LoadingScreen.PhsxStep();
            }
            if (!LogoScreenUp && !Tools.CurGameData.Loading)
                GameUpdate(gameTime);

            // What to draw
            if (LogoScreenUp || LogoScreenPropUp)
                LoadingScreen.Draw();
            else if (Tools.ShowLoadingScreen)
                DrawLoading();
            else if (Tools.CurGameData != null && !XnaVideo.IsPlaying)
                DrawGame();
            else
                DrawNothing();

            DrawExtra();

            DrawWatermark();
            DrawSavingText();
        }

        /// <summary>
        /// Non-game drawing, such as debug info and tool drawing.
        /// </summary>
        private void DrawExtra()
        {
#if DEBUG
            if (ShowFPS || Tools.ShowNums)
                DrawDebugInfo();
#endif

#if DEBUG && INCLUDE_EDITOR
            if (Tools.background_viewer != null)
                Tools.background_viewer.Draw();
#endif
            Tools.Nothing();
        }

        /// <summary>
        /// Draws the load screen, assuming the game should not be drawn this frame.
        /// </summary>
        private void DrawLoading()
        {
            Tools.CurrentLoadingScreen.PreDraw();
            Tools.CurrentLoadingScreen.Draw(MainCamera);
        }

        /// <summary>
        /// Draws nothing (black). Called when the game shouldn't be shown, nor anything else, such as load screens.
        /// </summary>
        private void DrawNothing()
        {
            MyGraphicsDevice.Clear(Color.Black);
        }

        /// <summary>
        /// Draws the actual the game, not any loading screens or other non-game graphics.
        /// </summary>
        private void DrawGame()
        {
            Tools.CurGameData.Draw();
            Tools.CurGameData.PostDraw();

            if (Tools.CurLevel != null)
            {
                Tools.QDrawer.Flush();
                Tools.StartGUIDraw();

#if PC
                if (!Tools.ShowLoadingScreen && ShowMouse)
                    MouseDraw();
                ShowMouse = false;
#endif

                if (Tools.SongWad != null)
                    Tools.SongWad.Draw();

                Tools.EndGUIDraw();
                Tools.QDrawer.Flush();
            }
        }

        /// <summary>
        /// The update function called for the actual game, not for loading screens or other non-game functions.
        /// </summary>
        /// <param name="gameTime"></param>
        private void GameUpdate(GameTime gameTime)
        {
#if WINDOWS && !MONO && !SDL2
            // Do nothing if editors are open.
            if (Tools.Dlg != null || Tools.DialogUp) return;
#endif

            // Update controller/keyboard states
            if (WindowInFocus)
            {
                ButtonCheck.UpdateControllerAndKeyboard_StartOfStep();
            }
            else
            {
                Tools.Keyboard = new KeyboardState();
                Tools.PrevKeyboard = new KeyboardState();

                CoreGamepad.Clear();

                Tools.KillVibrations();
            }

            // Update sounds
            if (!LogoScreenUp)
                Tools.SoundWad.Update();

            // Update songs
            if (Tools.SongWad != null)
                Tools.SongWad.PhsxStep();

            fps = .3f * fps + .7f * (1000f / (float)Math.Max(.00000231f, gameTime.ElapsedGameTime.TotalMilliseconds));

            // Determine how many phsx steps to take
            int Reps = 0;
            if (Tools.PhsxSpeed == 0 && DrawCount % 2 == 0) Reps = 1;
            else if (Tools.PhsxSpeed == 1) Reps = 1;
            else if (Tools.PhsxSpeed == 2) Reps = 2;
            else if (Tools.PhsxSpeed == 3) Reps = 4;

            // Do the phsx
            for (int i = 0; i < Reps; i++)
            {
                PhsxCount++;
                PhsxStep();
            }
        }

        private void UpdateFps(GameTime gameTime)
        {
        //if (!MediaPlayer.Instance.GameHasControl)
        //{
        //CustomMusicPlaying = true;
        //}

            // Track time, changes in time, and FPS
            Tools.gameTime = gameTime;
            DrawCount++;

            // Update fps
            float new_t = (float)gameTime.TotalGameTime.TotalSeconds;
            Tools.dt = new_t - Tools.t;
            Tools.t = new_t;
        }

        public static bool WindowInFocus = true;

        /// <summary>
        /// Sets up the renderer. Returns true if no additional drawing should be done, because the game does not have focus.
        /// </summary>
        /// <returns></returns>
        private bool SetupToRender()
        {
            // Set the viewport to the whole screen
            MyGraphicsDevice.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = MyGraphicsDevice.PresentationParameters.BackBufferWidth,
                Height = MyGraphicsDevice.PresentationParameters.BackBufferHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            // Clear whole screen to black
            MyGraphicsDevice.Clear(Color.Black);

#if WINDOWS
            if (OnlyDrawGameWhenInFocus)
            {
                if (!ActiveInactive())
                    return true;
            }
            else
            {
                if (ActiveInactive())
                    WindowInFocus = true;
                else
                    WindowInFocus = false;
            }
#endif

            // Make the actual view port we draw to, and clear it
            Tools.Render.MakeInnerViewport();
            MyGraphicsDevice.Clear(Color.Black);

            MyGraphicsDevice.Viewport = Tools.Render.MainViewport;

            // Default camera
            Vector4 cameraPos = new Vector4(MainCamera.Data.Position.X, MainCamera.Data.Position.Y, MainCamera.Zoom.X, MainCamera.Zoom.Y);//.001f, .001f);

            Tools.Render.SetStandardRenderStates();

            Tools.QDrawer.SetInitialState();
            ComputeFire();

            Tools.EffectWad.SetCameraPosition(cameraPos);

            Tools.SetDefaultEffectParams(MainCamera.AspectRatio);

            Tools.Render.SetStandardRenderStates();

            return false;
        }

        BlendState AdditiveColor_NormalAlpha;
        /// <summary>
        /// Draw the fireball textures to memory.
        /// </summary>
        private void ComputeFire()
        {
            if (!LogoScreenUp && !LogoScreenPropUp)
            {
                if (!Tools.CurGameData.Loading && Tools.CurLevel.PlayMode == 0 && Tools.CurGameData != null && !Tools.CurGameData.Loading && (!Tools.CurGameData.PauseGame || CharacterSelectManager.IsShowing))
                {
                    // Compute fireballs textures
                    MyGraphicsDevice.BlendState = AdditiveColor_NormalAlpha;

                    Fireball.DrawFireballTexture(MyGraphicsDevice, Tools.EffectWad);
                    Fireball.DrawEmitterTexture(MyGraphicsDevice, Tools.EffectWad);
                    
                    MyGraphicsDevice.BlendState = BlendState.AlphaBlend;
                }
            }
        }

#if PC
        public bool IsActive()
        {
            bool IsActive = true;

#if WINDOWS && !MONO && !SDL2
            // XNA on Windows does not correctly identify that the game window doesn't have focus
            // in the case where the game window STARTS in the background.
            // This is an additional check to see if the window is not in focus.
            IntPtr CkHandle = Tools.GameClass.Window.Handle;
            IntPtr ActiveHandle = WindowsHelper.GetForegroundWindow();

            if (CkHandle != ActiveHandle)
            {
#if DEBUG
                if (!Tools.ViewerIsUp)
#endif
                    IsActive = false;
            }

            if (!Tools.GameClass.IsActive)
                IsActive = false;
#else
            if (!Tools.GameClass.IsActive)
                IsActive = false;
#endif

            return IsActive;
        }

        public const bool OnlyDrawGameWhenInFocus = false;

        /// <summary>
        /// Decide if the game should be active or not.
        /// </summary>
        /// <returns>Returns true if the game is active.</returns>
        private bool ActiveInactive()
        {
            //if (!Tools.GameClass.IsActive || !IsActive)
            if (!IsActive())
            {
                // The window isn't active, so
                // show the actual mouse (not our custom drawn mouse)
                Tools.GameClass.IsMouseVisible = true;
                Tools.GameClass.FakeTab();

                if (OnlyDrawGameWhenInFocus)
                {
                    // If a song is playing, stop it,
                    // and note that we should resume once the window becomes active
                    if (Tools.SongWad != null && Tools.SongWad.IsPlaying())
                    {
                        if (!MediaPlaying_HoldState)
                        {
                            MediaPlaying_HoldState = true;
                            
                            try
                            {
                                MediaPlayer.Instance.Pause();
                            }
                            catch
                            {
                            }
                        }
                    }

                    // If a movie is playing, pause it,
                    // and note that we should resume once the window becomes active.
                    if (XnaVideo.IsPlaying)
                    {
                        if (!VideoPlaying_HoldState)
                        {
                            VideoPlaying_HoldState = true;
                            XnaVideo.Pause();
                        }
                    }

                    FirstInactiveFrame = false;
                }

                FirstActiveFrame = true;

                return false;
            }
            else
            {
                // The window is active, so
                // hide the actual mouse (we draw our own custom mouse in game)
                Tools.GameClass.IsMouseVisible = false;
                Tools.GameClass.FakeFull();

                if (FirstActiveFrame)
                {
                    // If a song was playing previously when the window was active before,
                    // resume that song
                    if (MediaPlaying_HoldState)
                    {
                        MediaPlaying_HoldState = false;
                        
                        try
                        {
                            MediaPlayer.Instance.Resume();
                        }
                        catch
                        {
                        }
                    }

                    // If a video was playing previously when the window was active before,
                    // unpause the video.
                    if (VideoPlaying_HoldState)
                    {
                        VideoPlaying_HoldState = false;
                        XnaVideo.Resume();
                    }

                    FirstActiveFrame = false;
                }

                FirstInactiveFrame = true;

                // If we are editing the background show the mouse
                if (Tools.ViewerIsUp)
                    Tools.GameClass.IsMouseVisible = true;

                return true;
            }
        }
#endif
    }
}
