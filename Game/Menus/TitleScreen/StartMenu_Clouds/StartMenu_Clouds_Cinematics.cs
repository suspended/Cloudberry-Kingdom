using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartMenu_Clouds_Cinematics : StartMenu
    {
        public TitleGameData_Clouds Title;
        public StartMenu_Clouds_Cinematics(TitleGameData_Clouds Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(TitleBackgroundState.Scene_StoryMode);

            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected void SetText(Text text)
        {
            text.MyFloatColor = new Color(34, 214, 47).ToVector4();
            text.OutlineColor = new Color(0, 0, 0, 0).ToVector4();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            SetText(item.MyText);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
            item.MySelectedText.MyFloatColor = new Color(73, 255, 86).ToVector4(); 
            item.MySelectedText.OutlineColor = new Color(0, 0, 0, 0).ToVector4();

            item.MyOscillateParams.Set(1f, 1.01f, .005f);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            MyMenu.ClearList();

            MakeHeader();

            CreateMenu();
        }

        protected virtual void CreateMenu()
        {
            MenuItem item;

			ItemPos = new Vector2(740f, 58.33334f) + new Vector2(-708.3339f, 216.6667f);
			FontScale = 0.5883336f;
			PosAdd = new Vector2(0, -140);

			for (int i = 0; i < 4; i++)
			{
				item = new CinematicsLevelItem(new Text(Localization.WordString(Localization.Words.Level) + " " + (1 + 10 * i).ToString(), ItemFont), "Cutscene_1");
				item.Name = "Subsection" + i.ToString();
				item.Go = Go;
				AddItem(item);
			}

            MyMenu.SelectItem(0);

            SetPos();
        }

        protected void MakeHeader()
        {
            var Header = new Text(Localization.Words.StoryMode, ItemFont);
            Header.Name = "Header";
            Header.Scale *= 1.3f;
            SetText(Header);
            Header.OutlineColor = Color.Black.ToVector4();
            MyPile.Add(Header);
            
            Header.Pos = new Vector2(-800.0029f, 863.8889f);
        }

        void Go(MenuItem item)
        {
            CinematicsLevelItem c_item = item as CinematicsLevelItem;
            if (null == c_item) return;

			XnaVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(c_item.Movie, 1);
        }

        void SetPos()
        {
        }
    }
}