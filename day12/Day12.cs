
class Day12
{
  public static void Run()
  {
    var input = Util.FileToArray("day12/in.txt");
    // 30 lines of presents. each is id, loc loc loc, empty
    List<List<Present>> presents = Enumerable.Range(0, 6).ToList().ConvertAll(i =>
    {
      var lines = input[(i * 5 + 1)..(i * 5 + 4)];
      HashSet<Loc> occupied = [];
      for (var row = 0; row < 3; row++)
      {
        for (var col = 0; col < 3; col++)
        {
          var c = lines[row][col];
          if (c == '#') occupied.Add(new Loc(row, col));
        }
      }
      var present = new Present(occupied);
      var present2 = present.Flip();
      List<Present> presents = [];
      for (var rotations = 0; rotations < 4; rotations++)
      {
        if (!presents.Any(other => other.Occupied.SetEquals(present.Occupied)))
        {
          presents.Add(present);
        }
        if (!presents.Any(other => other.Occupied.SetEquals(present2.Occupied)))
        {
          presents.Add(present2);
        }
        present = present.Rotate();
        present2 = present2.Rotate();
      }
      return presents;
    });

    List<PresentArrangement> arrangements = input[30..].ConvertAll(line =>
    {
      line = line.Replace("x", " ");
      line = line.Replace(": ", " ");
      var numbers = line.Split(" ").ToList().ConvertAll(int.Parse);
      return new PresentArrangement(numbers[0], numbers[1], numbers[2..], []);
    });

    var count = arrangements.Where(arrangement =>
    {
      return FillPercentRequired(arrangement, presents) < 0.9;
    }).Count();
    Console.WriteLine($"Part 1: {count}");
  }

  public static double FillPercentRequired(PresentArrangement arrangement, List<List<Present>> presents)
  {
    var presentSizes = presents.ConvertAll(list => list[0].Occupied.Count);
    var arrangementSpace = arrangement.Width * arrangement.Height;
    var sizeNeeded = arrangement.Presents.Zip(presentSizes).ToList().ConvertAll((val) => val.First * val.Second).Sum();
    return sizeNeeded / arrangementSpace;
  }
}

// Most of this ended up useless haha.
readonly struct Present(HashSet<Loc> occupied)
{

  public HashSet<Loc> Occupied { get; } = occupied;

  public Present Rotate()
  {
    return new Present([.. Occupied.ToList().ConvertAll((point) =>
    {
      var centralized = point - new Loc(1, 1);
      var rotated = new Loc(centralized.Col, -centralized.Row);
      return rotated + new Loc(1, 1);
    })]);
  }

  public Present Flip()
  {
    return new Present([.. Occupied.ToList().ConvertAll((point) =>
    {
      return new Loc(point.Row, -(point.Col - 1) + 1);
    })]);
  }

}

readonly struct PresentArrangement
{
  public PresentArrangement(int width, int height, List<int> presents, HashSet<Loc> occupied)
  {
    Width = width;
    Height = height;
    Presents = presents;
    Occupied = occupied;
  }

  public int Width { get; }
  public int Height { get; }
  public List<int> Presents { get; }

  public HashSet<Loc> Occupied { get; }

  public PresentArrangement? AddPresent(int index, Present p, Loc offset)
  {
    var newOccupied = new HashSet<Loc>(Occupied);
    foreach (Loc loc in p.Occupied)
    {
      var newLoc = loc + offset;
      if (newOccupied.Contains(newLoc)) return null;
      newOccupied.Add(newLoc);
    }
    var presentsCopy = Presents.ToList();
    presentsCopy[index]--;
    return new PresentArrangement(Width, Height, presentsCopy, newOccupied);
  }
}