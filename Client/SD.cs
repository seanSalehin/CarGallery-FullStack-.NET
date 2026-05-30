namespace Client
{
    public static class SD
    {
        //static details => for APIRequest
        public enum APIType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public const string SessionToken = "JWTToken";
        public const string CurrentApiVersion = "v2";
    }
}
