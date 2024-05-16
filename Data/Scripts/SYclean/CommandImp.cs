using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace SYclean
{
    public static class CommandImp
    {
        /// <summary>
        /// Performs the distance filter on gridData
        /// </summary>
        /// <returns></returns>
        public static GridData FilteredGridData(bool filter, bool ignorePlayers = false)
        {
            int playerRange = SYclean.Instance.Config.PlayerRange;
            int playerRangeSqr = playerRange * playerRange;
            int beaconRange = SYclean.Instance.Config.ScrapBeaconRange;
            int beaconRangeSqr = beaconRange * beaconRange;
            int maxPlayerBeaconOffset = playerRange - beaconRange;
            int maxPlayerBeaconOffsetSqr = maxPlayerBeaconOffset * maxPlayerBeaconOffset;

            // Grid specific filtering is done in GetGridData
            GridData gridData = GetGridData(filter);
            GridData filteredGridData = new GridData
            {
                GridGroups = new List<List<IMyCubeGrid>>(),
                BeaconPositions = new List<Vector3D>(),
                PlayerPositions = new List<Vector3D>(),
            };

            // get player positions
            if (!ignorePlayers) //Hack way but it will work
            {
                List<IMyPlayer> players = new List<IMyPlayer>();
                MyAPIGateway.Players.GetPlayers(players);
                foreach (var player in players)
                {
                    filteredGridData.PlayerPositions.Add(player.GetPosition());
                }
            }

            //Only do if Player range is greater than beacon range
            if (playerRange > beaconRange)
            {
                bool useBeacon;
                foreach (var beaconPosition in gridData.BeaconPositions)
                {
                    useBeacon = true;
                    foreach (var playerPosition in filteredGridData.PlayerPositions)
                    {
                        if (Vector3D.DistanceSquared(beaconPosition, playerPosition) < maxPlayerBeaconOffsetSqr)
                        {
                            useBeacon = false;
                        }
                    }
                    if (useBeacon)
                    {
                        filteredGridData.BeaconPositions.Add(beaconPosition);
                    }
                }
            }
            else
            {
                MyLog.Default.WriteLine("SYclean: PlayerRange is less than BeaconRange, beacon optimisation not done.");
            }

            // filter by distance from player and beacon. Non player grids also have to be protected.
            bool useGroup;
            foreach (var gridGroup in gridData.GridGroups)
            {
                useGroup = true;
                foreach (var grid in gridGroup)
                {
                    foreach (var playerPosition in filteredGridData.PlayerPositions)
                    {
                        if (Vector3D.DistanceSquared(grid.PositionComp.GetPosition(), playerPosition) < playerRangeSqr)
                        {
                            useGroup = false;
                            break;
                        }
                    }

                    foreach (var beaconPosition in filteredGridData.BeaconPositions)
                    {
                        if (Vector3D.DistanceSquared(grid.PositionComp.GetPosition(), beaconPosition) < beaconRangeSqr)
                        {
                            useGroup = false;
                            break;
                        }
                    }

                    if (!useGroup)
                        break;
                }

                if (!filter || useGroup)
                {
                    filteredGridData.GridGroups.Add(gridGroup);
                }
            }

            return filteredGridData;
        }


        public struct GridData
        {
            public List<List<IMyCubeGrid>> GridGroups;
            public List<Vector3D> BeaconPositions;
            public List<Vector3D> PlayerPositions;

            public int CountGrids()
            {
                int c = 0;
                foreach (var grid in GridGroups)
                {
                    c += grid.Count;
                }
                return c;
            }
        }

        /// <summary>
        /// Scan all the grids to find those elegable for removal by grid features (powered etc) and Scrap Beacon positions. If filter = false the list does not have inelegable grids removed.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static GridData GetGridData(bool filter)
        {
            var gridData = new GridData();
            var gridGroups = new List<List<IMyCubeGrid>>();
            var beaconPositions = new List<Vector3D>();

            List<long> NPCids = GetNPCids();
            List<IMyGridGroupData> groups = new List<IMyGridGroupData>();
            MyAPIGateway.GridGroups.GetGridGroups(GridLinkTypeEnum.Logical, groups);
            MyAPIGateway.Parallel.ForEach(groups, (group) =>
            {
                //Due to the locking do two stages, first does all the filtering and takes a long time. Second is a quick add to results.
                int c = 0;
                bool use = true;
                List<IMyCubeGrid> grids = new List<IMyCubeGrid>();
                group.GetGrids(grids);
                foreach (IMyCubeGrid grid in grids) // have lost the check for projectors
                {
                    var gridInfo = GetGridInfo(grid, NPCids);

                    // has player beacon
                    if (gridInfo.BeaconPositions.Count > 0)
                    {
                        lock (beaconPositions)
                        {
                            beaconPositions.AddRange(gridInfo.BeaconPositions);
                        }
                        use = false; // Not really needed as it would be filtered later by the beacon zone but it is an optimisation.
                    }

                    // player grid and has power
                    if (gridInfo.Owner == OwnerType.Player && gridInfo.IsPowered)
                    {
                        use = false;
                    }
                    //Log.Info($"Grid: {node.NodeData.DisplayName} use: {use} #Beacons: {gridInfo.BeaconPositions.Count} Owner: {gridInfo.Owner} IsPowered: {gridInfo.IsPowered}");
                }

                if (!filter || use)
                {
                    lock (gridGroups)
                    {
                        List<IMyCubeGrid> gridGroup = new List<IMyCubeGrid>();
                        foreach (var grid in grids)
                        {
                            gridGroup.Add(grid);
                            c++;
                        }
                        gridGroups.Add(gridGroup);
                        //Log.Info($"GridGroups Added group size {gridGroup.Count}");
                    }
                }

            });

            gridData.GridGroups = gridGroups;
            gridData.BeaconPositions = beaconPositions;

            MyLog.Default.WriteLine($"SYclean: GridGroups count {gridGroups.Count}");
            return gridData;
        }

        private struct GridInfo
        {
            public List<Vector3D> BeaconPositions;
            public bool IsPowered;
            public OwnerType Owner;
        }

        /// <summary>
        /// OwnerType Enumerator.
        /// </summary>
        public enum OwnerType
        {
            Nobody,
            NPC,
            Player
        }

        /// <summary>
        /// Consolodated scanning of a grid to retrieve the info in one pass
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private static GridInfo GetGridInfo(IMyCubeGrid grid, List<long> NPCids)
        {
            GridInfo gridInfo = new GridInfo
            {
                BeaconPositions = new List<Vector3D>()
            };

            MyResourceSourceComponent component;
            long ownerId;
            string endsWith = SYclean.Instance.Config.BeaconSubtype;

            foreach (var block in ((MyCubeGrid)grid).GetFatBlocks())
            {
                //Log.Info($"grid name>{grid.DisplayName} TypeId: {block.BlockDefinition.Id.TypeId.ToString()}");

                if (block.BlockDefinition.Id.SubtypeId.ToString().EndsWith(endsWith))
                {
                    //Log.Info($"grid name>{grid.DisplayName} Found SubtypeId: {block.BlockDefinition.Id.SubtypeId}");
                    gridInfo.BeaconPositions.Add(block.PositionComp.GetPosition());
                }


                component = block.Components?.Get<MyResourceSourceComponent>();
                if (component != null && component.ResourceTypes.Contains(MyResourceDistributorComponent.ElectricityId))
                {
                    if (component.HasCapacityRemainingByType(MyResourceDistributorComponent.ElectricityId) && component.ProductionEnabledByType(MyResourceDistributorComponent.ElectricityId))
                        gridInfo.IsPowered = true;
                }
            }

            if (grid.BigOwners.Count > 0 && grid.BigOwners[0] != 0)
                ownerId = grid.BigOwners[0];
            else if (grid.BigOwners.Count > 1)
                ownerId = grid.BigOwners[1];
            else
                ownerId = 0L;

            if (ownerId == 0L)
                gridInfo.Owner = OwnerType.Nobody;
            else if (NPCids.Contains(ownerId))
                gridInfo.Owner = OwnerType.NPC;
            else
                gridInfo.Owner = OwnerType.Player;

            return gridInfo;
        }

        public static List<long> GetNPCids()
        {
            List<long> NPCids = new List<long>();
            var factions = MyAPIGateway.Session.Factions.Factions;
            foreach (var faction in factions.Values)
            {
                if (!faction.IsEveryoneNpc())
                    continue;
                foreach (var member in faction.Members.Values)
                {
                    NPCids.Add(member.PlayerId);
                }
            }
            return NPCids;
        }

        public static int DeleteGrids(bool ignorePlayers)
        {
            if (ignorePlayers)
                MyAPIGateway.Utilities.ShowMessage("SYclean", "Players ignored");

            CommandImp.GridData gridData = CommandImp.FilteredGridData(true, ignorePlayers);

            var c = 0;
            foreach (var gridGroup in gridData.GridGroups)
            {
                foreach (var grid in gridGroup)
                {
                    c++;
                    MyLog.Default.WriteLine($"SYclean: Deleting grid: {grid.EntityId}: {grid.DisplayName}");

                    //Eject Pilot
                    var blocks = grid.GetFatBlocks<MyCockpit>();
                    foreach (var cockpit in blocks)
                    {
                        cockpit.RemovePilot();
                    }

                    grid.Close();
                }
            }
            return c;
        }


        public static int DeleteFloatingObjects()
        {
            MyLog.Default.WriteLine("SYclean: delete floating command");
            var count = 0;
            foreach (var floater in MyEntities.GetEntities().OfType<MyFloatingObject>())
            {
                MyLog.Default.WriteLine($"SYclean: Deleting floating object: {floater.DisplayName}");
                floater.Close();
                count++;
            }
            MyLog.Default.WriteLine($"SYclean: Cleanup deleted {count} floating objects");
            return count;
        }
    }
}
