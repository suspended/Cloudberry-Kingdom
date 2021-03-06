using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public class CkBaseMenu : GUI_Panel
    {
        public FancyVector2 zoom;
        public float MasterAlpha;
        public bool UseBounce;
		public bool UseSimpleBackdrop;

        public void EnableBounce()
        {
            MasterAlpha = 1;

            zoom = new FancyVector2();
            UseBounce = true;

			UseSimpleBackdrop = UseBounce && !(Tools.CurGameData is NormalGameData);
        }

        void BouncDraw()
        {
            if (zoom != null)
            {
                Vector2 v = zoom.Update();
                MasterAlpha = v.X * v.X;

                MyGame.Cam.Zoom = .001f * v;
                MyGame.Cam.SetVertexCamera();
                Text.ZoomWithCamera_Override = true;
            }
            else
            {
                MasterAlpha = 1f;
            }

			if (MyPile != null) MyPile.Alpha = MasterAlpha;
        }

        protected override void MyDraw()
        {
            if (UseBounce)
                BouncDraw();

            base.MyDraw();

			if (UseBounce)
			{
				MyGame.Cam.Zoom = new Vector2(.001f);
				MyGame.Cam.SetVertexCamera();
				Text.ZoomWithCamera_Override = false;
			}
        }


		protected void EpilepsySafe(float SafetyLevel)
		{
			float gray_color = CoreMath.Lerp(1f,  .8735f, SafetyLevel);
			float dark_color = CoreMath.Lerp(.1f, .1f,	  SafetyLevel);

			QuadClass _q;
			_q = MyPile.FindQuad("Backdrop");	if (_q != null) { _q.Alpha = .7f; _q.Quad.SetColor(ColorHelper.GrayColor(gray_color)); }
			_q = DarkBack;						if (_q != null) { _q.Quad.SetColor(ColorHelper.GrayColor(dark_color)); }
		}

        protected QuadClass DarkBack;
        protected void MakeDarkBack()
        {
            // Make the dark back
            DarkBack = new QuadClass("White");
            DarkBack.Quad.SetColor(ColorHelper.GrayColor(.2f));
            DarkBack.Alpha = 0f;
            DarkBack.Fade(.135f); DarkBack.MaxAlpha = .7125f;
            DarkBack.FullScreen(Tools.CurCamera);
            DarkBack.Pos = Vector2.Zero;
            DarkBack.Scale(5);
            MyPile.Add(DarkBack, "Dark");
        }

        protected CoreSound SelectSound, BackSound;

        public Vector2 ItemPos = new Vector2(-808, 110);
        protected Vector2 PosAdd = new Vector2(0, -151) * 1.181f;

        protected CoreFont ItemFont = Resources.Font_Grobold42;
        protected float FontScale = .75f;

        protected virtual void SetItemProperties(MenuItem item)
        {
            if (item.MyText == null) return;

            SetTextProperties(item.MyText);
            SetSelectedTextProperties(item.MySelectedText);
        }
        
        protected virtual void SetHeaderProperties(Text text)
        {
            text.MyFloatColor = new Vector4(.6f, .6f, .6f, 1f);
            text.OutlineColor = new Vector4(0f, 0f, 0f, 1f);

            text.Shadow = true;
            text.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            text.ShadowOffset = new Vector2(12, 12);

            text.Scale = FontScale * .9f;
        }

        /// <summary>
        /// Whether menu items added to the menu are given shadows
        /// </summary>
        protected bool ItemShadows = true;

        protected virtual void SetTextProperties(Text text)
        {
            text.MyFloatColor = new Color(184, 231, 231).ToVector4();

            text.Scale = FontScale;

            text.Shadow = ItemShadows;
            text.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            text.ShadowOffset = new Vector2(12, 12);
        }

        protected virtual void SetSelectedTextProperties(Text text)
        {
			//text.MyFloatColor = new Color(246, 214, 33).ToVector4();
			text.MyFloatColor = new Color(246, 214, 33).ToVector4() * .9735f;
			text.MyFloatColor.W = 1.0f;

            text.Scale = FontScale;

            text.Shadow = ItemShadows;
            text.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            text.ShadowOffset = new Vector2(12, 12);
        }

        /// <summary>
        /// Amount a menu item is shifted when selected.
        /// </summary>
        //protected Vector2 SelectedItemShift = new Vector2(-65, 0);
        protected Vector2 SelectedItemShift = new Vector2(0, 0);

        protected virtual void AddItem(MenuItem item)
        {
            SetItemProperties(item);

            item.Pos = ItemPos;
            item.SelectedPos = ItemPos + SelectedItemShift;

            ItemPos += PosAdd;

            MyMenu.Add(item);
        }

        public override int SlideLength
        {
            set
            {
                base.SlideLength = value;
                if (TopPanel != null) TopPanel.SlideLength = value;
                if (RightPanel != null) RightPanel.SlideLength = value;
            }
        }

        public override void Init()
        {
			UseSimpleBackdrop = UseBounce && !(Tools.CurGameData is NormalGameData);

            base.Init();

            // Sounds
            SelectSound = Menu.DefaultMenuInfo.Menu_Select_Sound;
            BackSound = Menu.DefaultMenuInfo.Menu_Back_Sound;

            // Delays
            Defaults();
        }

        public void DefaultDelays()
        {
            ReturnToCallerDelay = 40;
            CallDelay = 25;
        }

        public void DefaultSlides()
        {
            SlideLength = 38;
        }

        public void Defaults()
        {
            DefaultDelays();
            DefaultSlides();
        }

        public void NoDelays()
        {
            SlideLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;
        }

        public void FastDelays()
        {
            CallDelay = 18;
            ReturnToCallerDelay = 26;
        }

        public void FastSlides()
        {
            SlideLength = 33;
        }

        public void Fast()
        {
            FastDelays();
            FastSlides();
        }

        public void CategoryDelays()
        {
            ReturnToCallerDelay = 16;
            SlideInLength = 25;
            SlideOutLength = 24;

            CallDelay = 18;
        }

        /// <summary>
        /// When true this panel follows the following convention:
        /// When it calls a subpanel, this panel slides out to the left.
        /// When that subpanel returns control to this panel, this panel slides in from the left.
        /// </summary>
        protected bool CallToLeft = false;
        public override void Call(GUI_Panel child, int Delay)
        {
            base.Call(child, Delay);

            if (SelectSound != null && !SkipCallSound)
                SelectSound.Play();
            SkipCallSound = false;

            if (CallToLeft)
            {
                Hide(PresetPos.Left);
            }
        }

        public bool SkipCallSound = false;

        public override void OnReturnTo()
        {
            if (UseBounce)
            {
                base.OnReturnTo();
                RegularSlideOut(PresetPos.Right, 0);
                BubbleUp();
            }
            else
            {
                if (CallToLeft)
                {
                    // Reset the menu's selected item's oscillate
                    if (MyMenu != null) MyMenu.CurItem.OnSelect();

                    // Activate and show the panel
                    Active = true;

                    if (!Hid) return;
                    base.Show();
                    this.SlideOut(PresetPos.Left, 0);
                    this.SlideIn();
                }
                else
                    base.OnReturnTo();
            }
        }

        public override void ReturnToCaller() { ReturnToCaller(true); }
        public virtual void ReturnToCaller(bool PlaySound)
        {
            base.ReturnToCaller();

            if (DarkBack != null)
                DarkBack.Fade(-.1f);

            if (PlaySound && BackSound != null)
                BackSound.Play();
        }

        GUI_Panel _RightPanel, _TopPanel;
        protected GUI_Panel RightPanel
        {
            set { _RightPanel = value; _RightPanel.CopySlideLengths(this); }
            get { return _RightPanel; }
        }
        protected GUI_Panel TopPanel
        {
            set { _TopPanel = value; _TopPanel.CopySlideLengths(this); }
            get { return _TopPanel; }
        }

        public override void OnAdd()
        {
            base.OnAdd();

            if (TopPanel != null) MyGame.AddGameObject(TopPanel);
            if (RightPanel != null) MyGame.AddGameObject(RightPanel);

            //SlideLength = 38;

            Show();

            BubbleUp();
        }

        protected void BubbleUp()
        {
            if (zoom != null)
            {
                SlideIn(0);
                zoom.MultiLerp(5, new Vector2[] { new Vector2(0.98f), new Vector2(1.02f), new Vector2(.99f), new Vector2(1.005f), new Vector2(1f) });
            }
        }

        protected void BubbleDown()
        {
            BubblingOut = true;
            zoom.MultiLerp(5, new Vector2[] { new Vector2(1.0f), new Vector2(1.01f), new Vector2(.9f), new Vector2(.4f), new Vector2(0f) });
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            if (TopPanel != null) TopPanel.Release();
            if (RightPanel != null) RightPanel.Release();

			if (zoom != null) zoom.Release(); zoom = null;
			if (DarkBack != null) DarkBack.Release(); DarkBack = null;

			SelectSound = null; BackSound = null;
			ItemFont = null;
			_RightPanel = null; _TopPanel = null;
			MenuTemplate = null;
        }

        public PresetPos SlideInFrom = PresetPos.Left;
        public override void Show()
        {
            if (!Hid) return;

 	        base.Show();

            this.SlideOut(SlideInFrom, 0);

            this.SlideIn();
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);

            if (RightPanel != null)
                SlideIn_RightPanel(Frames);

            if (TopPanel != null)
            {
                TopPanel.SlideOut(PresetPos.Right, 0);
                TopPanel.SlideIn();
            }
        }

        protected virtual void SlideIn_RightPanel(int Frames)
        {
            RightPanel.SlideOut(PresetPos.Right, 0);
            RightPanel.SlideIn();
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            if (UseBounce)
                BounceSlideOut(Preset, Frames);
            else
                RegularSlideOut(Preset, Frames);
        }

        void BounceSlideOut(PresetPos Preset, int Frames)
        {
            ReturnToCallerDelay = 15;

            if (Frames == 0)
            {
                RegularSlideOut(Preset, Frames);
                return;
            }

            BubbleDown();
            if (MyGame != null) MyGame.WaitThenDo(15, Release);

            Active = true;

            ReleaseWhenDone = false;
            ReleaseWhenDoneScaling = false;
        }

        public void RegularSlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);

            if (RightPanel != null) SlideOut_RightPanel(Preset, Frames);
            if (TopPanel != null) TopPanel.SlideOut(PresetPos.Right, Frames);
        }

        protected virtual void SlideOut_RightPanel(GUI_Panel.PresetPos Preset, int Frames)
        {
            RightPanel.SlideOut(PresetPos.Right, Frames);
        }

        protected PresetPos SlideOutTo = PresetPos.Left;
        public override void Hide() { Hide(SlideOutTo); }
        public virtual void Hide(PresetPos pos) { Hide(pos, -1); }
        public virtual void Hide(PresetPos pos, int frames)
        {
            base.Hide();

            if (frames == -1)
                this.SlideOut(pos);
            else
                this.SlideOut(pos, frames);
        }

        protected void MakeStaticBackButton()
        {
            MyPile.Add(new QuadClass(ButtonTexture.Back, 90, "Button_Back"));
            MyPile.Add(new QuadClass("BackArrow2", "BackArrow"));
        }

        protected MenuItem MakeBackButton() { return MakeBackButton(Localization.Words.Back, true); }
        protected MenuItem MakeBackButton(Localization.Words Word, bool AddButtonTexture)
        {
            MenuItem item;

			if (ButtonCheck.ControllerInUse && AddButtonTexture)
			{
				item = new MenuItem(new Text(ButtonString.Back(86) + " " + Localization.WordString(Word)));
			}
			else
			{
				item = new MenuItem(new Text(Localization.WordString(Word), ItemFont));
			}

            item.Go = _MakeBackGo;
            item.Name = "Back";
            AddItem(item);
            item.SelectSound = null;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;

            return item;
        }

        void _MakeBackGo(MenuItem item)
        {
            item.MyMenu.OnB(item.MyMenu);
        }

        public static void MakeBackdrop(Menu menu, Vector2 TR, Vector2 BL)
        {
            menu.MyPieceQuad = new PieceQuad();
            menu.MyPieceQuadTemplate = MenuTemplate;
            menu.TR = TR;
            menu.BL = BL;
            menu.ResetPieces();

            SetBackdropProperties(menu.MyPieceQuad);
        }

        public static PieceQuad MenuTemplate = null;
        protected void MakeBackdrop(Vector2 TR, Vector2 BL)
        {
            MakeBackdrop(MyMenu, TR, BL);
        }

        public static void SetBackdropProperties(PieceQuad piecequad)
        {
            piecequad.SetAlpha(.7f);
        }

        //public static int DefaultMenuLayer = Levels.Level.LastInLevelDrawLayer + 1;
        public static int DefaultMenuLayer = Levels.Level.LastInLevelDrawLayer;

        public CkBaseMenu() { Core.DrawLayer = DefaultMenuLayer; }
        public CkBaseMenu(bool CallBaseConstructor) : base(CallBaseConstructor) { Core.DrawLayer = DefaultMenuLayer; }

        public override void Draw()
        {
            base.Draw();

            if (DarkBack != null && !IsOnScreen)
                DarkBack.Draw();
        }
    }
}