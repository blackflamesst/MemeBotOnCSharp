using Microsoft.AspNetCore.Identity.Data;

public class ImgflipResponse
{
    public bool Success { get; set; }
    public ImgflipData? Data { get; set; }
}

public class ImgflipData
{
    public string? Url { get; set; }
    public string? PageUrl { get; set; }
}
