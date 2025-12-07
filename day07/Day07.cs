class Day07
{
    public static void Run()
    {
        var input = Util.FileToArray("day07/in.txt");
        var locations = Util.ListToLocations(input);
        var laserStartColumn = input[0].IndexOf('S');
        HashSet<int> laserColumns = [laserStartColumn];
        Dictionary<int, long> numTimelines = [];
        numTimelines.Add(laserStartColumn, 1);
        var numSplits = 0;
        for (var row = 1; row < input.Count; row++)
        {
            HashSet<int> newLaserColumns = [];
            Dictionary<int, long> newNumTimelines = [];
            laserColumns
                .ToList()
                .ForEach(
                    (col) =>
                    {
                        var c = locations[new Loc(row, col)];
                        var timelines = numTimelines[col];
                        void DoAdd(int val)
                        {
                            newLaserColumns.Add(val);
                            newNumTimelines[val] =
                                timelines + newNumTimelines.GetValueOrDefault(val);
                        }
                        if (c == '^')
                        {
                            DoAdd(col + 1);
                            DoAdd(col - 1);
                            numSplits++;
                        }
                        else
                        {
                            DoAdd(col);
                        }
                    }
                );
            laserColumns = newLaserColumns;
            numTimelines = newNumTimelines;
        }

        Console.WriteLine($"Part 1: {numSplits}");
        Console.WriteLine($"Part 2: {numTimelines.Values.Sum()}");
    }
}
