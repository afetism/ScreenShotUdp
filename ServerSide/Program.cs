using ServerSide;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var client = new UdpClient(27001);
try
{
    var RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
  
    while (true)
    {
        var bytes= client.Receive(ref RemoteIpEndPoint);
        var str=Encoding.UTF8.GetString(bytes);
        var Command=JsonSerializer.Deserialize<Command>(str);
        if(Command!.Start==true)
        {
            Console.WriteLine("ScreenShot started");
        }

    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception : {ex.ToString()}");
}

