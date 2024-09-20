
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ClientSide;

public class MainViewModel : INotifyPropertyChanged
{
    private ImageSource _imageOpenClose;
    public ImageSource ImageOpenClose
    {
        get
        {
            return _imageOpenClose;
        }
        set
        {
            _imageOpenClose = value;
            OnPropertyChanged(nameof(ImageOpenClose));
        }
    }
    readonly UdpClient udpClient;
    IPEndPoint connectEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2701);

    

    public RelayCommand StartCommand { get; set; }

    public MainViewModel()
    {
        udpClient = new UdpClient();
       
        StartCommand = new RelayCommand(executeStart);
    }
 
    private void executeStart(object obj)
    {
       
       var command = new Command() { Start = true };
       var json=JsonSerializer.Serialize(command);
       var bytes= Encoding.UTF8.GetBytes(json);
       udpClient.Send(bytes,bytes.Length, connectEP);
       IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
       byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
       byte[] buffer = new byte[10000];
        var a = $"received_image1.jpeg{DateTime.Now:yyyyMMdd_HHmmss}";
        using (var target = new FileStream(a, FileMode.Create, FileAccess.Write)) {
            target.Write(receivedBytes, 0, receivedBytes.Length);
        }
        string receivedResponse = Encoding.ASCII.GetString(receivedBytes);
        //MessageBox.Show("Received from server: " + receivedResponse);
        Application.Current.Dispatcher.Invoke(() =>
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(a, UriKind.Relative);
            bitmap.EndInit();
            ImageOpenClose = bitmap;
        });
        // Cleanup
      //  udpClient.Close();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
