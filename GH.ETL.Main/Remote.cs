using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    class Remote
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient = new HttpClient();

        public Remote(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Update(ISet<Data> data, DateTime dateTime)
        {
            var endpoint = _configuration.GetSection("Parameter").GetSection("Endpoint").Value;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GH-ETL Agent");

            var request = new DataUpdateRequest();
            request.data = data;
            request.pharmacy.id = _configuration.GetSection("Parameter:Pharmacy:Id").Value;
            request.pharmacy.name = _configuration.GetSection("Parameter:Pharmacy:Name").Value;
            request.timestamp = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            request.hash = Sign(request);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeCustomConverter());
            var content = JsonContent.Create<DataUpdateRequest>(request, options: options);
            var response = await _httpClient.PostAsync(endpoint, content);
        }

        private string Sign(DataUpdateRequest request)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var plaintext = MakePlaintext(request);
                var bytes = Encoding.UTF8.GetBytes(plaintext);
                var outBytes = sha256.ComputeHash(bytes);

                return BitConverter.ToString(outBytes).Replace("-", "");
            }
        }

        private string MakePlaintext(DataUpdateRequest request)
        {
            var data = request.data.ToList();
            data.Sort(Data.ComparisonForSign);

            return
                data.Select(r => string.Format("{0}&{1}&{2}", r.IdNo, r.Birthday, r.CustomerName)).Aggregate((a, b) => a + "&" + b)
                + "&"
                + request.pharmacy.id
                + "&"
                + request.pharmacy.name
                + "&"
                + request.timestamp.ToString();
        }
    }
}
