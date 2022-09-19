using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using witchingandwizardry.BlockEntities;
using witchingandwizardry.Blocks;
using witchingandwizardry.Items;
using witchingandwizardry.GUIs;

[assembly: ModInfo( "Witching and Wizardry",Description = "Magic mod :D",Authors = new []{ "DrViruss" } )]
    
namespace witchingandwizardry
{
    public class Main : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            
            api.RegisterBlockClass("MagicFlower",typeof(MagicFlower));
            api.RegisterBlockEntityClass("MagicFlower", typeof(MagicFlowerBE));
            
            api.RegisterItemClass("Chalk",typeof(Chalk));
            api.RegisterBlockClass("ChalkPainting",typeof(ChalkPainting));
            api.RegisterBlockEntityClass("chalkCenterBE", typeof(ChalkCenterBE));
        }
        
        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Universal || base.ShouldLoad(forSide);
        }
        
        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            api.Logger.Debug("Hello from server");
            
            // Events.Init(api);
            // Worldgen.Init(api);
            // Commands.Init(api);
            // NetworkApiTest.ServerInit(api);
        }
        
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            GuiManager.Init(api);

            // NetworkApiTest.ClientInit(api);
            // GUI.Manager.Init(api);
        }
    }
}