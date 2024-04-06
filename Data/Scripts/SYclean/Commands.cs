using System.Text;
using VRage.Game.ModAPI;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.Game.Gui;
using System.Collections.Generic;
using VRage.Utils;
using System.Linq;

namespace SYclean
{
    public class Commands
    {
        public static void Help()
        {
            var sb = new StringBuilder();
            sb.AppendLine("!SYclean");
            sb.AppendLine("  Display command help");
            sb.AppendLine("!SYclean config");
            sb.AppendLine("  Show information about current configuration");
            sb.AppendLine("!SYclean list");
            sb.AppendLine("  Show which grids would be deleted");
            sb.AppendLine("!SYclean list nop");
            sb.AppendLine("  Show which grids would be deleted without players");
            sb.AppendLine("!SYclean list all");
            sb.AppendLine("  Show all grids found");
            sb.AppendLine("!SYclean delete");
            sb.AppendLine("  Delete grids that match the Scrapyard rules.");
            sb.AppendLine("!SYclean delete nop");
            sb.AppendLine("  Delete grids that match the Scrapyard rules without players");
            sb.AppendLine("!SYclean delete floating");
            sb.AppendLine("  Delete floating objects.");
            MyAPIGateway.Utilities.ShowMissionScreen("SYclean", "", "Help", sb.ToString(), null, "Close");
        }

        public static void ConfigInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Beacon SubtypeId ends with: {SYclean.Instance.Config.BeaconSubtype}");
            sb.AppendLine("Ranges");
            sb.AppendLine($"  Player: {SYclean.Instance.Config.PlayerRange}");
            sb.AppendLine($"  Scrap Beacon: {SYclean.Instance.Config.ScrapBeaconRange}");
            sb.AppendLine("Other");
            sb.AppendLine($"  Clean At Startup: {SYclean.Instance.Config.CleanAtStartup}");
            sb.AppendLine($"  Clean Floating Objects: {SYclean.Instance.Config.CleanFloatingObjects}");

            MyAPIGateway.Utilities.ShowMissionScreen("SYclean", "", "Information", sb.ToString(), null, "Close");
        }
        public static void Delete(bool ignorePlayers)
        {
            MyLog.Default.WriteLine("SYclean: delete command");
            var c = CommandImp.DeleteGrids(ignorePlayers);
            MyAPIGateway.Utilities.ShowMessage("SYclean", $"Deleted {c} grids matching the Scrapyard rules.");
            MyLog.Default.WriteLine($"SYclean: Deleted {c} grids matching the Scrapyard rules.");
        }

        public static void DeleteFloating()
        {
            var c = CommandImp.DeleteFloatingObjects();
            MyAPIGateway.Utilities.ShowMessage("SYclean", $"Deleted {c} floating objects.");
        }

        public static void List(bool ignorePlayers)
        {
            if (ignorePlayers)
                MyAPIGateway.Utilities.ShowMessage("SYclean", "Players ignored");

            CommandImp.GridData gridData;
            gridData = CommandImp.FilteredGridData(true, ignorePlayers);
            RespondGridData(gridData);
        }

        public static void ListAll()
        {
            CommandImp.GridData gridData;
            gridData = CommandImp.FilteredGridData(false);
            RespondGridData(gridData);
        }

        private static void RespondGridData(CommandImp.GridData gridData)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Active Safe Zones: Player={gridData.PlayerPositions.Count} Beacon={gridData.BeaconPositions.Count}");
            int c = 0;
            foreach (var gridGroup in gridData.GridGroups)
            {
                sb.AppendLine("---");
                foreach (var grid in gridGroup)
                {
                    c++;
                    List<IMySlimBlock> blocks = new List<IMySlimBlock>();
                    grid.GetBlocks(blocks);
                    sb.AppendLine($"  {grid.DisplayName} ({blocks.Count} block(s))");
                }
            }

            MyAPIGateway.Utilities.ShowMissionScreen("SYclean", "", $"Found {gridData.GridGroups.Count} groups, total {c} matching the Scrapyard rules", sb.ToString(), null, "Close");


        }
    }
}
