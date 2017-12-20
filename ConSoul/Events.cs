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

        private void OnEventChat(IInstance sender)
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


    }
}
