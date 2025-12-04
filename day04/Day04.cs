class Day04
{
    public static void Run()
    {
        var input = Util.FileToArray("day04/in.txt");
        var currLocations = Util.ListToLocations(input);
        var p1 = GetRemovableLocations(currLocations).Count;
        var p2 = 0;
        while (true)
        {
            var toRemove = GetRemovableLocations(currLocations);
            if (toRemove.Count == 0)
                break;
            p2 += toRemove.Count;
            currLocations = RemoveLocations(currLocations, toRemove);
        }

        Console.WriteLine($"Part 1: {p1}");
        Console.WriteLine($"Part 2: {p2}");
    }

    private static List<Loc> GetRemovableLocations(Dictionary<Loc, char> locations)
    {
        return
        [
            .. locations.Keys.Where(
                (loc) =>
                {
                    if (locations[loc] != '@')
                        return false;
                    var adj8 = loc.Adj8()
                        .ConvertAll(adjLoc => locations.GetValueOrDefault(adjLoc, '.'));
                    return adj8.Where(c => c == '@').Count() < 4;
                }
            ),
        ];
    }

    private static Dictionary<Loc, char> RemoveLocations(
        Dictionary<Loc, char> locations,
        List<Loc> toRemove
    )
    {
        var newLocations = new Dictionary<Loc, char>(locations);
        toRemove.ForEach(loc =>
        {
            newLocations.Remove(loc);
        });
        return newLocations;
    }
}
