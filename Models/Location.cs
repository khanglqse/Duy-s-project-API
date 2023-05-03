using DuyProject.API.Helpers;

public class Location {
    public string Type { get; set; } = "Point";
    public double[] Coordinates { get; set; }
    public Address Address { get; set; }

    public void Update(Location newLocation)
    {
        if (newLocation == null)
        {
            return;
        }

        if (Address == null)
        {
            Address = new Address();
        }

        Coordinates = newLocation.Coordinates;
        Address.Update(newLocation.Address);
    }
}