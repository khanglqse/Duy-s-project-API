using DuyProject.API.Configurations;
using DuyProject.API.ViewModels;
using GoogleApi;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.DistanceMatrix.Request;
using GoogleApi.Entities.Maps.DistanceMatrix.Response;
using Microsoft.Extensions.Options;

namespace DuyProject.API.Services
{
    public class GoogleMapService
    {
        private readonly GoogleSettings _googleSettings;
        public GoogleMapService(IOptions<GoogleSettings> googleSettings)
        {
            _googleSettings = googleSettings.Value;
        }

        public ServiceResult<DistanceMatrixResponse> DistanceMatrixUsingAddress(string? origin, string? destination)
        {
            //var origin = new GoogleApi.Entities.Common.Address("huyện Thạch Hà, Hà Tĩnh, Việt Nam");
            //var destination = new GoogleApi.Entities.Common.Address("03 Huỳnh Thúc Kháng, Phường 4, Lâm Đồng");

            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
            {
                return new ServiceResult<DistanceMatrixResponse>("Address does not exit");
            }

            var request = new DistanceMatrixRequest
            {
                Key = _googleSettings.MapApiKey,
                Origins = new[]
                {
                    new LocationEx(new GoogleApi.Entities.Common.Address(origin))
                },
                Destinations = new[]
                {
                    new LocationEx(new GoogleApi.Entities.Common.Address(destination))
                }
            };

            var result = GoogleMaps.DistanceMatrix.Query(request);
            var convert = new ServiceResult<DistanceMatrixResponse>(result);

            return convert;
        }

        public ServiceResult<DistanceMatrixResponse> DistanceMatrixUsingCoordinates(double?[] coordinate1, double?[] coordinate2)
		{
            var lon1 = coordinate1[0];
            var lon2 = coordinate2[0];
            var lat1 = coordinate1[1];
            var lat2 = coordinate2[1];

            if (IsDoubleNull(lon1) || IsDoubleNull(lon2) || IsDoubleNull(lat1) || IsDoubleNull(lat2))
            {
                return new ServiceResult<DistanceMatrixResponse>("Location is not exist");
            }

            var request = new DistanceMatrixRequest
            {
                Key = _googleSettings.MapApiKey,
                Origins = new[]
                {
                    new LocationEx(new CoordinateEx(GetDouble(lat1), GetDouble(lon1)))
                },
                Destinations = new[]
                {
                    new LocationEx(new CoordinateEx(GetDouble(lat2), GetDouble(lon2)))
                }
            };

            var result = GoogleMaps.DistanceMatrix.Query(request);
            var convert = new ServiceResult<DistanceMatrixResponse>(result);

            return convert;
        }

        private bool IsDoubleNull(double? value)
        {
            return value == null;
        }

        private double GetDouble(double? value)
        {
            return value ?? 0;
        }
    }
}

