using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using VRage.Utils;

namespace SYclean
{
    public class ChatCommandHandler
    {
        const string prefix = "!SYclean";

        public void Register()
        {
            MyAPIGateway.Utilities.MessageEntered += MessageEntered;
        }


        public void Unregister()
        {
            MyAPIGateway.Utilities.MessageEntered -= MessageEntered;
        }

        void MessageEntered(string text, ref bool send)
        {
            if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            send = false;
            var args = text.Split(' ');
            MyAPIGateway.Utilities.ShowMessage("SYclean", $"args length {args.Length}");
            if (args.Length == 1)
            {
                Commands.Help();
                return;
            }

            bool ignorePlayers = false;

            if (args.Length > 2)
            {
                switch (args[2].ToLower())
                {
                    case "nop":
                        {
                            ignorePlayers = true;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            switch (args[1].ToLower())
            {
                case "info":
                    {
                        Commands.Info();
                        break;
                    }
                case "list":
                    {
                        Commands.List(ignorePlayers);
                        break;
                    }

                case "listall":
                    {
                        Commands.ListAll();
                        break;
                    }

                case "delete":
                    {
                        Commands.Delete(ignorePlayers);
                        break;
                    }
                default:
                    {
                        MyAPIGateway.Utilities.ShowMessage("SYclean", "Unrecognised command");
                        break;
                    }
            }
        }
    }
}
