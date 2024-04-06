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
            var args = text.Split(new char[] { ' ' }, 2);

            if (args.Length == 1)
            {
                Commands.Help();
                return;
            }

            switch (args[1].ToLower())
            {
                case "config":
                    {
                        Commands.ConfigInfo();
                        break;
                    }
                case "list":
                    {
                        Commands.List(false);
                        break;
                    }

                case "list nop":
                    {
                        Commands.List(true);
                        break;
                    }

                case "list all":
                    {
                        Commands.ListAll();
                        break;
                    }

                case "delete":
                    {
                        Commands.Delete(false);
                        break;
                    }

                case "delete nop":
                    {
                        Commands.Delete(true);
                        break;
                    }

                case "delete floating":
                    {
                        Commands.DeleteFloating();
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
