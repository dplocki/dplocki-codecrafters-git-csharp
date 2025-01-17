using System.Text;

internal class LsTreeSubProgram
{
    private const byte Null = 0;
    private const byte Space = 32;

    public static void Run(string hash, bool nameOnly)
    {
        using var memoryStream = BlobUntil.DecompressBlob(hash);

        var data = memoryStream.ToArray();
        var spaceIndex = Array.IndexOf(data, Space);
        var nullIndex = Array.IndexOf(data, Null);
        var contentSize = int.Parse(Encoding.UTF8.GetString(data, spaceIndex, nullIndex - spaceIndex));

        foreach (var item in ParseTreeEntries(data, nullIndex + 1))
        {
            if (nameOnly)
            {
                Console.WriteLine(item.Name);
            }
            else
            {
                Console.WriteLine($"{item.Mode} {item.Type} {item.Hash} {item.Name}");
            }
        }
    }

    private static IEnumerable<TreeLine> ParseTreeEntries(byte[] data, int tokenBegin)
    {
        do
        {
            int tokenEnd = Array.IndexOf(data, Space, tokenBegin);
            var mode = Encoding.UTF8.GetString(data, tokenBegin, tokenEnd - tokenBegin);
            tokenBegin = tokenEnd + 1;

            var type = (mode == "40000") ? "tree" : "blob";

            tokenEnd = Array.IndexOf(data, Null, tokenBegin);
            var name = Encoding.UTF8.GetString(data, tokenBegin, tokenEnd - tokenBegin);
            tokenBegin = tokenEnd + 1;

            var hash = new byte[Hash.Length];
            Array.Copy(data, tokenBegin, hash, 0, Hash.Length);
            tokenBegin += Hash.Length;

            yield return new TreeLine
            {
                Mode = mode,
                Type = type,
                Hash = new Hash(hash),
                Name = name
            };
        }
        while (tokenBegin < data.Length);
    }
}
