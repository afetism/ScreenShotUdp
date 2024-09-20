
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Media.Media3D;

namespace ClientSide;

public class MainViewModel
{
    readonly UdpClient udpClient;
    public RelayCommand StartCommand { get; set; }

    public MainViewModel()
    {
        udpClient = new UdpClient();
        var connectEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27001);
        StartCommand = new RelayCommand(executeStart, a);
    }
    bool a(object o)=> 1 == 0;
    private void executeStart(object obj)
    {
       
       var command = new Command() { Start = true };
       var json=JsonSerializer.Serialize(command);
       var bytes= Encoding.UTF8.GetBytes(json);
       udpClient.Send(bytes);

    }
}
