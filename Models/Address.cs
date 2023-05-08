using DuyProject.API.Helpers;

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public void Update(Address newAddress)
    {
        Street = Street.GetValue(newAddress.Street);
        City = City.GetValue(newAddress.City);
        Country = Country.GetValue(newAddress.Country);
        State = State.GetValue(newAddress.State);
        ZipCode =  ZipCode.GetValue(newAddress.ZipCode);
    }
}