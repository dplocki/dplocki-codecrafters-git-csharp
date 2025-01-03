using System.Text;

internal static class CatFileSubProgram
{
    public static void Run(string[] args)
    {
        if (args.Length < 1 ) {
            Console.WriteLine("Please provide a sub-command parameters.");
            return;
        }

        if (args[0] != "-p") {
            throw new ArgumentException($"Unknown sub-command {args[0]}");
        }

        if (args.Length < 2 ) {
            Console.WriteLine("Please provide a blob hash.");
            return;
        }

        var hash = args[1];
        if (hash.Length != 40) {
            throw new ArgumentException("Provided hash is incorrect");
        }

        using var memoryStream = BlobUntil.DecompressBlob(hash);
        var data = memoryStream.ToArray();
        var nullIndex = Array.IndexOf(data, (byte)0);
        var header = Encoding.UTF8.GetString(data, 0, nullIndex);

        var content = new byte[data.Length - nullIndex - 1];
        Array.Copy(data, nullIndex + 1, content, 0, content.Length);

        string contentString = Encoding.UTF8.GetString(content);
        Console.Write(contentString);
    }
}
