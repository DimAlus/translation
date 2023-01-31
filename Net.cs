using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.IO;



using System.Windows;


namespace Translation
{
    delegate void DataEvnt (byte[] data);
    delegate byte[] GetData ();

    class NetClient
    {
        string ipadd = "192.168.43.215";
        IPEndPoint ipEndPoint;
        Socket sender;
        int startPort = 11000;
        int port = 0;

        public event DataEvnt GetData;

        public NetClient(string idaddress)
        {
            ipadd = idaddress;
            new Task(SendMessageFromSocket).Start();
        }

        void StartSend ()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipadd), startPort);
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipEndPoint);

            byte[] bts = new byte[10];
            sender.Receive(bts);
            port = bts[0];
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        void SendMessageFromSocket ()
        {
            if (port == 0)
                StartSend();
            try
            {
                ipEndPoint = new IPEndPoint(IPAddress.Parse(ipadd), startPort + port);
                sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Соединяем сокет с удаленной точкой
                sender.Connect(ipEndPoint);

                byte[] bytes = new byte[9000000];
                while (true)
                {
                    sender.Send(new byte[] { 0x1F });
                    int size = sender.Receive(bytes);
                    byte[] bts = new byte[size];
                    Array.Copy(bytes, bts, size);
                    GetData(bts);

                }

                // Освобождаем сокет
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch { }
            try
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch { }

        }

    }
    
    class NetServer
    {
        int port = 0;
        int startPort = 11000;
        bool EndWork = false;

        public GetData getData;

        List<Socket> MainSockets = new List<Socket>();
        List<Socket> Sockets = new List<Socket>();
        bool added = true;
        byte[] data = null;

        Task dataUpdater;


        async void UpdateData ()
        {
            while (!EndWork)
            {
                data = getData();
                await Task.Delay(1000 / 15);
            }
        }

        public NetServer (GetData GeterData)
        {
            getData = GeterData;
            dataUpdater = new Task(UpdateData);
            dataUpdater.Start();
        }


        async void ServiceSocket()
        {
            int integ = MainSockets.Count;
            Socket mainSocket = MainSockets[MainSockets.Count - 1];
            added = true;

            Socket handler = mainSocket.Accept();
            byte[] rcvbts = new byte[100];
            while (!EndWork)
            {
                try
                {
                    handler.Receive(rcvbts);
                    if (rcvbts[0] == 0x1F)
                    {
                        while (data == null)
                            await Task.Delay(10);
                        handler.Send(data);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error of added socket " + integ.ToString());
                }
            }
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

            mainSocket.Shutdown(SocketShutdown.Both);
            mainSocket.Close();
            MainSockets.Remove(mainSocket);
        }

        async void CreateSocket(int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, startPort + port);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(endPoint);
            serverSocket.Listen(20);

            while (!added)
                await Task.Delay(10);
            MainSockets.Add(serverSocket);
            added = false;
            new Task(ServiceSocket).Start();

        }

        void StartSend ()
        {

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, startPort);

            Socket myServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            myServerSocket.Bind(ipEndPoint);
            myServerSocket.Listen(20);

            while (!EndWork)
            {
                try
                {
                    Socket handler = myServerSocket.Accept();
                    handler.Send(new byte[] { (byte)++port });

                    CreateSocket(port);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error of main socket");
                }
            }

            myServerSocket.Shutdown(SocketShutdown.Both);
            myServerSocket.Close();
        }
    }
}
