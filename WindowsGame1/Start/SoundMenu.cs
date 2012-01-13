using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class SoundMenu : VerifyBaseMenu
    {
        bool Centered = false;
        public SoundMenu(int Control, bool Centered) : base(false)
        {
            this.Control = Control;
            this.Centered = Centered;
            FixedToCamera = true;

            Constructor();
        }

        EzText HeaderText;
        public override void Init()
        {
            base.Init();

            // Header
            string Text = "Options";
            HeaderText = new EzText(Text, ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;
#if PC_VERSION
            HeaderText.Pos = new Vector2(-967.064f, 951.6506f);
#endif

            ItemPos = new Vector2(555.5547f, 402.6666f);
            PosAdd.Y *= .97f;

            MenuSlider FxSlider = new MenuSlider(new EzText("fx:", ItemFont));
            FxSlider.MyFloat = Tools.SoundVolume;
            AddItem(FxSlider);
#if PC_VERSION
            FxSlider.SetPos = new Vector2(603.174f, 759.8094f);
#endif

            MenuSlider MusicSlider = new MenuSlider(new EzText("Music:", ItemFont));
            MusicSlider.MyFloat = Tools.MusicVolume;
            AddItem(MusicSlider);
            MusicSlider.Pos.X = MusicSlider.SelectedPos.X = MusicSlider.Pos.X - 300;
#if PC_VERSION
            MusicSlider.SetPos = new Vector2(358.7296f, 548.1749f);
#endif
            MusicSlider.SliderShift.X += 300;

#if PC_VERSION
            if (Centered)
            {
                MenuItem item = new MenuItem(new EzText("Controls", ItemFont));
                item.Go = _item =>
                    {
                        Hide();
                        Call(new ControlScreen(Control), 10);
                    };
                AddItem(item);
                item.SetPos = new Vector2(619.0466f, 273.0472f);
            }

#if PC_VERSION
            // Custom controls
            var mitem = new MenuItem(new EzText("Edit controls", ItemFont));
            mitem.Go = menuitem => Call(new CustomControlsMenu(), 10);
            AddItem(mitem);
            mitem.SetPos = new Vector2(611.1101f, 61.41245f);



            // Full screen resolutions
            var RezText = new EzText("Resolution:", ItemFont);
            SetHeaderProperties(RezText);
            MyPile.Add(RezText);
            RezText.Pos = new Vector2(-1221.032f, -238.8261f);
            RezText.Scale *= .9f;

            MenuList FsRezList = new MenuList();
            FsRezList.Center = false;
            FsRezList.MyExpandPos = new Vector2(-498.1506f, 713.873f);
            int i = 0;
            int CurRez = 0;

            // Get viable resolutions
            List<DisplayMode> modes = new List<DisplayMode>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (modes.Any(existing => existing.Width == mode.Width && existing.Height == mode.Height))
                    continue;
                else
                    modes.Add(mode);
            }

            // Add resolutions to the current list
            bool found = false;
            foreach (var mode in modes)
            {
                string str = mode.Width + " x " + mode.Height;
                Tools.Write(str);
                var item = new MenuItem(new EzText(str, ItemFont, false, true));
                SetItemProperties(item);
                FsRezList.AddItem(item, mode);

                if (mode.Width == Tools.TheGame.graphics.PreferredBackBufferWidth &&
                    mode.Height == Tools.TheGame.graphics.PreferredBackBufferHeight)
                {
                    CurRez = i;
                    found = true;
                }
                else if (!found && mode.Width == Tools.TheGame.graphics.PreferredBackBufferWidth)
                    CurRez = i;

                i++;
            }
            AddItem(FsRezList);
            FsRezList.SetPos = new Vector2(1085.714f, -353.7465f);
            FsRezList.SetIndex(CurRez);
            FsRezList.OnConfirmedIndexSelect = () =>
            {
                PlayerManager.SavePlayerData.ResolutionPreferenceSet = true;
                ResolutionGroup.Use(FsRezList.CurObj as DisplayMode);
                SaveGroup.SaveAll();
                PlayerManager.SaveRezAndKeys();
            };





            // Full screen toggle
            var FullScreenText = new EzText("Full screen:", ItemFont);
            SetHeaderProperties(FullScreenText);
            MyPile.Add(FullScreenText);
            FullScreenText.Pos = new Vector2(-1165.475f, -461.0471f);
            FullScreenText.Scale *= .9f;

            var toggle = new MenuToggle(ItemFont);
            toggle.OnToggle = (state) =>
            {
                PlayerManager.SavePlayerData.ResolutionPreferenceSet = true;
                Tools.Fullscreen = state;
                SaveGroup.SaveAll();
                PlayerManager.SaveRezAndKeys();
            };
            toggle.Toggle(Tools.Fullscreen);
            toggle.PrefixText = ""; // "Fullscreen: ";
            AddItem(toggle);
            toggle.SetPos = new Vector2(1253.967f, -404.1903f);
#endif


            ////// Resolution
            ////MenuList ResolutionList = new MenuList();
            ////ResolutionList.Center = false;
            ////ResolutionList.MyExpandPos = new Vector2(-498.1506f, 713.873f);
            ////foreach (ResolutionGroup rez in Tools.TheGame.Resolutions)
            ////{
            ////    var item = new MenuItem(new EzText(rez.ToString(), ItemFont, false, true));
            ////    SetItemProperties(item);
            ////    ResolutionList.AddItem(item, rez);
            ////}
            ////AddItem(ResolutionList);
            ////ResolutionList.Pos = new Vector2(0, 828f);
            ////ResolutionList.OnConfirmedIndexSelect = () =>
            ////{
            ////    ResolutionGroup rez = Tools.TheGame.Resolutions[ResolutionList.ListIndex];
            ////    rez.Use();
            ////};
            ////ResolutionList.SetIndex(0);
#endif
            SetPosition();

#if PC_VERSION
            MakeBackButton().SetPos = new Vector2(1603.173f, -621.111f);
#else
            MakeBackButton();
#endif
            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }

        private void SetPosition()
        {
            if (Centered)
            {
                MyPile.Pos = new Vector2(29.76172f, 21.82541f);
                MyMenu.FancyPos.RelVal = new Vector2(-1007.934f, -43.651f);

#if PC_VERSION
                Backdrop.Size = new Vector2(1376.984f, 1077.035f);
                Backdrop.Pos = new Vector2(-18.6521f, -10.31725f);
#else
                HeaderText.Pos = new Vector2(-506.746f, 729.4285f);

                Backdrop.Size = new Vector2(1380.952f, 1029.416f);
                Backdrop.Pos = new Vector2(18.55249f, -8.333206f);
#endif
            }
            else
            {
                HeaderText.Pos = new Vector2(416.6666f, 824.6663f);
                MyMenu.FancyPos.RelVal = new Vector2(-1125.001f, -241.6667f);
            }
        }

        public override void OnAdd()
        {
 	        base.OnAdd();

            SetPosition();
        }
    }
}