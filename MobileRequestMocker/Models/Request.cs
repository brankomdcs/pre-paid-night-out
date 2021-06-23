using MobileRequestMocker.Requests;

namespace MobileRequestMocker.Models
{
    public class Request
    {
        public Request(RequestType type, string url)
        {
            Type = type;
            Url = url;
        }
        public RequestType Type { get; }

        public string Url { get; }
    }
}
