using MathNet.Numerics.LinearAlgebra;

class Day10
{
    public static void Run()
    {
        var input = Util.FileToArray("day10/in.txt");
        List<WiringSchematic> schematics = input.ConvertAll(StringToSchematic);

        Console.WriteLine($"Part 1: {schematics.ConvertAll(NumPressesNeeded).Sum()}");
        Console.WriteLine($"Part 2: {schematics.ConvertAll(NumPressesNeededP2Fast).Sum()}");
    }

    /**
     * This solution is based off of a divide and conquer algorithm that was not my initial intuiton. Runs in about 17 seconds.
     */
    private static double NumPressesNeededP2Fast(WiringSchematic schematic)
    {
        var buttonMatrix = Matrix<double>.Build.Dense(
            schematic.Buttons.Count,
            schematic.Buttons[0].Count,
            (i, j) => schematic.Buttons[i][j]
        );
        // Map from curr location mod 2 to (cost, vec to add)
        Dictionary<Vector<double>, List<(double, Vector<double>)>> optionsMemo = [];
        var result = P2FastHelper(buttonMatrix, schematic.JoltagesGoal, optionsMemo);
        return result;
    }

    private static List<(double, Vector<double>)> GetOptions(
        Matrix<double> buttonMatrix,
        Vector<double> goal,
        Dictionary<Vector<double>, List<(double, Vector<double>)>> optionsMemo
    )
    {
        var mod2Goal = goal.Modulus(2);
        optionsMemo.TryGetValue(mod2Goal, out var result);
        if (result != null)
            return result;

        Vector<double> emptyButtonPresses = CreateVector.Dense<double>(buttonMatrix.RowCount);
        List<Vector<double>> buttonPushOptions = [emptyButtonPresses];
        for (var i = 0; i < buttonMatrix.RowCount; i++)
        {
            var oldOptions = buttonPushOptions.ConvertAll(option => option.Clone());
            oldOptions.ForEach(option =>
            {
                option[i] = 1;
                buttonPushOptions.Add(option);
            });
        }
        var successes = buttonPushOptions
            .FindAll(
                (option) =>
                {
                    var result = option.ToRowMatrix().Multiply(buttonMatrix).Row(0).Modulus(2);
                    return mod2Goal.Equals(result);
                }
            )
            .ConvertAll(pushOption =>
            {
                return (
                    pushOption.SumMagnitudes(),
                    pushOption.ToRowMatrix().Multiply(buttonMatrix).Row(0)
                );
            });
        optionsMemo.Add(mod2Goal, successes);
        return successes;
    }

    private static double P2FastHelper(
        Matrix<double> buttonMatrix,
        Vector<double> goal,
        Dictionary<Vector<double>, List<(double, Vector<double>)>> optionsMemo
    )
    {
        if (goal.SumMagnitudes() == 0)
            return 0;
        var successes = GetOptions(buttonMatrix, goal, optionsMemo);
        var bestPresses = double.PositiveInfinity;
        successes.ForEach(pair =>
        {
            var result = pair.Item2;
            var remaining = goal.Subtract(result).Divide(2);
            if (remaining.Any(d => d < 0))
                return;
            var totalPresses = pair.Item1 + 2 * P2FastHelper(buttonMatrix, remaining, optionsMemo);
            if (totalPresses < bestPresses)
                bestPresses = totalPresses;
        });
        return bestPresses;
    }

    /**
     * This was my original solution. It takes about a day to run (single-threaded). It does constraint solving with
     * every pruning method that came to mind. The smaller cases finish quickly, but about 15 take many minutes to hours
     * to run.
     */
    private static double NumPressesNeededP2Slow(WiringSchematic schematic)
    {
        var buttonMatrix = Matrix<double>.Build.Dense(
            schematic.Buttons.Count,
            schematic.Buttons[0].Count,
            (i, j) => schematic.Buttons[i][j]
        );
        // 0 means no constraint, 1 means max, 2 means equal
        var constraintMatrix = Matrix<double>.Build.Dense(
            schematic.Buttons.Count,
            schematic.Buttons[0].Count,
            (i, j) =>
            {
                var prev = buttonMatrix.At(i, j);
                if (prev == 0)
                    return 0;
                // Calculate all numbers below me
                var belowMeMatrix = buttonMatrix.SubMatrix(
                    i + 1,
                    buttonMatrix.RowCount - i - 1,
                    j,
                    1
                );
                if (belowMeMatrix.RowSums().Sum() == 0)
                {
                    return 2;
                }
                return 1;
            }
        );

        var calculatedPresses = MinP2SlowHelper(
            0,
            schematic.Joltages,
            buttonMatrix,
            constraintMatrix,
            schematic.JoltagesGoal,
            Math.Ceiling(
                schematic.JoltagesGoal.SumMagnitudes() / schematic.Buttons.Last().SumMagnitudes()
            )
        );
        if (calculatedPresses == null)
        {
            throw new Exception("Failed to find solution");
        }
        return calculatedPresses.Sum();
    }

