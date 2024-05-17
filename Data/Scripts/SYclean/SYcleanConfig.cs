using Sandbox.ModAPI;
using VRage.Utils;

namespace SYclean
{
    public class SYcleanConfig
    {
        const string configFileName = "SYclean.xml";

        public string BeaconSubtype = "ScrapBeacon";
        public int PlayerRange = 10000;
        public int ScrapBeaconRange = 250;
        public int IntervalMins = 60;
        public bool CleanAtStartup = true;
        public bool CleanFloatingObjects = true;

        public static SYcleanConfig Load()
        {
            SYcleanConfig config = new SYcleanConfig();

            if (MyAPIGateway.Utilities.FileExistsInWorldStorage(configFileName, typeof(SYcleanConfig)))
            {
                MyLog.Default.WriteLine("SYclean: Reading config file");
                using (var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage(configFileName, typeof(SYcleanConfig)))
                {
                    config = MyAPIGateway.Utilities.SerializeFromXML<SYcleanConfig>(reader.ReadToEnd());
                }
                return config;
            }

            MyLog.Default.WriteLine("SYclean: Config file not found, writing default.");
            using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage(configFileName, typeof(SYcleanConfig)))
            {
                var configStr = MyAPIGateway.Utilities.SerializeToXML<SYcleanConfig>(config);
                writer.Write(configStr);
            }
            config.CleanAtStartup = false; //suppress the very first clean
            return config;

        }
    }
}
