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
        private ChatCommandHandler commandHandler;

        public override void LoadData()
        {
            Instance = this;
            ticks = 0;

            MyLog.Default.WriteLine("Init SYclean");

            if (!MyAPIGateway.Session.IsServer)
            {
                MyLog.Default.WriteLine("SYclean: This is not the game server, not starting.");
                return;
            }

            Config = SYcleanConfig.Load();

            commandHandler = new ChatCommandHandler();
            commandHandler.Register();

        }

        protected override void UnloadData()
        {
            Instance = null; // important for avoiding this object to remain allocated in memory
            commandHandler.Unregister();
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