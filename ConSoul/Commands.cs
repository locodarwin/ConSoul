using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConSoul
{
    public partial class MainWindow
    {
        private void Commands(string sName, int iType, int iSession, string sMsg)
        {

            // Get first letter of what was said command; if not a command, abort command mode
            string sPrefix = sMsg.Substring(0, Globals.sComPrefix.Length);
            if (sPrefix != Globals.sComPrefix)
            {
                return;
            }

            // Strip all unneeded parts off the command and split into parameters
            string strip = sMsg.Substring(Globals.sComPrefix.Length);
            strip = strip.Trim();
            string[] cmd = strip.Split(' ');


            switch (cmd[0])
            {
                case "version":
                    DoVersion(sName, iType, iSession, cmd);
                    break;
                case "ver":
                    DoVersion(sName, iType, iSession, cmd);
                    break;


            }
        }

        // Method to respond back on the results of the command
        private void Response(int iSess, int iType, string sMsg)
        {
            if (iType == 2)
            {
                _instance.Whisper(iSess, sMsg);
                //Globals.LogStat = Logging.Chat;
                //Status("(whispered): " + sMsg);
            }
            else
            {
                _instance.Say(sMsg);
                //Globals.LogStat = Logging.Chat;
                //Status(sMsg);
            }

        }


        // Command VERSION
        private void DoVersion(string sName, int iType, int iSess, string[] cmd)
        {
            int iCitnum = GetCitnum(sName);
            //Globals.LogStat = Logging.Command;
            //Status("Command: version (requested by " + sName + " " + iCitnum.ToString() + ")");

            // Check permissions
            if (CheckPerms("version", iCitnum) == false)
            {
                Response(iSess, iType, "Sorry, " + sName + ", but you do not have permission to use the " + cmd[0] + " command.");
                return;
            }
            Response(iSess, iType, Globals.sAppName + " " + Globals.sVersion + " - " + Globals.sByline);
        }



    }
}
