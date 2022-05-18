using InfoLog;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public static class DataRequestExtensions
    {
        public static DataRequestWithLogger CastToDatabaseRequestWithLogger
            (this IRepository repository, ILogger logger = null)
        {
            return new DataRequestWithLogger
            {
                Context = repository.Context,
                Logger = logger
            };
        }

        public static DataRequest CastToDatabaseRequest
            (this IRepository repository)
        {
            return new DataRequest { Context = repository.Context };
        }
    }
}