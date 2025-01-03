using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

internal class HashObjectSubProgram
{
    public static void Run(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please provide a sub-command parameters.");
            return;
        }

        if (args[0] != "-w")
        {
            throw new ArgumentException($"Unknown sub-command {args[0]}");
        }

        if (args.Length < 2)
        {
            Console.WriteLine("Please provide a file.");
            return;
        }

        var inputFilePath = args[1];

        // Compress the file after appending the additional data
        using var memoryStream = new MemoryStream();
        using var inputFileStream = File.Open(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var additionalBytes = Encoding.UTF8.GetBytes($"blob {inputFileStream.Length}\0");

        memoryStream.Write(additionalBytes, 0, additionalBytes.Length);
        inputFileStream.CopyTo(memoryStream);

        var hash = CalculateHash(memoryStream);

        BlobUntil.CreateDirectoryForHash(hash);

        memoryStream.Position = 0;

        using var outputFileStream = File.Create(BlobUntil.GetPathForHash(hash));
        using var compressStream = new ZLibStream(outputFileStream, CompressionMode.Compress);

        memoryStream.CopyTo(compressStream);

        Console.WriteLine(hash);
    }

    private static string CalculateHash(Stream memoryStream)
    {
        using var sha1 = SHA1.Create();

        memoryStream.Position = 0;
        var hashBytes = sha1.ComputeHash(memoryStream);
        return Convert.ToHexStringLower(hashBytes);
    }
}
