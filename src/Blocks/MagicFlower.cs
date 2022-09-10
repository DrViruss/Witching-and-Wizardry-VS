using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace witchingandwizardry.Blocks
{
    public class MagicFlower : BlockPlant
    {
        //WorldInteraction[] interactions = null; //Like in BlockReeds
        public override float OnGettingBroken(IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter)
        {
            if (Variant["state"] == "harvested")
            {
                dt /= 2f;
            }
            else
            {
                EnumTool? activeTool = player.InventoryManager.ActiveTool;
                EnumTool enumTool = EnumTool.Knife;
                if (!(activeTool.GetValueOrDefault() == enumTool & activeTool.HasValue))
                {
                    dt /= 3f;
                }
                else
                {
                    float num;
                    if (itemslot.Itemstack.Collectible.MiningSpeed.TryGetValue(EnumBlockMaterial.Plant, out num))
                        dt *= num;
                }
            }
            
            float num1 = RequiredMiningTier == 0 ? remainingResistance - dt : remainingResistance;
            if (counter % 5 == 0 ||  num1 <= 0.0)
            {
                double posx =  blockSel.Position.X + blockSel.HitPosition.X;
                double posy =  blockSel.Position.Y + blockSel.HitPosition.Y;
                double posz =  blockSel.Position.Z + blockSel.HitPosition.Z;
                player.Entity.World.PlaySoundAt( num1 > 0.0 ? Sounds.GetHitSound(player) : Sounds.GetBreakSound(player), posx, posy, posz, player, true, 16f, 1f);
            }
            return num1;
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            if (byPlayer != null)
            {
                EnumTool? activeTool = byPlayer.InventoryManager.ActiveTool;
                if (byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative)
                {
                    if (world.Side == EnumAppSide.Server)
                    {
                        foreach (var bdrop in Drops)
                        {
                            if (!bdrop.Tool.HasValue ||
                                (bdrop.Tool.HasValue && activeTool.HasValue && bdrop.Tool == activeTool))
                            {
                                ItemStack drop = bdrop.GetNextItemStack();
                                if (drop != null)
                                    world.SpawnItemEntity(drop, new Vec3d(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5));
                            }
                        }

                        world.PlaySoundAt(Sounds.GetBreakSound(byPlayer), pos.X, pos.Y, pos.Z, byPlayer);
                    }


                    if (Variant["state"] == "flowering" && activeTool == EnumTool.Knife)
                    {
                        int blockId = world.GetBlock(CodeWithVariant("state", "harvested")).BlockId;
                        world.BlockAccessor.SetBlock(blockId, pos);
                        return;
                    }

                    if (activeTool == EnumTool.Shovel)
                    {
                        BlockPos downPos = pos.Copy().Down();
                        int blockId = world.GetBlock(world.BlockAccessor.GetBlock(downPos).CodeWithVariant("grasscoverage", "none")).BlockId;
                        world.BlockAccessor.SetBlock(blockId, downPos);
                    }
                }
            }
            world.BlockAccessor.GetBlockEntity(pos)?.OnBlockBroken(byPlayer);
            world.BlockAccessor.SetBlock(0,pos);
        }
    }
}