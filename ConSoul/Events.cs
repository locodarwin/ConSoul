using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW;

namespace ConSoul
{
    public partial class MainWindow
    {

        public void OnEventChat(IInstance sender)
        {

            // Echo the chat (or whisper) to the chat window
            if (sender.Attributes.ChatType == ChatTypes.Whisper)
            {
                //Globals.LogStat = Logging.Chat;
                //Status(sender.Attributes.AvatarName + " (whispered): " + sender.Attributes.ChatMessage);
            }
            else
            {
                //Globals.LogStat = Logging.Chat;
                //Status(sender.Attributes.AvatarName + ": " + sender.Attributes.ChatMessage);
            }

            // If command is whispered rather than stated aloud in chat
            int iType;
            if (sender.Attributes.ChatType == ChatTypes.Whisper)
            {
                iType = 2;
            }
            else
            {
                iType = 1;
            }

            // Send the chat to the parser with avatar name, speaking type, avatar session, and the message
            Commands(sender.Attributes.AvatarName, iType, sender.Attributes.ChatSession, sender.Attributes.ChatMessage);

        }


        public void OnEventCellObject(IInstance sender)
        {
            //Console.WriteLine(sender.Attributes.ObjectModel);
            // Add the columns to the internal datatable

            int ObjID, ObjType, ObjOwner, ObjTS, x, y, z, yaw, tilt, roll;
            string model, desc, act;
            byte[] data;

            ObjID = sender.Attributes.ObjectId;
            ObjType = sender.Attributes.ObjectType;
            ObjOwner = sender.Attributes.ObjectOwner;
            ObjTS = sender.Attributes.ObjectBuildTimestamp;
            x = sender.Attributes.ObjectX;
            y = sender.Attributes.ObjectY;
            z = sender.Attributes.ObjectZ;
            yaw = sender.Attributes.ObjectYaw;
            tilt = sender.Attributes.ObjectTilt;
            roll = sender.Attributes.ObjectRoll;
            model = sender.Attributes.ObjectModel;
            desc = sender.Attributes.ObjectDescription;
            act = sender.Attributes.ObjectAction;
            data = sender.Attributes.ObjectData;



            Globals.PropertyTable.Rows.Add(new Object[] {ObjID, ObjType, ObjOwner, ObjTS, x, y, z, yaw, tilt, roll, model, desc, act, data});




        }





    }
}
