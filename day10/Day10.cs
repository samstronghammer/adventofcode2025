class Day10
{
    public static void Run()
    {
        var input = Util.FileToArray("day10/in.txt");
        List<WiringSchematic> schematics = input.ConvertAll(StringToSchematic);

        Console.WriteLine($"Part 1: {schematics.ConvertAll(NumPressesNeeded).Sum()}");
        Console.WriteLine($"Part 2 test: {NumPressesNeededP2(schematics[0])}");
        // Console.WriteLine($"Part 2: {schematics.ConvertAll(NumPressesNeededP2).Sum()}");
    }

    private static int NumPressesNeeded(WiringSchematic schematic)
    {
        Dictionary<string, int> minPresses = [];
        HashSet<string> done = [];
        minPresses.Add(schematic.CalcLightsString(), 0);
        Queue<WiringSchematic> frontier = [];
        frontier.Enqueue(schematic);
        while (true)
        {
            var next = frontier.Dequeue();
            if (next.IsDoneP1())
                return minPresses[next.CalcLightsGoalString()];
            var nextString = next.CalcLightsString();
            if (done.Contains(nextString))
                continue;
            var nextDist = minPresses[nextString] + 1;
            for (var i = 0; i < schematic.Buttons.Count; i++)
            {
                var toAdd = next.PushButton(i);
                var toAddString = toAdd.CalcLightsString();
                if (minPresses.ContainsKey(toAddString))
                    continue;
                frontier.Enqueue(toAdd);
                minPresses[toAdd.CalcLightsString()] = nextDist;
            }
            done.Add(nextString);
        }
    }

    private static int? NumPressesNeededP2(WiringSchematic schematic)
    {
        var buttons = schematic.Buttons;
        var start = new JoltageList(schematic.Joltages);
        var end = new JoltageList(schematic.JoltagesGoal.ConvertAll(val => val / 3));
        var totalDistance = end.Magnitude();
        var joltageTotals = new JoltageList(schematic.Joltages);
        buttons.ForEach(button => joltageTotals = joltageTotals.PushButton(button));

        Func<JoltageList, int> h = (JoltageList from) =>
        {
            if (from.Equals(end))
                return 0;
            if (from.IsOverjolted(end))
                return int.MaxValue;
            var distDone = from.Magnitude();
            var expectedLocation = new JoltageList(
                end.Joltages.ConvertAll((val) => val * distDone / totalDistance)
            );
            var distLeft = (end - from).Magnitude();
            var distFromExpected = (expectedLocation - from).Magnitude();
            if (distDone * 20 > totalDistance * 19)
            {
                Console.WriteLine("close");
                // return distLeft + distFromExpected * 3;
            }
            return distLeft + distFromExpected * 10;

            // TODO need two gradients-- one for distance from the end, one for distance from straight line there.

            // var dist = from
            //     .Joltages.Zip(end.Joltages, joltageTotals.Joltages)
            //     .Aggregate(
            //         0,
            //         (acc, val) =>
            //         {
            //             var diff = Math.Abs(val.First - val.Second);
            //             return acc + diff;
            //             // var buttonPower = val.Third;
            //             // var pushesNeeded = diff == 0 ? 0 : diff / buttonPower;
            //             // return acc + pushesNeeded;
            //         }
            //     );
            // Console.WriteLine($"{from}, {end}, {dist}, {joltageTotals}");
            // return dist;
            // return squareDist;
            // return Math.Sqrt(squareDist);
        };

        // static string joltageToString(JoltageList joltages) => string.Join(",", joltages);
        Dictionary<JoltageList, int> gScore = []; // KNOWN CHEAPEST PATH
        gScore[start] = 0;
        var openSet = new PriorityQueue<JoltageList, int>();
        openSet.Enqueue(start, h(start));
        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            Console.WriteLine($"{current}, {h(current)}, {(end - current).Magnitude()}");
            if (current == end)
                return gScore[current];
            buttons.ForEach(button =>
            {
                var neighbor = current.PushButton(button);
                if (neighbor.IsOverjolted(end))
                    return;
                var tentative_gScore = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor])
                {
                    gScore[neighbor] = tentative_gScore;
                    openSet.Remove(neighbor, out _, out _);
                    openSet.Enqueue(neighbor, tentative_gScore + h(neighbor));
                }
            });
        }
        return -1;
    }

    // private static int? NumPressesNeededP2(WiringSchematic schematic)
    // {
    //     Console.WriteLine(schematic.CalcJoltagesString());
    //     if (schematic.IsDoneP2())
    //         return 0;
    //     if (schematic.IsOverjolted())
    //         return null;
    //     for (var i = 0; i < schematic.Buttons.Count; i++)
    //     {
    //         var toAdd = schematic.PushButton(i);
    //         var result = NumPressesNeededP2(toAdd);
    //         if (result == null)
    //             continue;
    //         return result + 1;
    //     }
    //     return null;
    // }

    // private static int NumPressesNeededP2Helper(WiringSchematic schematic) { }

    // private static int NumPressesNeededP2(WiringSchematic schematic)
    // {
    //     Console.WriteLine("Doing");
    //     Dictionary<string, int> minPresses = [];
    //     HashSet<string> done = [];
    //     minPresses.Add(schematic.CalcJoltagesString(), 0);
    //     Queue<WiringSchematic> frontier = [];
    //     frontier.Enqueue(schematic);
    //     while (true)
    //     {
    //         var next = frontier.Dequeue();
    //         if (next.IsDoneP2())
    //             return minPresses[next.CalcJoltagesGoalString()];
    //         var nextString = next.CalcJoltagesString();
    //         Console.WriteLine(nextString);
    //         if (done.Contains(nextString) || next.IsOverjolted())
    //             continue;
    //         var nextDist = minPresses[nextString] + 1;
    //         for (var i = 0; i < schematic.Buttons.Count; i++)
    //         {
    //             var toAdd = next.PushButton(i);
    //             var toAddString = toAdd.CalcJoltagesString();
    //             if (minPresses.ContainsKey(toAddString))
    //                 continue;
    //             frontier.Enqueue(toAdd);
    //             minPresses[toAdd.CalcJoltagesString()] = nextDist;
    //         }
    //         done.Add(nextString);
    //     }
    // }

    private static WiringSchematic StringToSchematic(string input)
    {
        input = input.Replace("] (", "]|(");
        input = input.Replace(") {", ")|{");
        input = input.Replace(")", "");
        input = input.Replace("(", "");
        input = input.Replace("]", "");
        input = input.Replace("[", "");
        input = input.Replace("{", "");
        input = input.Replace("}", "");
        var toks = input.Split("|");
        List<bool> lightsGoal = toks[0].ToList().ConvertAll(c => c == '#');
        List<List<int>> buttons = toks[1]
            .Split(' ')
            .ToList()
            .ConvertAll(buttonString =>
            {
                return buttonString.Split(',').ToList().ConvertAll(int.Parse);
            });
        List<int> joltagesGoal = toks[2].Split(',').ToList().ConvertAll(int.Parse);
        List<bool> lights = lightsGoal.ConvertAll(_ => false);
        List<int> joltages = joltagesGoal.ConvertAll(_ => 0);
        return new WiringSchematic(lights, lightsGoal, buttons, joltages, joltagesGoal);
    }
}

