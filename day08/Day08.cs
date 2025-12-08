using QuikGraph;
using QuikGraph.Algorithms;

class Day08
{
    public static void Run()
    {
        var input = Util.FileToArray("day08/in.txt");
        var graph = new BidirectionalGraph<Point3D, Edge<Point3D>>();
        var numConnections = 1000;
        var points = input.ConvertAll(line =>
        {
            var toks = line.Split(',');
            return new Point3D(long.Parse(toks[0]), long.Parse(toks[1]), long.Parse(toks[2]));
        });
        points.ForEach(p => graph.AddVertex(p));

        List<(Point3D, Point3D, long)> distances = [];
        for (var i = 0; i < points.Count; i++)
        {
            for (var j = i + 1; j < points.Count; j++)
            {
                var p1 = points[i];
                var p2 = points[j];
                distances.Add((p1, p2, p1.DistSquared(p2)));
            }
        }
        distances.Sort(
            (d1, d2) =>
            {
                return d1.Item3.CompareTo(d2.Item3);
            }
        );

        graph.IncrementalConnectedComponents(
            out Func<KeyValuePair<int, IDictionary<Point3D, int>>> getComponents
        );

        for (var i = 0; i < distances.Count; i++)
        {
            if (i == numConnections)
            {
                distances[0..numConnections]
                    .ForEach(d =>
                    {
                        graph.AddEdge(new Edge<Point3D>(d.Item1, d.Item2));
                        graph.AddEdge(new Edge<Point3D>(d.Item2, d.Item1));
                    });
                var currentP1 = getComponents();
                Dictionary<int, List<Point3D>> components = [];
                currentP1
                    .Value.ToList()
                    .ForEach(
                        (pair) =>
                        {
                            var point = pair.Key;
                            var component = pair.Value;
                            if (!components.ContainsKey(component))
                                components[component] = [];
                            components[component].Add(point);
                        }
                    );
                List<int> sizes = components.Keys.ToList().ConvertAll(key => components[key].Count);
                sizes.Sort();
                sizes.Reverse();
                Console.WriteLine($"Part 1: {sizes[0..3].Aggregate(1, (acc, val) => acc * val)}");
            }
            var d = distances[i];
            graph.AddEdge(new Edge<Point3D>(d.Item1, d.Item2));
            var current = getComponents();
            if (current.Key == 1)
            {
                Console.WriteLine($"Part 2: {d.Item1.X * d.Item2.X}");
                break;
            }
        }
    }
}
