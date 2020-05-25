using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    enum RequestType
    {
        ListDirectory,
        GetFileContent
    }

    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            Console.WriteLine(ipAddress);
            var listener = new TcpListener(ipAddress, 12000);
            listener.Start();
            while (listener.Server.IsBound)
            {
                var client = listener.AcceptTcpClient();
                var thread = new Thread(new ParameterizedThreadStart(ClientHandler));
                thread.Start(client);
            }
        }

        static void HandleGetFileContent(BinaryReader reader, BinaryWriter writer)
        {
            var filePath = reader.ReadString();
            try
            {
                var text = File.ReadAllLines(filePath);
                writer.Write(text.Length);
                foreach (var line in text)
                {
                    writer.Write(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                writer.Write(0);
            }
            finally
            {
                writer.Flush();
            }
        }

        static void HandleListDirectory(BinaryReader reader, BinaryWriter writer)
        {
            var path = reader.ReadString();
            try
            {
                var dirInfo = new DirectoryInfo(path);
                var files = new List<string>();
                foreach (var file in dirInfo.GetFiles())
                {
                    files.Add(file.FullName);
                }
                writer.Write(files.Count);

                foreach (var file in files)
                {
                    writer.Write(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                writer.Write(0);
            }
            finally
            {
                writer.Flush();
            }
        }

        static void ClientHandler(object clnt)
        {
            var client = (TcpClient)clnt;
            var reader = new BinaryReader(client.GetStream());
            var writer = new BinaryWriter(client.GetStream());
            while (client.Connected)
            {
                try
                {
                    var request = (RequestType)reader.ReadInt32();
                    switch (request)
                    {
                        case RequestType.GetFileContent:
                            HandleGetFileContent(reader, writer);
                            break;
                        case RequestType.ListDirectory:
                            HandleListDirectory(reader, writer);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