readonly struct JoltageList
{
    public JoltageList(List<int> joltages)
    {
        Joltages = [.. joltages];
    }

    public List<int> Joltages { get; }

    public override string ToString()
    {
        return $"JoltageList({string.Join(",", Joltages)})";
    }

    public override int GetHashCode()
    {
        var code = new HashCode();
        Joltages.ForEach(joltage => code.Add(joltage));
        return code.ToHashCode();
    }

    public JoltageList PushButton(List<int> button)
    {
        List<int> newJoltages = [.. Joltages];
        button.ForEach(index => newJoltages[index] = newJoltages[index] + 1);
        return new JoltageList(newJoltages);
    }

    public int Magnitude()
    {
        return Joltages.ConvertAll(Math.Abs).Sum();
    }

    public bool IsOverjolted(JoltageList goal)
    {
        for (var i = 0; i < Joltages.Count; i++)
        {
            if (Joltages[i] > goal.Joltages[i])
                return true;
        }
        return false;
    }

    public static bool operator ==(JoltageList l1, JoltageList l2)
    {
        return l1.Joltages.Zip(l2.Joltages).All(pair => pair.First == pair.Second);
    }

    public static bool operator !=(JoltageList l1, JoltageList l2)
    {
        return !(l1 == l2);
    }

    public static JoltageList operator +(JoltageList l1, JoltageList l2)
    {
        return new JoltageList(
            l1.Joltages.Zip(l2.Joltages).ToList().ConvertAll((val) => val.First + val.Second)
        );
    }

    public static JoltageList operator -(JoltageList l)
    {
        return new JoltageList(l.Joltages.ConvertAll(val => -val));
    }

    public static JoltageList operator -(JoltageList l1, JoltageList l2)
    {
        return l1 + -l2;
    }
}

