public class Location 
{
    public Location(double[] coordinates)
    {
        Coordinates = coordinates;
        Type = "Point";
    }

    public Location(double[] coordinates, string type)
    {
        Coordinates = coordinates;
        Type = type;
    }

    public double[] Coordinates { get; set; }
    public string Type { get; set; } = "Point";
}