using System;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace witchingandwizardry.BlockEntities
{
    public class MagicFlowerBE: BlockEntity
    { 
      private static readonly Random Rand = new Random();
      private double _lastCheckAtTotalDays;
      private double _transitionHoursLeft = -1.0;
      private double? _totalDaysForNextStageOld;
      private RoomRegistry _roomreg;
      private int _roomness;
      
      public override void Initialize(ICoreAPI api)
      {
        base.Initialize(api);
        if (!(api is ICoreServerAPI)) return;
        if (_transitionHoursLeft <= 0.0)
        {
          _transitionHoursLeft = GetHoursForNextStage();
          _lastCheckAtTotalDays = api.World.Calendar.TotalDays;
        }
        RegisterGameTickListener(CheckGrow, 800);
        _roomreg = Api.ModLoader.GetModSystem<RoomRegistry>();
        if (!_totalDaysForNextStageOld.HasValue) return;
        _transitionHoursLeft = (_totalDaysForNextStageOld.Value - Api.World.Calendar.TotalDays) * Api.World.Calendar.HoursPerDay;
      }

      private void CheckGrow(float dt)
      {
        if (Block.Attributes == null || Api.World.BlockAccessor.GetBlock(Pos).LastCodePart(1)=="flowering")
          return;
        _lastCheckAtTotalDays = Math.Min(_lastCheckAtTotalDays, Api.World.Calendar.TotalDays);
        double num1 = GameMath.Mod(Api.World.Calendar.TotalDays - _lastCheckAtTotalDays, Api.World.Calendar.DaysPerYear);
        ClimateCondition climateAt1 = Api.World.BlockAccessor.GetClimateAt(Pos);
        if (climateAt1 == null) return;
        float temperature = climateAt1.Temperature;
        bool flag1 = false;
        float num2 = 1f / Api.World.Calendar.HoursPerDay;
        float num3 = 0.0f;
        float num4 = 0.0f;
        float num5 = 0.0f;
        float num6 = 0.0f;
        float num7 = 0.0f;
        float num8 = 0.0f;
        
        if (num1 > num2)
        {
          num3 = Block.Attributes["resetBelowTemperature"].AsFloat(-999f);
          num4 = Block.Attributes["resetAboveTemperature"].AsFloat(999f);
          num5 = Block.Attributes["stopBelowTemperature"].AsFloat(-999f);
          num6 = Block.Attributes["stopAboveTemperature"].AsFloat(999f);
          num7 = Block.Attributes["revertBlockBelowTemperature"].AsFloat(-999f);
          num8 = Block.Attributes["revertBlockAboveTemperature"].AsFloat(999f);
          
          if (Api.World.BlockAccessor.GetRainMapHeightAt(Pos) > Pos.Y)
          {
            Room roomForPosition = _roomreg?.GetRoomForPosition(Pos);
            _roomness = roomForPosition == null || roomForPosition.SkylightCount <= roomForPosition.NonSkylightCount || roomForPosition.ExitCount != 0 ? 0 : 1;
          }
          else _roomness = 0;
          flag1 = true;
        }
        
        
        while (num1 >  num2)
        {
          num1 -=  num2;
          _lastCheckAtTotalDays +=  num2;
          --_transitionHoursLeft;
          climateAt1.Temperature = temperature;
          ClimateCondition climateAt2 = Api.World.BlockAccessor.GetClimateAt(Pos, climateAt1, EnumGetClimateMode.ForSuppliedDate_TemperatureOnly, _lastCheckAtTotalDays);
          if (_roomness > 0) climateAt2.Temperature += 5f;
          bool flag2 =  climateAt2.Temperature <  num3 ||  climateAt2.Temperature >  num4;
          int num9 =  climateAt2.Temperature <  num5 ? 1 : ( climateAt2.Temperature >  num6 ? 1 : 0);
          bool flag3 =  climateAt2.Temperature <  num7 ||  climateAt2.Temperature >  num8;
          int num10 = flag2 ? 1 : 0;
          if ((num9 | num10) != 0)
          {
            ++_transitionHoursLeft;
            if (flag2)
            {
              _transitionHoursLeft = GetHoursForNextStage();
              if (flag3 && Block.Variant["state"] != "sprout")
                Api.World.BlockAccessor.ExchangeBlock(Api.World.GetBlock(Block.CodeWithVariant("state", "sprout")).BlockId, Pos);
            }
          }
          else if (_transitionHoursLeft <= 0.0)
          {
            if (!DoGrow())
              return;
            _transitionHoursLeft = GetHoursForNextStage();
          }
        }
        if (!flag1)
          return;
        MarkDirty();
      }

      private double GetHoursForNextStage() => IsFlowering() ? 4.0 * (5.0 + Rand.NextDouble()) * 0.8 *  Api.World.Calendar.HoursPerDay : (5.0 + Rand.NextDouble()) * 0.8 *  Api.World.Calendar.HoursPerDay;

      private bool IsFlowering() => Api.World.BlockAccessor.GetBlock(Pos).LastCodePart() == "flowering";

      private bool DoGrow()
      {
        Block block1 = Api.World.BlockAccessor.GetBlock(Pos);
        string str = block1.LastCodePart(1);
        AssetLocation blockCode = block1.CodeWithPart(str == "sprout" ? "harvested" : (str == "harvested" ? "flowering" : "sprout"),2);
        if (!blockCode.Valid)
        {
          Api.World.BlockAccessor.RemoveBlockEntity(Pos);
          return false;
        }
        Block block2 = Api.World.GetBlock(blockCode);
        if (block2?.Code == null)
          return false;
        Api.World.BlockAccessor.ExchangeBlock(block2.BlockId, Pos);
        MarkDirty(true);
        return true;
      }

      public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
      {
        base.FromTreeAttributes(tree,worldForResolving);
        _transitionHoursLeft = tree.GetDouble("transitionHoursLeft");
        if (tree.HasAttribute("totalDaysForNextStage"))
          _totalDaysForNextStageOld = tree.GetDouble("totalDaysForNextStage");
        _lastCheckAtTotalDays = tree.GetDouble("lastCheckAtTotalDays");
        _roomness = tree.GetInt("roomness");
      }

      public override void ToTreeAttributes(ITreeAttribute tree)
      {
        base.ToTreeAttributes(tree);
        tree.SetDouble("transitionHoursLeft", _transitionHoursLeft);
        tree.SetDouble("lastCheckAtTotalDays", _lastCheckAtTotalDays);
        tree.SetInt("roomness", _roomness);
      }

      public override void GetBlockInfo(IPlayer forPlayer, StringBuilder sb)
      {
        if (true) //if Player Knows
        {
          Block block = Api.World.BlockAccessor.GetBlock(Pos);
          string state = block.LastCodePart(1);
          string type = block.LastCodePart(2);
          sb.AppendLine(type.Replace(type[0], char.ToUpper(type[0])));
          sb.AppendLine(state.Replace(state[0], char.ToUpper(state[0])));
        }
      }
    }
}