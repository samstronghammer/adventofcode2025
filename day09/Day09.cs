class Day09
{
    public static void Run()
    {
        var input = Util.FileToArray("day09/in.txt");
        var locs = input.ConvertAll(l =>
        {
            var toks = l.Split(',');
            return new Point2D(long.Parse(toks[0]), long.Parse(toks[1]));
        });

        List<LineSegment> lines = [];
        for (var i = 0; i < locs.Count; i++)
        {
            var l1 = locs[i];
            var l2Index = (i + 1) % locs.Count;
            var l2 = locs[l2Index];
            lines.Add(new LineSegment(l1, l2, GoesLeft(locs, i) != GoesLeft(locs, l2Index)));
        }

        long maxY = locs.Aggregate((long)0, (acc, val) => Math.Max(acc, val.Y));
        long maxSize = 0;
        long maxSize2 = 0;

        for (var i = 0; i < locs.Count; i++)
        {
            for (var j = i + 1; j < locs.Count; j++)
            {
                var loc1 = locs[i];
                var loc2 = locs[j];
                var area = (Math.Abs(loc1.X - loc2.X) + 1) * (Math.Abs(loc1.Y - loc2.Y) + 1);
                maxSize = Math.Max(area, maxSize);
                if (area > maxSize2 && IsValid(lines, loc1, loc2, maxY))
                {
                    maxSize2 = area;
                }
            }
        }
        Console.WriteLine($"Part 1: {maxSize}");
        Console.WriteLine($"Part 2: {maxSize2}");
    }

    private static bool GoesLeft(List<Point2D> locs, int index)
    {
        var loc = locs[index];
        var p1 = locs[(index + 1) % locs.Count];
        var p2 = locs[(index - 1 + locs.Count) % locs.Count];
        var offsetPoint = p1.X != loc.X ? p1 : p2;
        if (offsetPoint.X == loc.X)
            throw new Exception("Cols should be different");
        if (offsetPoint.X > loc.X)
            return false;
        return true;
    }

    private static bool IsInside(List<LineSegment> lines, Point2D point, long maxY)
    {
        var pointSegment = new LineSegment(point, point, false);
        if (lines.Any(line => line.Intersects(pointSegment)))
            return true;
        var numCrossings = 0;
        var maxSegment = new LineSegment(point, new Point2D(point.X, maxY), false);
        lines.ForEach(line =>
        {
            if (maxSegment.Intersects(line))
            {
                if (maxSegment.IsVertical != line.IsVertical || line.IsZag)
                    numCrossings++;
            }
        });
        return numCrossings % 2 == 1;
    }

    private static List<Point2D> GetCheckPoints(List<LineSegment> lines, LineSegment line1)
    {
        List<Point2D> intersections = [];
        intersections.Add(line1.Start);
        intersections.Add(line1.End);
        lines.ForEach(line2 =>
        {
            var intersection = line1.IntersectionPoint(line2);
            if (intersection == null)
                return;
            intersections.Add(intersection.Value);
        });
        intersections.Sort(
            (p1, p2) => Math.Sign(p1.DistSquared(line1.Start) - p2.DistSquared(line1.Start))
        );
        List<Point2D> importantPoints = [];
        for (var i = 0; i < intersections.Count - 1; i++)
        {
            var p1 = intersections[i];
            var p2 = intersections[i + 1];
            var midpoint = new Point2D((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            importantPoints.Add(midpoint);
        }
        return importantPoints;
    }

    private static bool IsValid(List<LineSegment> lines, Point2D point1, Point2D point2, long maxY)
    {
        var newYLoc = new Point2D(point1.X, point2.Y);
        var newXLoc = new Point2D(point2.X, point1.Y);
        List<LineSegment> segments =
        [
            new LineSegment(point1, newYLoc, false),
            new LineSegment(point1, newXLoc, false),
            new LineSegment(newYLoc, point2, false),
            new LineSegment(newXLoc, point2, false),
        ];
        for (var i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];
            var importantPoints = GetCheckPoints(lines, segment);
            if (importantPoints.Any(point => !IsInside(lines, point, maxY)))
                return false;
        }
        return true;
    }
}

readonly struct LineSegment(Point2D start, Point2D end, bool isZag)
{
    public Point2D Start { get; } = start;
    public Point2D End { get; } = end;
    public bool IsZag { get; } = isZag;

    public bool IsVertical { get; } = start.X == end.X;
    public InclusiveRange YRange { get; } =
        new InclusiveRange(Math.Min(start.Y, end.Y), Math.Max(start.Y, end.Y));
    public InclusiveRange XRange { get; } =
        new InclusiveRange(Math.Min(start.X, end.X), Math.Max(start.X, end.X));

    public bool Intersects(LineSegment other)
    {
        if (IsVertical == other.IsVertical)
        {
            if (IsVertical && Start.X != other.Start.X)
                return false;
            if (!IsVertical && Start.Y != other.Start.Y)
                return false;
            return IsVertical ? YRange.Intersects(other.YRange) : XRange.Intersects(other.XRange);
        }
        else
        {
            if (IsVertical)
            {
                if (!YRange.Contains(other.Start.Y))
                    return false;
                if (!other.XRange.Contains(Start.X))
                    return false;
                return true;
            }
            else
            {
                if (!XRange.Contains(other.Start.X))
                    return false;
                if (!other.YRange.Contains(Start.Y))
                    return false;
                return true;
            }
        }
    }

    public Point2D? IntersectionPoint(LineSegment other)
    {
        if (!Intersects(other) || IsVertical == other.IsVertical)
            return null;
        if (IsVertical)
        {
            return new Point2D(Start.X, other.Start.Y);
        }
        return new Point2D(other.Start.X, Start.Y);
    }
}
