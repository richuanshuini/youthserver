using System.Text.Json.Serialization;

namespace YouthApartmentServer.ModelDto.BaseRequest;

public class TinyMceUploadResult
{
    // TinyMCE 需要返回 { location: "..." }
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;
}