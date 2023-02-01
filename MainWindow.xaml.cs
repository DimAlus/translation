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



        void SetImage(byte[] bts)
        {
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

            });
        }

        public MainWindow ()
        {
            InitializeComponent();
        }
        NetClient client;
        NetServer server;

        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(new System.Drawing.Point(0, 0),
                new System.Drawing.Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                    System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height));
        private async void Server_Click (object sender, RoutedEventArgs e)
        {
            if (server == null)
                server = new NetServer(CaptureRect);
            
        }

        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Black);
        System.Drawing.Rectangle r = new System.Drawing.Rectangle(0, 0, 20, 20);
        System.Drawing.Point p = new System.Drawing.Point();
        System.Drawing.Icon ic = new System.Drawing.Icon("bx-shower.ico");
        byte[] CaptureRect()
        {
            p = System.Windows.Forms.Control.MousePosition;
            r.Location = new System.Drawing.Point((int)p.X - 10, (int)p.Y - 10);
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
            using (var ms = new System.IO.MemoryStream())
            {
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(rect.Width, rect.Height,
                    System.Drawing.Imaging.PixelFormat.Format16bppRgb555))
                {
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, System.Drawing.CopyPixelOperation.SourceCopy);//System.Drawing.CopyPixelOperation.SourceCopy
                        //graphics.DrawEllipse(pen, r);
                        graphics.DrawIcon(ic, r);
                    }
                    
                    bitmap.Save(ms, format);
                }
                byte[] a = ms.ToArray();
                Dispatcher.Invoke(() => TB.Text = a.Length.ToString());
                return a;
            }
        }

        private void Client_Click (object sender, RoutedEventArgs e)
        {
            IPWindow.Visibility = Visibility.Collapsed;
            client = new NetClient(ipadd.Text);
            client.GetData += SetImage;
        }

        private void IpCancle_Click (object sender, RoutedEventArgs e)
        {
            IPWindow.Visibility = Visibility.Collapsed;
        }
        private void IpShow_Click (object sender, RoutedEventArgs e)
        {
            IPWindow.Visibility = Visibility.Visible;
        }
        
    }
}
