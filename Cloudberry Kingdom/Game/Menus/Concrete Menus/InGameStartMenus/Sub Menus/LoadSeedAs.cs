using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class LoadSeedAs : EnterTextGui
    {
        PlayerData Player;

        public LoadSeedAs(int Control, PlayerData Player)
            : base(false)
        {
            this.Player = Player;

            HeaderWord = Localization.Words.LoadSeed;
            SuggestedString = GetDefaultString();

            Constructor(Control);
        }

        private static string GetDefaultString()
        {
            string clipboard = null;

#if WINDOWS && !MONO
            try
            {
                clipboard = System.Windows.Forms.Clipboard.GetText();
            }
            catch
            {
                clipboard = "<Error>";
            }
#endif

            if (clipboard == null || clipboard.Length == 0)
                clipboard = "Type in a seed!";

            clipboard = Tools.SantitizeOneLineString(clipboard, Resources.LilFont);
            return clipboard;
        }

        protected override void OnEnter()
        {
            if (TextBox.Text.Length <= 0)
                return;

            // Save the seed
            if (TextBox.Text.Length > 0)
            {
                OnSuccess();
            }
            else
            {
                OnFailure();
            }

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
                Active = false;
            }
        }

        protected override void OnFailure()
        {
            // Failure!
            ReturnToCaller(false);
        }

        protected override void OnSuccess()
        {
            SavedSeedsGUI.LoadSeed(TextBox.Text, this);
            Active = false;
            ReturnToCaller();
        }
    }
}