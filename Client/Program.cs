using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class GameClient
    {
        IPEndPoint localEp;
        IPEndPoint remoteEp;
        UdpClient client;
        int localport;
        int remoteport;
        bool ConnectionEstablished = false; 
        string servermessage = null;
        static void Main(string[] args)
        {
            GameClient gc = new GameClient();
            gc.localEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7878);
            gc.remoteEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10888);

            gc.client = new UdpClient(gc.localEp);
            gc.Establish();
            while (gc.ConnectionEstablished)
            {
                gc.servermessage = gc.ReceiveMsgFromServer();
                if (gc.servermessage == "RPLY") gc.SendMsgToServer(Console.ReadLine());
                else Console.WriteLine(gc.servermessage);   
            }
        }

        private void SendMsgToServer(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message.Trim());
            client.Send(buffer, buffer.Length, remoteEp );
        }
        private string ReceiveMsgFromServer()
        {
            byte[] buffer = new byte[1024];
            buffer = client.Receive(ref remoteEp);
            string message = Encoding.UTF8.GetString(buffer);
            return message.Trim();
        }


        private void Establish()
        {            
            string testmessage = "HELO";
            byte[] bytes = Encoding.UTF8.GetBytes(testmessage);
            client.Send(bytes, bytes.Length, remoteEp);
            byte[]  answerbytes = client.Receive(ref remoteEp);
            string answer = Encoding.UTF8.GetString(answerbytes);
            Console.WriteLine("Message from {0}:{1} - {2}", remoteEp.Address, remoteEp.Port, answer);
            ConnectionEstablished = true;


        }
    }
}
