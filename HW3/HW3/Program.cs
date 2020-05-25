using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace HW3
{
    class Program
    {
        static Rijndael getAlgorithm(string passwordBytes)
        {
            var cryptoAlg = Rijndael.Create();
            byte[] saltBytes = Encoding.UTF8.GetBytes("SaltBytes");
            var p = new Rfc2898DeriveBytes(passwordBytes, saltBytes);
            cryptoAlg.IV = p.GetBytes(cryptoAlg.BlockSize / 8);
            cryptoAlg.Key = p.GetBytes(cryptoAlg.KeySize / 8);
            return cryptoAlg;
        }

        static void Encrypt(string dirPath, string outPath, string password)
        {
            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine("Directory does not exist");
                return;
            }
            var dirInfo = new DirectoryInfo(dirPath);
            var cryptoAlg = getAlgorithm(password);
            var cryptoTransform = cryptoAlg.CreateEncryptor();
            try
            {
                var outStream = new FileStream(outPath, FileMode.OpenOrCreate);
                var cryptoStream = new CryptoStream(outStream, cryptoTransform, CryptoStreamMode.Write);
                var gzipStream = new GZipStream(cryptoStream, CompressionMode.Compress);
                var bWriter = new BinaryWriter(gzipStream);
                bWriter.Write(dirInfo.GetFiles().Length);
                foreach (var file in dirInfo.GetFiles())
                {
                    bWriter.Write(file.Name);
                    bWriter.Write(file.Length);
                }
                foreach (var file in dirInfo.GetFiles())
                {
                    using (var inputFile = new FileStream(file.FullName, FileMode.Open))
                    {
                        inputFile.CopyTo(gzipStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        static void Decrypt(string filePath, string outDir, string password)
        {
            var cryptoAlg = getAlgorithm(password);
            var cryptoTransform = cryptoAlg.CreateDecryptor();
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            try
            {
                var inStream = new FileStream(filePath, FileMode.Open);
                var cryptoStream = new CryptoStream(inStream, cryptoTransform, CryptoStreamMode.Read);
                var gzipStream = new GZipStream(cryptoStream, CompressionMode.Decompress);
                var bReader = new BinaryReader(gzipStream);
                var fileCount = bReader.ReadInt32();
                var fileNames = new List<string>();
                var fileSizes = new List<long>();
                for (int i = 0; i < fileCount; ++i)
                {
                    var fileName = bReader.ReadString();
                    var fileSize = bReader.ReadInt64();
                    fileNames.Add(fileName);
                    fileSizes.Add(fileSize);
                }
                for (int i = 0; i < fileCount; ++i)
                {
                    var file = bReader.ReadBytes((int)fileSizes[i]);
                    Console.WriteLine("Decrypting " + fileNames[i]);
                    using (var outFile = new FileStream(outDir + "\\" + fileNames[i], FileMode.OpenOrCreate))
                    {
                        outFile.Write(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        static void Main(string[] args)
        {
            if (args[0] == "encrypt")
            {
                Encrypt(args[1], args[2], args[3]);
                return;
            }
            if (args[0] == "decrypt")
            {
                Decrypt(args[1], args[2],args[3]);
                return;
            }

            Console.WriteLine("Bad first argument.");
            
        }
    }
}