    private static Vector<double>? MinP2SlowHelper(
        int index,
        Vector<double> currVector,
        Matrix<double> buttonMatrix,
        Matrix<double> constraintMatrix,
        Vector<double> goal,
        double upperbound
    )
    {
        if (upperbound < 0)
        {
            return null;
        }

        Vector<double> pressMeVector = CreateVector.Dense<double>(buttonMatrix.RowCount);

        if (index >= buttonMatrix.RowCount)
        {
            return currVector.Equals(goal) ? pressMeVector : null;
        }
        var button = buttonMatrix.Row(index);
        var constraints = constraintMatrix.Row(index);
        var remaining = goal - currVector;
        var remainingMagnitude = remaining.SumMagnitudes();
        var buttonMagnitude = button.SumMagnitudes();
        var minPressesRequired = remainingMagnitude / buttonMagnitude;
        if (minPressesRequired > upperbound)
        {
            return null;
        }
        double? equals = null;
        double max = upperbound;
        for (var i = 0; i < constraints.Count; i++)
        {
            var constraint = constraints[i];
            if (constraint == 0)
                continue;
            if (constraint == 1)
                max = double.Min(max, remaining[i]);
            if (constraint == 2)
            {
                if (equals != null && equals != remaining[i])
                    return null; // Can't equal two different things!
                equals = remaining[i];
            }
        }
        if (equals != null && equals > max)
            return null; // Can't be equal to something and greater than max
        double minCoeff = equals ?? 0;
        double maxCoeff = equals ?? max;
        pressMeVector[index] = 1;
        Vector<double>? bestAns = null;
        for (var coeff = maxCoeff; coeff >= minCoeff; coeff--)
        {
            var newVector = button.Multiply(coeff) + currVector;
            var newUpperbound = upperbound - coeff;
            var recursiveAns = MinP2SlowHelper(
                index + 1,
                newVector,
                buttonMatrix,
                constraintMatrix,
                goal,
                newUpperbound
            );

            if (recursiveAns != null)
            {
                var pressesWithMe = pressMeVector.Multiply(coeff) + recursiveAns;
                var numPressesWithMe = pressesWithMe.SumMagnitudes();
                if (bestAns == null || bestAns.SumMagnitudes() > numPressesWithMe)
                {
                    upperbound = Math.Min(numPressesWithMe, upperbound);
                    bestAns = pressesWithMe;
                }
            }
        }

        return bestAns;
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
        Vector<double> lightsGoal = CreateVector.Dense<double>([
            .. toks[0].ToList().ConvertAll(c => c == '#' ? 1.0 : 0.0),
        ]);
        List<Vector<double>> buttons = toks[1]
            .Split(' ')
            .ToList()
            .ConvertAll(buttonString =>
            {
                var buttonVec = lightsGoal.Multiply(0);
                buttonString
                    .Split(',')
                    .ToList()
                    .ConvertAll(int.Parse)
                    .ForEach(index => buttonVec[index] = 1);
                return buttonVec;
            });
        Vector<double> joltagesGoal = CreateVector.Dense<double>([
            .. toks[2].Split(',').ToList().ConvertAll(double.Parse),
        ]);
        Vector<double> lights = lightsGoal.Multiply(0);
        Vector<double> joltages = joltagesGoal.Multiply(0);
        return new WiringSchematic(
            lights,
            lightsGoal,
            buttons,
            joltages,
            joltagesGoal,
            CreateVector.Dense<double>([.. buttons.ConvertAll(btn => 0)])
        );
    }
}

readonly struct WiringSchematic
{
    public WiringSchematic(
        Vector<double> lights,
        Vector<double> lightsGoal,
        List<Vector<double>> buttons,
        Vector<double> joltages,
        Vector<double> joltagesGoal,
        Vector<double> buttonPushes
    )
    {
        Lights = lights;
        LightsGoal = lightsGoal;
        buttons.Sort((b1, b2) => Convert.ToInt32(b2.Sum() - b1.Sum())); // Bigger buttons better to use first
        Buttons = buttons;
        Joltages = joltages;
        JoltagesGoal = joltagesGoal;
        ButtonPushes = buttonPushes;
    }

    public Vector<double> Lights { get; }
    public Vector<double> LightsGoal { get; }
    public List<Vector<double>> Buttons { get; }
    public Vector<double> Joltages { get; }
    public Vector<double> JoltagesGoal { get; }
    public Vector<double> ButtonPushes { get; }

    public bool IsDoneP1()
    {
        return Lights.Modulus(2).Equals(LightsGoal.Modulus(2));
    }

    public bool IsDoneP2()
    {
        return Joltages.Equals(JoltagesGoal);
    }

    public bool IsOverjolted()
    {
        return (JoltagesGoal - Joltages).Minimum() < 0;
    }

    public WiringSchematic PushButton(int buttonIndex)
    {
        var newLights = Lights.Add(Buttons[buttonIndex]);
        var newJoltages = Joltages.Add(Buttons[buttonIndex]);
        var newButtonPushes = ButtonPushes.Clone();
        newButtonPushes[buttonIndex]++;
        return new WiringSchematic(
            newLights,
            LightsGoal,
            Buttons,
            newJoltages,
            JoltagesGoal,
            newButtonPushes
        );
    }

    public string CalcLightsString()
    {
        return CalcAnyLightsString(Lights);
    }

    public string CalcLightsGoalString()
    {
        return CalcAnyLightsString(LightsGoal);
    }

    private static string CalcAnyLightsString(Vector<double> lights)
    {
        return string.Join(
            "",
            Array.ConvertAll(lights.AsArray(), light => light % 2 == 1 ? '#' : '.')
        );
    }

    public string CalcJoltagesString()
    {
        return CalcAnyJoltagesString(Joltages);
    }

    public string CalcJoltagesGoalString()
    {
        return CalcAnyJoltagesString(JoltagesGoal);
    }

    private static string CalcAnyJoltagesString(Vector<double> joltages)
    {
        return string.Join(",", joltages);
    }

    public override string ToString()
    {
        Func<Vector<double>, string> calcLights = lights =>
            string.Join(
                "",
                Array.ConvertAll(lights.AsArray(), light => light % 2 == 1 ? '#' : '.')
            );
        return $"WiringSchematic current lights: {calcLights(Lights)}, goal lights: {calcLights(LightsGoal)}, buttons: {string.Join(" ", Buttons.ConvertAll(button => "(" + string.Join(",", button) + ")"))}, joltages: {{{string.Join(",", Joltages)}}}";
    }
}
