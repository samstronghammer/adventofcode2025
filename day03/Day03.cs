class Day03
{
    public static void Run()
    {
        var input = Util.fileToArray("day03/in.txt");
        var joltageList = input.ConvertAll((line) => CalcJoltage(line, 2));
        var joltageList2 = input.ConvertAll((line) => CalcJoltage(line, 12));
        Console.WriteLine($"Part 1: {joltageList.Sum()}");
        Console.WriteLine($"Part 2: {joltageList2.Sum()}");
    }

    private static long CalcJoltage(string line, int numDigits)
    {
        long total = 0;
        int nextIndex = 0;
        for (int i = 0; i < numDigits; i++)
        {
            var currDigit = 0;
            var currDigitIndex = nextIndex;
            var digitsLeft = numDigits - i - 1;
            for (int j = currDigitIndex; j < line.Length - digitsLeft; j++)
            {
                var digit = int.Parse(line[j].ToString());
                if (digit > currDigit)
                {
                    currDigit = digit;
                    currDigitIndex = j;
                }
            }
            nextIndex = currDigitIndex + 1;
            total *= 10;
            total += currDigit;
        }
        return total;
    }
}
