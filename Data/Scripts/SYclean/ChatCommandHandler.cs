using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;

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
            if(!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
                return;
            }
            send = false;
            var args = text.Split(' ');

            if(args.Length == 1 ) {
                Commands.Help();
                return;
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
                        Commands.List();
                        break;
                    }

                case "listall":
                    {
                        Commands.ListAll();
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
