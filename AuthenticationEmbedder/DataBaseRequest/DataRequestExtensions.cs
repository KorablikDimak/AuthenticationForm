using InfoLog;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public static class DataRequestExtensions
    {
        public static DatabaseRequestWithLogger CastToDatabaseRequestWithLogger
            (this IDatabaseRequest databaseRequest, ILogger logger = null)
        {
            return new DatabaseRequestWithLogger
            {
                Context = databaseRequest.Context,
                Logger = logger
            };
        }

        public static DatabaseRequest CastToDatabaseRequest
            (this IDatabaseRequest databaseRequest)
        {
            return new DatabaseRequest { Context = databaseRequest.Context };
        }
    }
}