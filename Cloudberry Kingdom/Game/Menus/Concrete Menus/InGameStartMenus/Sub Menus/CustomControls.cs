using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CloudberryKingdom
{
#if PC_VERSION
    public class ControlItem : MenuItem
    {
        public QuadClass MyQuad;
        public Keys MyKey;
        public Action<Keys> SetSecondaryKey;
        public Action<ControlItem> Reset;

        public ControlItem(string description, Keys key)
            : base(new EzText(description, Resources.Font_Grobold42, 2000, false, false, .65f))
        {
            MyKey = key;
            MyQuad = new QuadClass("White", 72);
            //MyQuad.Quad.SetColor(CustomControlsMenu.SecondaryKeyColor);
            MyQuad.Quad.SetColor(new Color(240,240,240));
            SetKey(MyKey);
        }

        public void SetKey(Keys key)
        {
            MyKey = key;
            //SetSecondaryKey(key);
            SetQuad();
        }

        public void SetQuad()
        {
            MyQuad.TextureName = ButtonString.KeyToTexture(MyKey);
            MyQuad.ScaleYToMatchRatio();
        }
    }

    public class CustomControlsMenu : CkBaseMenu
    {
        public static Color SecondaryKeyColor = Color.SkyBlue;
        protected override void ReleaseBody()
        {
            base.ReleaseBody();
        }

        void Save()
        {
            // Before we exit make sure secondary keys match up to what the user just specified.
            foreach (MenuItem item in MyMenu.Items)
            {
                ControlItem citem = item as ControlItem;
                if (null != citem)
                    citem.SetSecondaryKey(citem.MyKey);
            }

            PlayerManager.SaveRezAndKeys();
        }

        protected override void SetTextProperties(EzText text)
        {
            base.SetTextProperties(text);

            //text.Shadow = false;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = false;
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            // Add the associated quad
            ControlItem citem = item as ControlItem;
            if (null == citem) return;

            MyPile.Add(citem.MyQuad);
            citem.MyQuad.Pos = item.Pos + new Vector2(-150, -132);
        }

        public CustomControlsMenu() { }

        protected QuadClass Backdrop;
        public virtual void MakeBackdrop()
        {
            Backdrop = new QuadClass("Backplate_1230x740", 1500, true);
            MyPile.Add(Backdrop);
            Backdrop.Size = new Vector2(1376.984f, 1077.035f);
            Backdrop.Pos = new Vector2(-18.6521f, -10.31725f);
        }

        public void MakeInstructions()
        {
            var backdrop = new QuadClass("score_screen", 1500, true);
            MyPile.Add(backdrop);
            MyPile.Add(backdrop);
            backdrop.Size = new Vector2(603.1736f, 429.5635f);
            backdrop.Pos = new Vector2(1157.441f, 51.19061f);
            backdrop.Quad.SetColor(new Color(215, 215, 215, 255));

            var text = new EzText("Press any key to switch control of selected item.", Resources.Font_Grobold42, 700, true, true, .675f);
            text.Scale *= .68f;
            text.Pos = new Vector2(1158.73f, 79.36493f);
            MyPile.Add(text);
        }

        void Reset(MenuItem _item)
        {
            ButtonCheck.Reset();
            foreach (var item in MyMenu.Items)
            {
                ControlItem c = item as ControlItem;
                if (null == c) continue;

                c.Reset(c);
            }
        }

        void MakeBack()
        {
            MenuItem item;

            // Customize
            item = new MenuItem(new EzText("Reset", ItemFont));
            item.Go = Reset;
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();

            ItemPos = new Vector2(698.9696f, 892.0638f);
            item.UnaffectedByScroll = true;
            AddItem(item);
            item.ScaleText(.5f);

            // Back
            ItemPos = new Vector2(810.0829f, 752.6987f);
            item = MakeBackButton();
            item.UnaffectedByScroll = true;
            item.ScaleText(.5f);
        }

        public override void Init()
        {
            base.Init();

            PauseGame = true;

            ReturnToCallerDelay = 10;
            SlideInLength = 26;
            SlideOutLength = 26;

            this.SlideInFrom = PresetPos.Right;
            this.SlideOutTo = PresetPos.Right;

            FontScale = .8f;

            MyPile = new DrawPile();

            // Make the backdrop
            MakeBackdrop();

            MakeInstructions();

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.Control = Control;

            MakeBack();
            ItemSetup();

            ControlItem item;

            item = new ControlItem("Quick spawn", ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey);
            item.SetSecondaryKey = key => ButtonCheck.Quickspawn_KeyboardKey.Set(key);
			item.Reset = _item => _item.SetKey(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey);
            AddItem(item);

            item = new ControlItem("Power up menu", ButtonCheck.Help_KeyboardKey.KeyboardKey);
            item.SetSecondaryKey = key => ButtonCheck.Help_KeyboardKey.Set(key);
            item.Reset = _item => _item.SetKey(ButtonCheck.Help_KeyboardKey.KeyboardKey);
            AddItem(item);

            //item = new ControlItem("Go/Select", ButtonCheck.Go_Secondary);
            //item.SetSecondaryKey = key => ButtonCheck.Go_Secondary = key;
            //item.Reset = _item => _item.SetKey(ButtonCheck.Go_Secondary);
            //AddItem(item);

            //item = new ControlItem("Back/Cancel", ButtonCheck.Back_Secondary);
            //item.SetSecondaryKey = key => ButtonCheck.Back_Secondary = key;
            //item.Reset = _item => _item.SetKey(ButtonCheck.Back_Secondary);
            //AddItem(item);

            //item = new ControlItem("Start/Menu", ButtonCheck.Start_Secondary);
            //item.SetSecondaryKey = key => ButtonCheck.Start_Secondary = key;
            //item.Reset = _item => _item.SetKey(ButtonCheck.Start_Secondary);
            //AddItem(item);

            item = new ControlItem("Left", ButtonCheck.Left_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.Left_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Left_Secondary);
            AddItem(item);

            item = new ControlItem("Right", ButtonCheck.Right_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.Right_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Right_Secondary);
            AddItem(item);

            item = new ControlItem("Up", ButtonCheck.Up_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.Up_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Up_Secondary);
            AddItem(item);

            item = new ControlItem("Down", ButtonCheck.Down_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.Down_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.Down_Secondary);
            AddItem(item);

            item = new ControlItem("Replay, Previous part", ButtonCheck.ReplayPrev_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.ReplayPrev_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.ReplayPrev_Secondary);
            AddItem(item);

            item = new ControlItem("Replay, Next part", ButtonCheck.ReplayNext_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.ReplayNext_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.ReplayNext_Secondary);
            AddItem(item);

            item = new ControlItem("Replay, Toggle single/multi playback", ButtonCheck.SlowMoToggle_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.ReplayToggle_Secondary = key;
			item.Reset = _item => _item.SetKey(ButtonCheck.SlowMoToggle_Secondary);
            AddItem(item);

            item = new ControlItem("Toggle Slow-mo, (only works when activated)", ButtonCheck.SlowMoToggle_Secondary);
            item.SetSecondaryKey = key => ButtonCheck.SlowMoToggle_Secondary = key;
            item.Reset = _item => _item.SetKey(ButtonCheck.SlowMoToggle_Secondary);
            AddItem(item);

            ButtonCheck.KillSecondary();
            MyMenu.OnX = MyMenu.OnB =
                _m =>
                {
                    Save();
                    return MenuReturnToCaller(_m);
                };

            // Shift everything
            EnsureFancy();

            MyMenu.SelectItem(2);
        }

        private void ItemSetup()
        {
            ItemPos = new Vector2(0f, 700f) + new Vector2(-850, 187);//214.2859f);
            PosAdd = new Vector2(0, -176);//-165);
            FontScale *= .73f;// .778f;
        }


        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active || MyMenu == null || MyMenu.Released) return;

            ControlItem item = MyMenu.CurItem as ControlItem;
            if (null != item)
            {
                foreach (Keys key in ButtonString.KeyToString.Keys)
                    if (ButtonCheck.State(key).Down)
                    {
                        //// Make sure there are no double keys
                        //foreach (MenuItem other in MyMenu.Items)
                        //{
                        //    ControlItem citem = other as ControlItem;
                        //    if (null != citem && citem.MyKey == key)
                        //        citem.SetKey(item.MyKey);
                        //}

                        item.SetKey(key);
                    }
            }
        }
    }
#endif
}