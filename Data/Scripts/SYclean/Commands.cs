using System.Text;
using VRage.Game.ModAPI;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.Game.Gui;
using System.Collections.Generic;

namespace SYclean { 
    public class Commands 
    {
        public static void Help()
        {
            var sb = new StringBuilder();
            sb.AppendLine("!SYclean");
            sb.AppendLine("  Display command help");
            sb.AppendLine("!SYclean info");
            sb.AppendLine("  Show information about current configuration");
            sb.AppendLine("!SYclean list");
            sb.AppendLine("  Show which grids would be deleted");
            sb.AppendLine("!SYclean list all");
            sb.AppendLine("  Show all grids found");
            sb.AppendLine("!SYclean delete");
            sb.AppendLine("  Delete grids that match the Scrapyard rules.");
            MyAPIGateway.Utilities.ShowMissionScreen("SYclean", "", "Help", sb.ToString(), null, "Close");
        }

        public static void Info()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Beacon SubtypeId ends with: {SYclean.Instance.Config.BeaconSubtype}");
            sb.AppendLine("Ranges");
            sb.AppendLine($"  Player: {SYclean.Instance.Config.PlayerRange}");
            sb.AppendLine($"  Scrap Beacon: {SYclean.Instance.Config.ScrapBeaconRange}");
            sb.AppendLine("Other"); 
            sb.AppendLine($"  Clean At Startup: {SYclean.Instance.Config.CleanAtStartup}");
            sb.AppendLine($"  Clean Floating Objects: {SYclean.Instance.Config.CleanFloatingObjects}");
            sb.AppendLine($"  Reset All Voxels: {SYclean.Instance.Config.VoxelReset}");

            MyAPIGateway.Utilities.ShowMissionScreen("SYclean", "", "Information", sb.ToString(), null, "Close");
        }
        /*       public void Delete()
               {
                   Log.Info("delete command");
                   CommandImp.GridData gridData = CommandImp.FilteredGridData(true);

                   var c = 0;
                   foreach ( var gridGroup in gridData.GridGroups ) {
                       foreach (var grid in gridGroup)
                       {
                           c++;
                           Log.Info($"Deleting grid: {grid.EntityId}: {grid.DisplayName}");

                           //Eject Pilot
                           var blocks = grid.GetFatBlocks<MyCockpit>();
                           foreach (var cockpit in blocks)
                           {
                               cockpit.RemovePilot();
                           }

                           grid.Close();
                       }
                   }
                   Context.Respond($"Deleted {c} grids matching the Scrapyard rules.");
                   Log.Info($"Sclean deleted {c} grids matching the Scrapyard rules.");

               }
        */
        public static void List() {
            CommandImp.GridData gridData;
            gridData = CommandImp.FilteredGridData(true);
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
                foreach(var grid in gridGroup)
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
