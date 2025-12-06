class Util
{
    public static List<string> FileToArray(string file)
    {
        // Open the text file using a stream reader.
        using StreamReader reader = new(file);

        // Read the stream as a string.
        string text = reader.ReadToEnd();
        return text.Split("\n").ToList().ConvertAll((line) => line.Trim());
    }

    public static Dictionary<Loc, char> ListToLocations(List<string> input)
    {
        var locations = new Dictionary<Loc, char>();
        for (var row = 0; row < input.Count; row++)
        {
            var line = input[row];
            for (var col = 0; col < line.Length; col++)
            {
                var character = line[col];
                locations.Add(new Loc(row, col), character);
            }
        }
        return locations;
    }
}

readonly struct InclusiveRange(long start, long end)
{
    public long Start { get; } = start;
    public long End { get; } = end;

    public bool Contains(long num)
    {
        return num >= Start && num <= End;
    }

    public bool CanMerge(InclusiveRange other)
    {
        return Contains(other.Start)
            || Contains(other.End)
            || other.End == Start - 1
            || other.Start == End + 1
            || other.Contains(Start);
    }

    public InclusiveRange Merge(InclusiveRange other)
    {
        return new InclusiveRange(long.Min(Start, other.Start), long.Max(End, other.End));
    }

    public long Size()
    {
        return End - Start + 1;
    }
}

readonly struct Loc(int row, int col)
{
    public static readonly Loc UP = new(-1, 0);
    public static readonly Loc DOWN = new(1, 0);
    public static readonly Loc LEFT = new(0, -1);
    public static readonly Loc RIGHT = new(0, 1);

    public int Row { get; } = row;
    public int Col { get; } = col;

    public List<Loc> Adj4()
    {
        return [this + UP, this + LEFT, this + RIGHT, this + DOWN];
    }

    public List<Loc> Adj8()
    {
        return
        [
            this + UP + LEFT,
            this + UP,
            this + UP + RIGHT,
            this + LEFT,
            this + RIGHT,
            this + DOWN + LEFT,
            this + DOWN,
            this + DOWN + RIGHT,
        ];
    }

    public bool Equals(Loc other)
    {
        return Row == other.Row && Col == other.Col;
    }

    public override string ToString()
    {
        return $"Loc({Row},{Col})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Col);
    }

    public static Loc operator +(Loc l1, Loc l2)
    {
        return new Loc(l1.Row + l2.Row, l1.Col + l2.Col);
    }

    public static Loc operator -(Loc l)
    {
        return new Loc(-l.Row, -l.Col);
    }

    public static Loc operator -(Loc l1, Loc l2)
    {
        return l1 + -l2;
    }

    public static Loc operator *(Loc l, int scalar)
    {
        return new Loc(l.Row * scalar, l.Col * scalar);
    }
}
