using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Utils;

namespace SYclean
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class SYclean : MySessionComponentBase
    {
        const int ticksPerMin = 3600;

        public static SYclean Instance; // the only way to access session comp from other classes and the only accepted static field.

        public SYcleanConfig Config;
        private int ticks;
        private int intervalTicks;
        private bool cleanNow;
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
            cleanNow = Config.CleanAtStartup;
            intervalTicks = Config.IntervalMins * ticksPerMin;

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
            if (cleanNow && ticks > 600)
            {
                MyAPIGateway.Utilities.ShowMessage("SYclean", "Startup Clean");
                MyLog.Default.WriteLine("SYclean: Startup Clean");

                var c = Commands.DeleteGrids(true);
                MyAPIGateway.Utilities.ShowMessage("SYclean", $"Deleted {c} grids matching the Scrapyard rules.");
                MyLog.Default.WriteLine($"SYclean: Deleted {c} grids matching the Scrapyard rules.");

                c = Commands.DeleteFloatingObjects();
                MyAPIGateway.Utilities.ShowMessage("SYclean", $"Deleted {c} floating objects.");
                MyLog.Default.WriteLine($"SYclean: Deleted {c} floating objects.");

                cleanNow = false;
            }
            if (ticks > intervalTicks)
            {
                MyAPIGateway.Utilities.ShowMessage("SYclean", "Cleanup");
                MyLog.Default.WriteLine("SYclean: Timmed Clean");

                var c = Commands.DeleteGrids(false);
                MyAPIGateway.Utilities.ShowMessage("SYclean", $"Deleted {c} grids matching the Scrapyard rules.");
                MyLog.Default.WriteLine($"SYclean: Deleted {c} grids matching the Scrapyard rules.");

                c = Commands.DeleteFloatingObjects();
                MyAPIGateway.Utilities.ShowMessage("SYclean", $"Deleted {c} floating objects.");
                MyLog.Default.WriteLine($"SYclean: Deleted {c} floating objects.");

                ticks = 0;
            }
        }

    }
}