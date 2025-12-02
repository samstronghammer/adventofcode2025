class Util
{
    public static List<string> fileToArray(string file)
    {
        // Open the text file using a stream reader.
        using StreamReader reader = new(file);

        // Read the stream as a string.
        string text = reader.ReadToEnd();
        return text.Split("\n").ToList().ConvertAll((line) => line.Trim());
    }
}