readonly struct WiringSchematic
{
    public WiringSchematic(
        List<bool> lights,
        List<bool> lightsGoal,
        List<List<int>> buttons,
        List<int> joltages,
        List<int> joltagesGoal
    )
    {
        Lights = lights;
        LightsGoal = lightsGoal;
        buttons.Sort((b1, b2) => b2.Count - b1.Count);
        Buttons = buttons;
        Joltages = joltages;
        JoltagesGoal = joltagesGoal;
    }

    public List<bool> Lights { get; }
    public List<bool> LightsGoal { get; }
    public List<List<int>> Buttons { get; }
    public List<int> Joltages { get; }
    public List<int> JoltagesGoal { get; }

    public bool IsDoneP1()
    {
        for (var i = 0; i < Lights.Count; i++)
        {
            if (Lights[i] != LightsGoal[i])
                return false;
        }
        return true;
    }

    public bool IsDoneP2()
    {
        for (var i = 0; i < Joltages.Count; i++)
        {
            if (Joltages[i] != JoltagesGoal[i])
                return false;
        }
        return true;
    }

    public bool IsOverjolted()
    {
        for (var i = 0; i < Joltages.Count; i++)
        {
            if (Joltages[i] > JoltagesGoal[i])
                return true;
        }
        return false;
    }

    public WiringSchematic PushButton(int buttonIndex)
    {
        List<bool> newLights = [.. Lights];
        Buttons[buttonIndex].ForEach(index => newLights[index] = !newLights[index]);
        List<int> newJoltages = [.. Joltages];
        Buttons[buttonIndex].ForEach(index => newJoltages[index] = newJoltages[index] + 1);
        return new WiringSchematic(newLights, LightsGoal, Buttons, newJoltages, JoltagesGoal);
    }

    public string CalcLightsString()
    {
        return CalcAnyLightsString(Lights);
    }

    public string CalcLightsGoalString()
    {
        return CalcAnyLightsString(LightsGoal);
    }

    private static string CalcAnyLightsString(List<bool> lights)
    {
        return string.Join("", lights.ConvertAll(light => light ? '#' : '.'));
    }

    public string CalcJoltagesString()
    {
        return CalcAnyJoltagesString(Joltages);
    }

    public string CalcJoltagesGoalString()
    {
        return CalcAnyJoltagesString(JoltagesGoal);
    }

    private static string CalcAnyJoltagesString(List<int> joltages)
    {
        return string.Join(",", joltages);
    }

    public override string ToString()
    {
        Func<List<bool>, string> calcLights = lights =>
            string.Join("", lights.ConvertAll(light => light ? '#' : '.'));
        return $"WiringSchematic current lights: {calcLights(Lights)}, goal lights: {calcLights(LightsGoal)}, buttons: {string.Join(" ", Buttons.ConvertAll(button => "(" + string.Join(",", button) + ")"))}, joltages: {{{string.Join(",", Joltages)}}}";
    }
}
