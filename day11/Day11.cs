class Day11
{
    public static void Run()
    {
        var input = Util.FileToArray("day11/in.txt");
        Dictionary<string, List<string>> devices = [];
        input.ForEach(line =>
        {
            var toks = line.Split(": ");
            var deviceId = toks[0];
            var outIds = toks[1].Split(' ').ToList();
            devices[deviceId] = outIds;
        });
        Console.WriteLine($"Part 1: {CountWays(devices, "you")}");
        Console.WriteLine($"Part 2: {CountWays2(devices, "svr", [], [])}");
    }

    public static long CountWays(Dictionary<string, List<string>> devices, string location)
    {
        if (location == "out")
            return 1;
        var forwardIds = devices[location];
        var forwardWays = forwardIds.ConvertAll(id => CountWays(devices, id)).Sum();
        return forwardWays;
    }

    public static long CountWays2(
        Dictionary<string, List<string>> devices,
        string location,
        HashSet<string> path,
        Dictionary<string, long> cachedAnswers
    )
    {
        if (path.Contains(location))
            return 0;
        HashSet<string> mySet = [.. path];
        mySet.Add(location);
        var myId = $"{location},{mySet.Contains("dac")},{mySet.Contains("fft")}";
        if (cachedAnswers.TryGetValue(myId, out long value))
            return value;
        if (location == "out")
        {
            return mySet.Contains("dac") && mySet.Contains("fft") ? 1 : 0;
        }
        var forwardIds = devices[location];
        var forwardWays = forwardIds
            .ConvertAll(id => CountWays2(devices, id, mySet, cachedAnswers))
            .Sum();
        cachedAnswers[myId] = forwardWays;
        return forwardWays;
    }
}
