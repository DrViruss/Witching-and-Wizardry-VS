using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using Vintagestory.API.Client;

namespace witchingandwizardry.Items
{
    public class Chalk : Item
    {
        public static readonly int SIGIL_COUNT = 3;
        public static readonly int ICON_COUNT = 3;

        SkillItem[] modes;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);

            modes = getSkills(api as ICoreClientAPI,Variant["type"]);
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            List<WorldInteraction> interactions = new List<WorldInteraction>(base.GetHeldInteractionHelp(inSlot));
            interactions.Add(new WorldInteraction(){
                    ActionLangCode = "heldhelp-settoolmode",
                    HotKeyCode = "toolmodeselect"
                }
            );

            return interactions.ToArray();
        }


        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel,
            bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
            if(blockSel==null || !(byEntity is EntityPlayer)) return;

            

            if(api.Side == EnumAppSide.Server)
            {
                IWorldAccessor worldAccessor = byEntity.World;
                IPlayer player =worldAccessor.PlayerByUid((byEntity as EntityPlayer).PlayerUID);
                string code ="requireattachable";
                

                if(TryPlaceBlock(worldAccessor,player,blockSel,slot.Itemstack,ref code) && player.WorldData.CurrentGameMode != EnumGameMode.Creative)
                    DamageItem(worldAccessor,byEntity,slot);
            }


            handling = EnumHandHandling.Handled;
        }

        bool TryPlaceBlock(IWorldAccessor world,IPlayer player, BlockSelection blockSel,ItemStack Itemstack,ref string failureCode){
            string CHtype = Itemstack.Item.Variant["type"];
            AssetLocation blockAL = new AssetLocation("witchingandwizardry", $"chalkpainting-{GetBlockPath(CHtype,Itemstack.Attributes.GetInt("drawtype")-1,GetChalkBlock(CHtype))}-{blockSel.Face.Opposite.Code}-{GetBlockRotation(player,blockSel,blockSel.Face.Opposite)}");
            Block block = world.GetBlock(blockAL);

            BlockSelection bs = blockSel.Clone();
            bs.Position.Add(blockSel.Face);
            if(!block.CanPlaceBlock(world, player, bs, ref failureCode)) return false;

            return TryAttachTo(block,world,blockSel.Position,blockSel.Face,player,blockSel);
        }


        bool TryAttachTo(Block block,IWorldAccessor world, BlockPos blockpos, BlockFacing onBlockFace,IPlayer player, BlockSelection blockSel)
        {

            BlockPos attachingBlockPos = blockpos.AddCopy(onBlockFace);
            Block b1 = world.BlockAccessor.GetBlock(blockpos);
            if (b1.CanAttachBlockAt(world.BlockAccessor, block, attachingBlockPos, onBlockFace))
            {
                world.BlockAccessor.SetBlock(block.Id, attachingBlockPos);
                return true;
            }
            return false;
        }

        

        private string GetBlockPath(string CHtype,int drawtype,string name)
        {
            int rand;

            if(CHtype == "magic"||CHtype == "unstable"){
                if(drawtype>SIGIL_COUNT)
                    name="center-"+name;
                rand = new Random().Next(SIGIL_COUNT);
            }
            else
                rand = new Random().Next(ICON_COUNT);
            

            if(!name.Contains("center"))
                name="base-"+name;


            if (drawtype<0)
                name += rand;
            else
                name += drawtype;

            return name;
        }

        private BlockFacing GetBlockRotation(IPlayer player,BlockSelection blockSel,BlockFacing side)
        {
            if(player == null)
            {
                if(side.IsHorizontal)
                    return BlockFacing.NORTH;
                return BlockFacing.UP;
            }

            BlockFacing[] facings = Block.SuggestedHVOrientation(player,blockSel);
            if(facings[1] != null && side.IsHorizontal)
                return facings[1] == BlockFacing.DOWN ? side : side.Opposite;

            return facings[0];
        }

        private string GetChalkBlock(string type) => 
            type == "none" ? $"none-" : 
            type == "magic" ? $"magic-" : 
            $"unstable-";


        #region Skills
	        public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
	        {
	            return modes;
	        }
	
	        public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection, int toolMode)
	        {
	            slot.Itemstack.Attributes.SetInt("drawtype", toolMode);
	        }
	
	        public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection)
	        {
	            return slot.Itemstack.Attributes.GetInt("drawtype", 0);
	        }
	
	        public override void OnUnloaded(ICoreAPI api)
	        {
	            for (int i = 0; modes != null && i < modes.Length; i++)
	            {
	                modes[i]?.Dispose();
	            }
	        }
	
	        private SkillItem[] getSkills(ICoreClientAPI capi, string type)
	        {
	            List<SkillItem> skills = new List<SkillItem>(){
	                genSkill(capi,new AssetLocation("random"),"Random",new AssetLocation("textures/icons/scythetrim.svg"))
	            };
	
	
	            if (type == "none")
	            {
	                skills.Add(genSkill(capi, new AssetLocation("homeicon"), "Home", new AssetLocation("witchingandwizardry", $"textures/icons/chalk/{type}-0.svg")));
	                skills.Add(genSkill(capi, new AssetLocation("xicon"), "X", new AssetLocation("witchingandwizardry", $"textures/icons/chalk/{type}-1.svg")));
	                skills.Add(genSkill(capi, new AssetLocation("arrowicon"), "Arrow", new AssetLocation("witchingandwizardry", $"textures/icons/chalk/{type}-2.svg")));
	
	                if (skills.Count < ICON_COUNT + 1)
	                    api.Logger.Fatal("Not all of ICONS has added as toolmode");
	            }
	
	            if (type == "magic")
	            {
	                skills.Add(genSkill(capi, new AssetLocation("sigil1con"), "Sigil", new AssetLocation("witchingandwizardry", $"textures/icons/chalk/{type}-0.svg")));
	                skills.Add(genSkill(capi, new AssetLocation("sigil2con"), "Sigil", new AssetLocation("witchingandwizardry", $"textures/icons/chalk/{type}-1.svg")));
	                skills.Add(genSkill(capi, new AssetLocation("sigil3con"), "Sigil", new AssetLocation("witchingandwizardry", $"textures/icons/chalk/{type}-2.svg")));
	
	
	                skills.Add(genSkill(capi, new AssetLocation("sigilcenter"), "Center", new AssetLocation("textures/icons/scythetrim.svg")));
	                if (skills.Count < SIGIL_COUNT + 2)
	                    api.Logger.Fatal("Not all of SIGILS has added as toolmode");
	            }
	
	
	            return skills.ToArray();
	        }
	
	        private SkillItem genSkill(ICoreClientAPI capi, AssetLocation code, string name, AssetLocation icon)
	        {
	            var tmp = new SkillItem() { Code = code, Name = name };
	            if (capi != null)
	            {
	                tmp.WithIcon(capi, capi.Gui.LoadSvgWithPadding(icon, 48, 48, 5, ColorUtil.WhiteArgb));
	            }
	            return tmp;
	        }
#endregion
    
    }
}