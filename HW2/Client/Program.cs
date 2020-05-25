using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    enum RequestType
    {
        ListDirectory,
        GetFileContent
    }

    class Program
    {
        static void Connect(RequestType requestType, string path)
        {
            TcpClient client = new TcpClient();
            client.Connect(new IPAddress(new byte[] { 127, 0, 0, 1 }), 12000);

            var reader = new BinaryReader(client.GetStream());
            var writer = new BinaryWriter(client.GetStream());
            writer.Write((int)requestType);
            writer.Write(path);
            writer.Flush();
            var lineCount = reader.ReadInt32();
            for (int i = 0; i < lineCount; ++i)
            {
                var line = reader.ReadString();
                Console.WriteLine(line);
            }
        }

        static void Main(string[] args)
        {
            int requestType = int.Parse(Console.ReadLine());
            string path = Console.ReadLine();
            try
            {
                Connect((RequestType)requestType, path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
