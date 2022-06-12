using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace Hygenus
{
    //work in progress...
    public class HygenusServer
    {
        Socket serverSocket;
        public const int PORT = 12345;
        public Scene scene;
        private byte[] byteData;
        private List<EndPoint> clients;

        public HygenusServer()
        {
            clients = new List<EndPoint>();
            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, PORT);
            serverSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

            serverSocket.Bind(serverEndPoint);
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref clientEndPoint, DoRecieveFrom, clientEndPoint);

        }

        private void DoRecieveFrom(IAsyncResult iar)
        {
            try
            {
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int len = serverSocket.EndReceiveFrom(iar, ref clientEndPoint);
                byte[] data = new byte[len];
                if (!clients.Exists((ep) => ep.Equals(clientEndPoint)))
                    clients.Add(clientEndPoint);

                EndPoint newClientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref newClientEndPoint, DoRecieveFrom, newClientEndPoint);
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}
