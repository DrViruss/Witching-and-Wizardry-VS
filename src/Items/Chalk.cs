using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;

namespace witchingandwizardry.Items
{
    public class Chalk : Item
    {
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

            IWorldAccessor worldAccessor = byEntity.World;
            IPlayer player =worldAccessor.PlayerByUid((byEntity as EntityPlayer).PlayerUID);
            AssetLocation al = GetBlockAL(slot.Itemstack.Attributes.GetInt("drawtype")-1,"chalkpainting-"+GetChalkBlock(slot.Itemstack.Item.Variant["type"]) , blockSel, player);
            Block block = worldAccessor.GetBlock(al);

            if(TryPlaceBlock(worldAccessor,blockSel, block) && player.WorldData.CurrentGameMode != EnumGameMode.Creative)
                DamageItem(worldAccessor,byEntity,slot);
        }
        
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




        
        public bool TryPlaceBlock(IWorldAccessor world, BlockSelection blockSel, Block block)
        {
            BlockPos pos = blockSel.Position;
            if(world.BlockAccessor.GetBlock(pos).CanAttachBlockAt(world.BlockAccessor,block, pos.AddCopy(blockSel.Face), blockSel.Face) && !world.BlockAccessor.GetBlock(pos.AddCopy(blockSel.Face)).Code.Path.Contains("chalkpainting")){
                world.BlockAccessor.SetBlock(block.Id, pos.AddCopy(blockSel.Face));
                return true;
            }
            return false;
        }

        private AssetLocation GetBlockAL(int drawtype,string name,BlockSelection blockSel,IPlayer player)
        {
            if (drawtype <0)
                name += $"{new Random().Next(3)}-{blockSel.Face.Opposite}-{GetBlockRotation(player,blockSel,blockSel.Face.Opposite).Code}";
            else
                name += drawtype+$"-{blockSel.Face.Opposite}-{GetBlockRotation(player,blockSel,blockSel.Face.Opposite).Code}";
            
            return new AssetLocation("witchingandwizardry", name);
        }

        private BlockFacing GetBlockRotation(IPlayer player,BlockSelection blockSel,BlockFacing side){
            BlockFacing[] facings = Block.SuggestedHVOrientation(player,blockSel);
            if(facings[1] != null && side.IsHorizontal)
                return facings[1] == BlockFacing.DOWN ? side : side.Opposite;

            return facings[0];
        }

        private string GetChalkBlock(string type) => 
            type == "none" ? $"icon-none-" : 
            type == "magic" ? $"sigil-magic-" : 
            $"sigil-unstable-";

        private SkillItem[] getSkills(ICoreClientAPI capi,string type){
            List<SkillItem> skills = new List<SkillItem>(){
                genSkill(capi,new AssetLocation("randomicon"),"Random",new AssetLocation("textures/icons/scythetrim.svg"))
            };


            if(type == "none")
            {
                skills.Add(genSkill(capi,new AssetLocation("homeicon"),"Home",new AssetLocation("witchingandwizardry",$"textures/icons/chalk/{type}-0.svg")));
                skills.Add(genSkill(capi,new AssetLocation("xicon"),"X",new AssetLocation("witchingandwizardry",$"textures/icons/chalk/{type}-1.svg")));
                skills.Add(genSkill(capi,new AssetLocation("arrowicon"),"Arrow",new AssetLocation("witchingandwizardry",$"textures/icons/chalk/{type}-2.svg")));
            }

            if(type == "magic")
            {
                skills.Add(genSkill(capi,new AssetLocation("sigil1con"),"Sigil",new AssetLocation("witchingandwizardry",$"textures/icons/chalk/{type}-0.svg")));
                skills.Add(genSkill(capi,new AssetLocation("sigil2con"),"Sigil",new AssetLocation("witchingandwizardry",$"textures/icons/chalk/{type}-1.svg")));
                skills.Add(genSkill(capi,new AssetLocation("sigil3con"),"Sigil",new AssetLocation("witchingandwizardry",$"textures/icons/chalk/{type}-2.svg")));
            }


            return skills.ToArray();
        }

        private SkillItem genSkill(ICoreClientAPI capi,AssetLocation code, string name,AssetLocation icon){
            var tmp = new SkillItem(){Code = code, Name = name};
            if (capi != null){
                tmp.WithIcon(capi, capi.Gui.LoadSvgWithPadding(icon, 48, 48, 5, ColorUtil.WhiteArgb));
            }
            return tmp;
        }
    }
}