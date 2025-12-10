using System.Runtime.CompilerServices;

class Util
{
    public static List<string> FileToArray(string file)
    {
        return FileToUntrimmedArray(file).ConvertAll((line) => line.Trim());
    }

    public static List<string> FileToUntrimmedArray(string file)
    {
        // Open the text file using a stream reader.
        using StreamReader reader = new(file);

        // Read the stream as a string.
        string text = reader.ReadToEnd();
        return [.. text.Split("\n")];
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

    public static long LongSquare(long l)
    {
        return l * l;
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

    public bool Intersects(InclusiveRange other)
    {
        return Contains(other.Start) || Contains(other.End) || other.Contains(Start);
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

readonly struct Point3D(long x, long y, long z)
{
    public long X { get; } = x;
    public long Y { get; } = y;
    public long Z { get; } = z;

    public bool Equals(Point3D other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override string ToString()
    {
        return $"Point3D({X},{Y},{Z})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public long DistSquared(Point3D other)
    {
        return Util.LongSquare(X - other.X)
            + Util.LongSquare(Y - other.Y)
            + Util.LongSquare(Z - other.Z);
    }
}

readonly struct Point2D(long x, long y)
{
    public long X { get; } = x;
    public long Y { get; } = y;

    public bool Equals(Point3D other)
    {
        return X == other.X && Y == other.Y;
    }

    public override string ToString()
    {
        return $"Point2D({X},{Y})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public long DistSquared(Point2D other)
    {
        return Util.LongSquare(X - other.X) + Util.LongSquare(Y - other.Y);
    }

    public static Point2D operator +(Point2D l1, Point2D l2)
    {
        return new Point2D(l1.X + l2.X, l1.Y + l2.Y);
    }

    public static Point2D operator -(Point2D l)
    {
        return new Point2D(-l.X, -l.Y);
    }

    public static Point2D operator -(Point2D l1, Point2D l2)
    {
        return l1 + -l2;
    }

    public static Point2D operator *(Point2D l, long scalar)
    {
        return new Point2D(l.X * scalar, l.Y * scalar);
    }
}
