using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class LightXController : ControllerBase
{
    private const string ApiUrl = "https://api.lightxeditor.com/external/api/v1/image2image";
    private const string StatusUrl = "https://api.lightxeditor.com/external/api/v1/order-status";
    private const string ApiKey = "4640214a6118489ab90bbdccee92614f_417e3f484c5d46ebac4d742fd3db1af8_andoraitools";

    [HttpPost("generate-image")]
    public async Task<IActionResult> GenerateImage([FromBody] LightXRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.ImageUrl))
        {
            return BadRequest("Eksik ya da hatalı veri gönderildi.");
        }

        // Sabit değerler
        request.TextPrompt = "set hairstyle for this man";
        request.Strength = 0.5;
        request.StyleStrength = 0.9;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-api-key", ApiKey);

        var payload = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        });
        Console.WriteLine($"Gönderilen JSON: {payload}");

        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(ApiUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Hatası: {errorMessage}");
            return StatusCode((int)response.StatusCode, errorMessage);
        }

        var createResponse = JsonConvert.DeserializeObject<LightXResponse>(await response.Content.ReadAsStringAsync());

        // Status kontrolü
        var statusPayload = new { orderId = createResponse.Body.OrderId };
        var statusContent = new StringContent(JsonConvert.SerializeObject(statusPayload), Encoding.UTF8, "application/json");

        for (int i = 0; i < 5; i++)
        {
            await Task.Delay(3000);
            var statusResponse = await client.PostAsync(StatusUrl, statusContent);
            if (!statusResponse.IsSuccessStatusCode)
            {
                var statusError = await statusResponse.Content.ReadAsStringAsync();
                return StatusCode((int)statusResponse.StatusCode, statusError);
            }

            var statusResult = JsonConvert.DeserializeObject<LightXStatusResponse>(await statusResponse.Content.ReadAsStringAsync());
            if (statusResult.Body.Status == "active")
            {
                return Ok(statusResult.Body.Output);
            }
        }

        return BadRequest("Görsel oluşturulamadı.");
    }


}

public class LightXRequest
{
    public string ImageUrl { get; set; }
    public double Strength { get; set; }
    public string TextPrompt { get; set; }
    public string StyleImageUrl { get; set; }
    public double StyleStrength { get; set; }
}

public class LightXResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public LightXBody Body { get; set; }
}

public class LightXBody
{
    public string OrderId { get; set; }
    public int MaxRetriesAllowed { get; set; }
    public int AvgResponseTimeInSec { get; set; }
    public string Status { get; set; }
}

public class LightXStatusResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public LightXStatusBody Body { get; set; }
}

public class LightXStatusBody
{
    public string OrderId { get; set; }
    public string Status { get; set; }
    public string Output { get; set; }
}
