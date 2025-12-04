class Day02
{
    public static void Run()
    {
        var input = Util.FileToArray("day02/in.txt");
        long maxNum = 0;
        var rangeStrings = input[0].Split(',');
        var ranges = rangeStrings
            .ToList()
            .ConvertAll(
                (s) =>
                {
                    var toks = s.Split('-');
                    var rangeEnd = long.Parse(toks[1]);
                    maxNum = long.Max(maxNum, rangeEnd);
                    return new IdRange(long.Parse(toks[0]), rangeEnd);
                }
            );
        long currNum = 1;
        var invalidIds1 = new HashSet<long>();
        var invalidIds2 = new HashSet<long>();
        while (true)
        {
            var repetitions = 2;
            while (true)
            {
                var invalidIdGuess = long.Parse(
                    string.Concat(Enumerable.Repeat(currNum.ToString(), repetitions))
                );
                if (invalidIdGuess > maxNum)
                    break;
                if (
                    ranges.Any(
                        (range) => invalidIdGuess >= range.Start && invalidIdGuess <= range.End
                    )
                )
                {
                    if (repetitions == 2)
                        invalidIds1.Add(invalidIdGuess);
                    invalidIds2.Add(invalidIdGuess);
                }
                repetitions++;
            }
            if (repetitions == 2)
                break;
            currNum++;
        }
        Console.WriteLine($"Part 1: {invalidIds1.Sum()}");
        Console.WriteLine($"Part 2: {invalidIds2.Sum()}");
    }
}

readonly struct IdRange(long start, long end)
{
    public long Start { get; } = start;
    public long End { get; } = end;
}
