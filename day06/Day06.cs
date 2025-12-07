using System.Text.RegularExpressions;

class Day06
{
    public static void Run()
    {
        var input = Util.FileToArray("day06/in.txt");
        var input2 = Util.FileToUntrimmedArray("day06/in.txt");
        var index = 0;
        var op = ' ';
        long sum = 0;
        List<long> numbers = [];
        while (true)
        {
            var s = "";
            var shouldStay = false;
            for (var i = 0; i < input2.Count; i++)
            {
                var line = input2[i];
                shouldStay = shouldStay || line.Length > index;
                var c = line.Length > index ? line[index] : ' ';
                s += c;
            }
            var numberMatch = Regex.Match(s, "\\d+").ToString();
            var opMatch = Regex.Match(s, "\\*|\\+").ToString();
            if (numberMatch == "")
            {
                Console.WriteLine(op);
                Console.WriteLine(string.Join(',', numbers));
                if (op == '+')
                {
                    sum += numbers.Sum();
                }
                else if (op == '*')
                {
                    sum += numbers.Aggregate((long)1, (acc, val) => acc * val);
                }
                else
                {
                    throw new Exception($"Unexpected op '{op}'");
                }
                numbers = [];
                op = ' ';
            }
            else
            {
                numbers.Add(long.Parse(numberMatch));
            }

            if (opMatch != "")
            {
                op = opMatch[0];
            }
            Console.WriteLine(numberMatch);
            Console.WriteLine(op);
            if (!shouldStay)
                break;
            index++;
        }
        Console.WriteLine($"Part 1: {CalcEquations(input)}");
        Console.WriteLine($"Part 2: {sum}");
    }

    private static long CalcEquations(List<string> input)
    {
        List<List<long>> equations = [];
        List<char> operations = [];
        input.ForEach(line =>
        {
            var toks = Regex.Split(line.Trim(), "\\s+").ToList();
            if (toks[0] == "*" || toks[0] == "+")
            {
                operations = toks.ConvertAll(tok => tok[0]);
            }
            else
            {
                equations.Add(toks.ConvertAll(tok => long.Parse(tok)));
            }
        });
        long sum = 0;
        for (var i = 0; i < operations.Count; i++)
        {
            var op = operations[i];
            var operands = equations.ConvertAll(list => list[i]);
            if (op == '+')
            {
                sum += operands.Sum();
            }
            else
            {
                sum += operands.Aggregate((long)1, (acc, val) => acc * val);
            }
        }
        return sum;
    }
}
