using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using Vintagestory.API.MathTools;

namespace witchingandwizardry.Blocks
{
    public class ChalkPainting : Block
    {
        public override BlockDropItemStack[] GetDropsForHandbook(ItemStack handbookStack, IPlayer forPlayer)
        {
            return GetHandbookDropsFromBreakDrops(handbookStack, forPlayer);
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
        {
            return new ItemStack[] { new ItemStack() };
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
        {
            return new ItemStack();
        }

        public override string GetPlacedBlockName(IWorldAccessor world, BlockPos pos)
        {
            return "Chalk drowing";
        }

        public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
        {
            if (!CanBlockStay(world, pos))
                world.BlockAccessor.BreakBlock(pos, null);
        }

        bool CanBlockStay(IWorldAccessor world, BlockPos pos)
        {
            BlockFacing facing = BlockFacing.FromCode(this.LastCodePart(1));
            return world.BlockAccessor.GetBlock(pos.AddCopy(facing)).CanAttachBlockAt(world.BlockAccessor, this, pos.AddCopy(facing), facing.Opposite);
        }

        public override bool CanAttachBlockAt(IBlockAccessor world, Block block, BlockPos pos, BlockFacing blockFace, Cuboidi attachmentArea = null)
        {
            return false;
        }
    }
}