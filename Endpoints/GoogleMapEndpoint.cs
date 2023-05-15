using DuyProject.API.Services;
using Newtonsoft.Json;

namespace DuyProject.API.Endpoints
{
	public static class GoogleMapEndpoint
    {
		public static void Map(WebApplication app)
        {
            app.MapGet("api/distance/{long1}/{lat1}/{long2}/{lat2}", (GoogleMapService googleMap, double? long1, double? lat1, double? long2, double? lat2) =>
            {
                var result = googleMap.DistanceMatrixUsingCoordinates(new double?[2] { long1, lat1 }, new double?[2] {long2, lat2});
                return Results.Ok(JsonConvert.SerializeObject(result));
            })
           .AllowAnonymous()
           .WithName("GET_DistanceCoordinates").WithGroupName("GoogleMap");

            app.MapGet("api/distance/{origin}/{destination}", (GoogleMapService googleMap, string? origin, string? destination) =>
            {
                var result = googleMap.DistanceMatrixUsingAddress(origin, destination);
                return Results.Ok(JsonConvert.SerializeObject(result));
            })
          .AllowAnonymous()
          .WithName("GET_DistanceAddress").WithGroupName("GoogleMap");
        }
    }
}
