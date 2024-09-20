using ServerSide;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Timers;

static extern int GetSystemMetrics(int nIndex);

const int SM_CXSCREEN = 0;
const int SM_CYSCREEN = 1;
var client = new UdpClient(2701);
var timer = new System.Timers.Timer(5000); // Set up a timer to trigger every 5000 milliseconds (5 seconds)
var RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
bool isScreenshotting = false;

try
{
    timer.Elapsed += TakeScreenshot; // Assign the method to be called
    timer.AutoReset = true; // Reset the timer after each execution
    timer.Enabled = false; // Start disabled

    while (true)
    {
        var bytes = client.Receive(ref RemoteIpEndPoint);
        var str = Encoding.UTF8.GetString(bytes);
        var command = JsonSerializer.Deserialize<Command>(str);

        if (command!.Start == true && !isScreenshotting)
        {
            isScreenshotting = true;
            timer.Start(); // Start taking screenshots every 5 seconds
            Console.WriteLine("Screenshot mode activated.");
        }
        else if (command!.Start == false)
        {
            timer.Stop(); // Stop taking screenshots
            isScreenshotting = false;
            Console.WriteLine("Screenshot mode deactivated.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.ToString()}");
}

void TakeScreenshot(object source, ElapsedEventArgs e)
{
    string screenshotPath = TakeScreen();
    Console.WriteLine("Screenshot taken.");

    var bts = File.ReadAllBytes(screenshotPath);
    client.Send(bts, bts.Length, RemoteIpEndPoint);
}

string TakeScreen()
{
    // Create a bitmap of the appropriate size
    var width = 200;
    var height =200;
    var bitmap = new Bitmap(width, height);

    // Create a graphics object from the bitmap
    using (var g = Graphics.FromImage(bitmap))
    {
        g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
    }

    // Save the screenshot to a temporary file
    string screenshotPath = Path.Combine(Path.GetTempPath(), $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
    bitmap.Save(screenshotPath, ImageFormat.Png);
    return screenshotPath;
}
