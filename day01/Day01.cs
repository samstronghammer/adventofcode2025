class Day01
{
    public static void Run()
    {
        var input = Util.fileToArray("day01/in.txt");
        var currValue1 = 50;
        var currValue2 = 50;
        var zeroCount1 = 0;
        var zeroCount2 = 0;
        input.ForEach(
            (line) =>
            {
                var number = int.Parse(line[1..]);
                var direction = line[0] == 'R' ? 1 : -1;
                // Part 1
                currValue1 += direction * number;
                if (currValue1 % 100 == 0)
                    zeroCount1++;
                // Part 2. Didn't feel like doing something more mathy here.
                zeroCount2 += number / 100;
                for (var i = 0; i < number % 100; i++)
                {
                    currValue2 += direction;
                    if (currValue2 % 100 == 0)
                        zeroCount2++;
                }
            }
        );
        Console.WriteLine($"Part 1: {zeroCount1}");
        Console.WriteLine($"Part 2: {zeroCount2}");
    }
}
