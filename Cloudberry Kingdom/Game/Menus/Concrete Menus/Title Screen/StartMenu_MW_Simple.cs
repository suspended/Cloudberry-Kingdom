using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Simple : StartMenu_MW
    {
        public StartMenu_MW_Simple(TitleGameData_MW Title)
            : base(Title)
        {
        }

        protected override void MakeMenu()
        {
            MenuItem item;

            // Arcade
            item = new MenuItem(new EzText(Localization.Words.TheArcade, ItemFont, true));
            item.Name = "Arcade";
            item.Go = MenuGo_Arcade;
            AddItem(item);

            // Campaign
            item = new MenuItem(new EzText(Localization.Words.StoryMode, ItemFont, true));
            item.Name = "Campaign";
            AddItem(item);
            item.Go = MenuGo_Campaign;

            // Free Play
            item = new MenuItem(new EzText(Localization.Words.FreePlay, ItemFont, true));
            item.Name = "Freeplay";
            item.Go = MenuGo_Freeplay;
            AddItem(item);

            // Options
            item = new MenuItem(new EzText(Localization.Words.Options, ItemFont, true));
            item.Name = "Options";
            item.Go = MenuGo_Options;
            AddItem(item);

            // Exit
            item = new MenuItem(new EzText(Localization.Words.Exit, ItemFont, true));
            item.Name = "Exit";
            item.Go = MenuGo_Exit;
            AddItem(item);

            EnsureFancy();

            this.CallToLeft = true;
        }

        protected override void SetPos()
        {
            BackBox.TextureName = "White";
            BackBox.Quad.SetColor(ColorHelper.Gray(.1f));
            BackBox.Alpha = .73f;

            MenuItem _item;
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(0f, 365.5279f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(0f, 160.3057f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(0f, -26.47217f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(0f, -216.0278f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(0f, -419.1389f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-80.55566f, -219.4445f);

            QuadClass _q;
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-61.11133f, -336.1111f); _q.Size = new Vector2(524.4158f, 524.4158f); }

            MyPile.Pos = new Vector2(-27.77734f, -33.33337f);
        }
    }
}