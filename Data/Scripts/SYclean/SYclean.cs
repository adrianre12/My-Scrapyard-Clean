using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Utils;

namespace SYclean
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class SYclean : MySessionComponentBase
    {

        public static SYclean Instance; // the only way to access session comp from other classes and the only accepted static field.

        public SYcleanConfig Config;
        private int ticks;

        public override void LoadData()
        {
            Instance = this;
            ticks = 0;

            MyLog.Default.WriteLine("Init SYclean");

            Config = SYcleanConfig.Load();

        }

        public override void UpdateBeforeSimulation()
        {
            // executed every tick, 60 times a second, before physics simulation and only if game is not paused.
            ++ticks;
            if (ticks > 600) {
                MyAPIGateway.Utilities.ShowMessage("Sclean", $"{Config.BeaconSubtype}");
                ticks = 0;
            }
        }
       
    }
}