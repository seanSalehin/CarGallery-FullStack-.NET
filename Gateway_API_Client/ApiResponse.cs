namespace Gateway_API_Client
{
    public class ApiResponse<TData>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public TData? Data { get; set; } //return TData => object holder
        public object? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<TData> Create(bool success, int statusCode, string message,  TData? data=default, object? errors=null)
        {
            return new ApiResponse<TData>
            {
                Success = success,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = errors,
            };
        }


        //helper functions
        public static ApiResponse<TData> NotFound(string message = "Resource Not Found") => Create(false, 404, message);
        public static ApiResponse<TData> NoContent(string message = "Operation completed successfully") => Create(true, 204, message);
        public static ApiResponse<TData> BadRequest(string message, object? errors=null) => Create(false, 404, message, errors:errors); //errors:errors because our next parameters in create is data and we don ot have data and wanna move to error and both are object so  to dotnet do not make a mistake we do this
        public static ApiResponse<TData> Conflict(string message) => Create(false, 409, message);
        public static ApiResponse<TData> Error(int StatusCode, string message, object? errors=null) => Create(false, StatusCode, message, errors:errors);
        public static ApiResponse<TData> Ok(TData data, string message) => Create(true, 200, message, data);
        public static ApiResponse<TData> CreatedAt(TData data, string message) => Create(true, 201, message, data);


    }
}
