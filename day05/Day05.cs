class Day05
{
    public static void Run()
    {
        var input = Util.FileToArray("day05/in.txt");
        List<InclusiveRange> ranges = [];
        List<long> ids = [];
        input.ForEach(
            (line) =>
            {
                if (line.Length == 0)
                    return;
                var toks = line.Split("-");
                if (toks.Length == 1)
                {
                    ids.Add(long.Parse(toks[0]));
                }
                else if (toks.Length == 2)
                {
                    ranges.Add(new InclusiveRange(long.Parse(toks[0]), long.Parse(toks[1])));
                }
                else
                {
                    throw new Exception($"Unexpected input: {line}");
                }
            }
        );
        var fresh = ids.Where(id => ranges.Any((range) => range.Contains(id))).Count();
        List<InclusiveRange> finishedRanges = [];
        HashSet<int> removedRangeIndexes = [];
        for (var i = 0; i < ranges.Count; i++)
        {
            if (removedRangeIndexes.Contains(i))
                continue;
            var currRange = ranges[i];
            while (true)
            {
                var indicesToMerge = Enumerable
                    .Range(i + 1, ranges.Count - i - 1)
                    .Where(
                        (index) =>
                        {
                            if (removedRangeIndexes.Contains(index))
                                return false;
                            return ranges[index].CanMerge(currRange);
                        }
                    );
                if (!indicesToMerge.Any())
                    break;
                indicesToMerge
                    .ToList()
                    .ForEach(
                        (index) =>
                        {
                            currRange = currRange.Merge(ranges[index]);
                            removedRangeIndexes.Add(index);
                        }
                    );
            }
            finishedRanges.Add(currRange);
            removedRangeIndexes.Add(i);
        }

        Console.WriteLine($"Part 1: {fresh}");
        Console.WriteLine($"Part 2: {finishedRanges.ConvertAll(range => range.Size()).Sum()}");
    }
}
