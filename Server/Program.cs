using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace Server
{
    class Program
    {
        private const int port = 10888;
        IPEndPoint localEP;
        IPEndPoint remoteEP;
        
        UdpClient server;
        private bool ConnectionEstablished;

        //public Program()
        //{
        //    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //    localEP = new IPEndPoint(IPAddress.Any,port);
        //    socket.Bind(localEP);
        //    ConnectionEstablished = false;
        //}

        public Program()
        {
            localEP = new IPEndPoint(IPAddress.Any, port);
            server = new UdpClient(localEP);
            //IPEndPoint
            //client.Send()
        }

        static void Main(string[] args)
        {

            Program myProgram = new Program();
            myProgram.Establish();

            Game myGame = new Game();
            myGame.getdatafromplayer1 = ReadFromConsole;
            myGame.getdatafromplayer2 = myProgram.ReceiveMsgFromClient;
            myGame.writemessagePlayer1 = WriteToConsole;
            myGame.writemessagePlayer2 = myProgram.SendMsgToClient;
            myGame.Go();


        }

        private static void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }

        private static string ReadFromConsole()
        {
            return Console.ReadLine();
        }

        
        private  void SendMsgToClient(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            server.Send(buffer, buffer.Length, remoteEP);
        }
        private string ReceiveMsgFromClient()
        {
            SendMsgToClient("RPLY");
            byte[] buffer = new byte[1024];
            buffer= server.Receive(ref remoteEP);
            string message = Encoding.UTF8.GetString(buffer);
            return message.Trim() ;
        }

        
        private void Establish()
        {
            remoteEP = new IPEndPoint(IPAddress.Any,port);
            
             byte[] bytes =   server.Receive(ref remoteEP);
             Console.WriteLine("Message from {0}:{1} - {2}", remoteEP.Address, remoteEP.Port, Encoding.ASCII.GetString(bytes));
             string testmessage = "HELO";
             bytes = Encoding.ASCII.GetBytes(testmessage);
             server.Send(bytes, bytes.Length, remoteEP);
             ConnectionEstablished = true;
            
           
        }



    }
}
