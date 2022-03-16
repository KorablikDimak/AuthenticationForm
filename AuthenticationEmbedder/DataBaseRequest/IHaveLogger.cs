using InfoLog;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public interface IHaveLogger
    {
        public ILogger Logger { get; set; }
    }
}