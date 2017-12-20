using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConSoul
{
    public partial class MainWindow
    {

        // Method to get & return citizen number
        private int GetCitnum(string sName)
        {
            _instance.CitizenAttributesByName(sName);
            return _instance.Attributes.CitizenNumber;
        }

        public bool CheckPerms(string sCommand, int iCit)
        {

            string sCit = iCit.ToString();


            // Check to see if command is in the admin list - if so, futher test it
            // If not, assume anyone can issue it
            if (Globals.lAdminCmds.Contains(sCommand))
            {
                // If the cit is owner or admin, permission granted - otherwise, denied!
                if (Globals.lOwners.Contains(sCit) || Globals.lAdmins.Contains(sCit))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }

        // Load the INI file into the globals
        private void LoadINI()
        {
            string sOwners, sAdmins, sAdminCmds;
            string sPort, sCitnum, sAV, sCaretaker;

            var iniFile = new IniFile(Globals.sINI);
            Globals.sUnivLogin = iniFile.GetValue("Login", "loginhost", "auth.activeworlds.com");
            sPort = iniFile.GetValue("Login", "loginport", "6670");
            Globals.sBotName = iniFile.GetValue("Login", "loginname", "Isabel");
            sCitnum = iniFile.GetValue("Login", "logincitnum", "318855");
            Globals.sPassword = iniFile.GetValue("Login", "loginpassword", "test");
            Globals.sWorld = iniFile.GetValue("Login", "loginworld", "Simulator");
            Globals.sCoords = iniFile.GetValue("Login", "logincoords", "0n 0w 0a 0");
            sAV = iniFile.GetValue("Login", "loginav", "19");
            sCaretaker = iniFile.GetValue("Login", "logincaretaker", "yes");

            // Permissions
            sOwners = iniFile.GetValue("Permissions", "ownerlist", "318855");
            sAdmins = iniFile.GetValue("Permissions", "adminlist", "318855");
            sAdminCmds = iniFile.GetValue("Permissions", "admincmds", "stats");

            // Create the Permissions lists
            Globals.lOwners = sOwners.Split(',').ToList();
            Globals.lAdmins = sAdmins.Split(',').ToList();
            Globals.lAdminCmds = sAdminCmds.Split(',').ToList();

            // Command prefix
            Globals.sComPrefix = iniFile.GetValue("Commands", "commandprefix", "/");
            
            // Logging
            Globals.sLogfile = iniFile.GetValue("Logging", "logfile", "botlog.txt");

            // Convert some of the values into their respective types
            Globals.iPort = Convert.ToInt32(sPort);
            Globals.iCitNum = Convert.ToInt32(sCitnum);
            Globals.iAV = Convert.ToInt32(sAV);




        }


        private Coords ConvertCoords(string sCoords)
        {
            Coords cTemp;
            string ns, ew, alt, yaw, b;
            int x, y, z, q, iyaw;
            char g;

            // Lowercase the input string
            sCoords = sCoords.ToLower();

            // Handle north or south in the string
            if (sCoords.Contains("n") || sCoords.Contains("s"))
            {
                q = sCoords.IndexOf("n");
                if (q > -1)
                {
                    ns = sCoords.Substring(0, q);
                    z = ExtractSingleCoord(ns);
                }
                else
                {
                    q = sCoords.IndexOf("s");
                    ns = sCoords.Substring(0, q);
                    z = ExtractSingleCoord(ns);
                    z = z * -1;
                }

            }
            else
            {
                z = 0;
            }

            // Handle eat/west in the string
            if (sCoords.Contains("e") || sCoords.Contains("w"))
            {
                q = sCoords.IndexOf("w");
                if (q > -1)
                {
                    ew = sCoords.Substring(0, q);
                    x = ExtractSingleCoord(ew);
                }
                else
                {
                    q = sCoords.IndexOf("e");
                    ew = sCoords.Substring(0, q);
                    x = ExtractSingleCoord(ew);
                    x = x * -1;
                }

            }
            else
            {
                x = 0;
            }

            // Handle altitude
            if (sCoords.Contains("a"))
            {
                q = sCoords.IndexOf("a");
                if (q > -1)
                {
                    alt = sCoords.Substring(0, q);
                    y = ExtractSingleCoord(alt);
                    y = y / 10;
                }
                else
                {
                    y = 0;
                }
            }
            else
            {
                y = 0;
            }

            // Handle yaw
            q = sCoords.Length;
            yaw = "";
            for (int j = q; j-- > 0;)
            {
                // string temp = s.Substring(a - 1, b); 
                b = sCoords.Substring(j, 1);
                g = Convert.ToChar(b);
                if (Char.IsDigit(g))
                {
                    yaw = yaw + b;
                }
                else
                {
                    break;
                }

            }
            char[] input = yaw.ToCharArray();
            Array.Reverse(input);
            yaw = new string(input);
            iyaw = Convert.ToInt32(yaw);
            iyaw = iyaw * 10;
            if (iyaw < 0)
            {
                iyaw = 0;
            }
            if (iyaw > 3599)
            {
                iyaw = 0;
            }

            // Prepare the output
            cTemp.x = x;
            cTemp.y = y;
            cTemp.z = z;
            cTemp.yaw = iyaw;
            return cTemp;
        }

        private int ExtractSingleCoord(string sIn)
        {
            float value = 0;
            string b;
            string cut = "";
            char g;
            int final;
            int bb = sIn.Length;
            for (int j = bb; j-- > 0;)
            {
                b = sIn.Substring(j, 1);
                g = Convert.ToChar(b);
                if (Char.IsDigit(g) || b == "." || b == "," || b == "-")
                {
                    cut = cut + b;
                }
                else
                {
                    break;
                }

            }
            char[] input = cut.ToCharArray();
            Array.Reverse(input);
            cut = new string(input);
            value = Convert.ToSingle(cut);
            value = value * 1000;
            final = Convert.ToInt32(value);

            return final;
        }

        public void ObjectSpawn(string sModel, string sDesc, string sAction, int iX, int iY, int iZ, int iYaw, int iTilt, int iRoll)
        {
            //int rc;
            _instance.Attributes.ObjectX = iX;
            _instance.Attributes.ObjectY = iY;
            _instance.Attributes.ObjectZ = iZ;
            _instance.Attributes.ObjectYaw = iYaw;
            _instance.Attributes.ObjectTilt = iTilt;
            _instance.Attributes.ObjectRoll = iRoll;
            _instance.Attributes.ObjectModel = sModel;
            _instance.Attributes.ObjectDescription = sDesc;
            _instance.Attributes.ObjectAction = sAction;
            _instance.ObjectAdd();

        }




    }
}
