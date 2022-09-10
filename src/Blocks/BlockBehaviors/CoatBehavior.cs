using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace witchingandwizardry.Blocks.BlockBehaviors
{
    public class CoatBehavior : BlockBehavior
    {
        private string _orientName;
        public CoatBehavior(Block block) : base(block){}
        
        public override void Initialize(JsonObject properties)
        {
            _orientName = properties["orientName"].AsString("side");
        }

        public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
        {
            BlockFacing facing = BlockFacing.FromCode(block.Variant[_orientName]);
            if (pos.AddCopy(facing) != neibpos) return;

            world.BlockAccessor.SetBlock(0,pos);
            world.Api.Logger.Warning("ChalkBlock loose his AttachBlock. REMOVING!@");
        }
    }
}