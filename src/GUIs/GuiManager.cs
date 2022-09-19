using Vintagestory.API.Client;
using System.Collections.Generic;

namespace witchingandwizardry.GUIs
{
    public class GuiManager
    {
        private static Dictionary<string, GuiDialog> Dialogs = new Dictionary<string, GuiDialog>();

        public static GuiDialog getGUI(string chalkType){
            return Dialogs[chalkType];
        }



        public static void Init(ICoreClientAPI capi){
            // initChalks(capi);
            
            capi.Logger.Debug("Waw: GUIs succesfull loaded ");
        }

        private static void initChalks(ICoreClientAPI capi){

        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
        ElementBounds textBounds = ElementBounds.Fixed(0, 40, 300, 100);
        ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
        bgBounds.BothSizing = ElementSizing.FitToChildren;
        bgBounds.WithChildren(textBounds);

        

            
        Dialogs.Add("none",new ChalkGui(
                capi,
                capi.Gui.CreateCompo("nonechalkgui", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddStaticText("None chalk GUI", CairoFont.WhiteDetailText(), textBounds)
            ));
        Dialogs.Add("magic",new ChalkGui(
                capi,
                capi.Gui.CreateCompo("magicchalkgui", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddStaticText("Magic chalk GUI", CairoFont.WhiteDetailText(), textBounds)
            ));
        Dialogs.Add("unstable",new ChalkGui(
                capi,
                capi.Gui.CreateCompo("unstablechalkgui", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddStaticText("Temporal chalk GUI", CairoFont.WhiteDetailText(), textBounds)
            ));
            
        }

    }
        
}