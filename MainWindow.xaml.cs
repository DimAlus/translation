using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Net;
using System.Net.Sockets;
using System.IO;





namespace Translation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage img = new BitmapImage();

        /*byte[] bytes = new byte[9294400];
        int size = 0;
        bool blockTask = false;
        bool blockSend = false;

        async void tsk ()
        {
            while (true)
            {
                await Task.Delay(10);
                if (size > 0)
                {
                    while (blockSend)
                        await Task.Delay(10);
                    blockTask = true;
                    byte[] bts = new byte[size];
                    Array.Copy(bytes, bts, size);
                    blockTask = false;
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {

                            img = new BitmapImage();
                            img.BeginInit();
                            img.StreamSource = new MemoryStream(bts);
                            img.EndInit();
                            Imag.Source = img;
                        }
                        catch (Exception e)
                        {
                            //MessageBox.Show(e.ToString());
                        }

                    }
                    );
                }
            }
        }*/
        public MainWindow ()
        {
            InitializeComponent();/*
            Task t = new Task(tsk);
            t.Start();*/
        }
        NetClient client;
        NetServer server;

        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new System.Drawing.Point(0, 0),
                new System.Drawing.Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                    System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height));
        private async void Server_Click (object sender, RoutedEventArgs e)
        {
            server = new NetServer(CaptureRect);
            /* IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 11000);

             Socket myServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);*/
            /*IPEndPoint ipEndPoint;
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            if (isLocal)
                ipEndPoint = new IPEndPoint(ipAddr, 11000);
            else
                ipEndPoint = new IPEndPoint(IPAddress.Any, 11000);


            Socket myServerSocket = new Socket(isLocal ? ipAddr.AddressFamily : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); ;


            try
            {
                myServerSocket.Bind(ipEndPoint);
                myServerSocket.Listen(20);
                Socket handler = await myServerSocket.AcceptAsync();

                while (true)
                {
                    await Task.Delay(1000 / 30);
                    handler.Send(CaptureRect(rect, System.Drawing.Imaging.ImageFormat.Png));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }*/
        }


        byte[] CaptureRect ()
        {
           // System.Drawing.Rectangle rect;
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
            using (var ms = new System.IO.MemoryStream())
            {
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(rect.Width, rect.Height,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, System.Drawing.CopyPixelOperation.SourceCopy);
                    }
                    
                    bitmap.Save(ms, format);
                }/*
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();*/
                return ms.ToArray();
            }
        }
        //bool isLocal = false;
        private void Client_Click (object sender, RoutedEventArgs e)
        {
            IPWindow.Visibility = Visibility.Collapsed;
            client = new NetClient(ipadd.Text);
            //SendMessageFromSocket(11000);
        }
        //
        async void SendMessageFromSocket (int port)
        {
           /* try
            {

                // Буфер для входящих данных


                // Соединяемся с удаленным устройством

                // Устанавливаем удаленную точку для сокета
               /* IPEndPoint ipEndPoint;
                IPHostEntry ipHost = Dns.GetHostEntry("localhost");
                IPAddress ipAddr = ipHost.AddressList[0];
                if (isLocal)
                    ipEndPoint = new IPEndPoint(ipAddr, port);
                else
                    ipEndPoint = new IPEndPoint(IPAddress.Parse(/*"192.168.43.215"/*//*ipadd.Text), 11000);


                Socket sender = new Socket(isLocal ? ipAddr.AddressFamily : AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); ;

                // Соединяем сокет с удаленной точкой
                await sender.ConnectAsync(ipEndPoint);

                while (true)
                {

                    // Получаем ответ от сервера
                    while (blockTask)
                        await Task.Delay(10);
                    blockSend = true;
                    size = await sender.ReceiveAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None);//(bytes);
                    blockSend = false;

                    await Task.Delay(100);
                    /*
                    byte[] arr = new byte[bytesRec];

                    img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = new MemoryStream(bytes, 0, bytesRec);
                    img.EndInit();
                    Imag.Source = img;
                    //break;*/
              /*  }

                // Освобождаем сокет
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            size = 0;*/
        }

        private void IpCancle_Click (object sender, RoutedEventArgs e)
        {
            IPWindow.Visibility = Visibility.Collapsed;
        }
        private void IpShow_Click (object sender, RoutedEventArgs e)
        {
            IPWindow.Visibility = Visibility.Visible;
        }
        /*
        private void CheckBox_Click (object sender, RoutedEventArgs e)
        {
            isLocal = (bool)((CheckBox)sender).IsChecked;
        }*/
    }
}
