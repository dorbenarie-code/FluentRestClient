namespace FluentRestClient.Core
{
    public class RestResponse
    {
        public int StatusCode { get; }
        public string Body { get; }
        public bool IsSuccess => StatusCode >= 200 && StatusCode <= 299;

        public RestResponse(int statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body ?? string.Empty;
        }
    }
}