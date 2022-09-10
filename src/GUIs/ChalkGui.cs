using Vintagestory.API.Client;

namespace witchingandwizardry.GUIs
{
    public class ChalkGui : GuiDialog
    {
        

        public override string ToggleKeyCombinationCode => "chalkdrawingui";

        public ChalkGui(ICoreClientAPI capi,GuiComposer composer) : base(capi)
        {
            SingleComposer = composer
                .AddDialogTitleBar("Hello from TitleBar!!", OnCloseClicked)
                .Compose();
        }

        private void OnCloseClicked(){
            TryClose();
        }

    }
        
}