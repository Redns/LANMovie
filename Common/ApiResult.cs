namespace LANMovie.Common
{
    public class ApiResult<T>
    {
        public ApiResultCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public ApiResult(ApiResultCode statusCode, string message, T result)
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
        }
    }

    public enum ApiResultCode
    {
        Success = 0,            // 正常处理
        ServerError,            // 服务器出错
        AccessDenied,           // 无权限
        ResourceNotFound,       // 资源未找到
    }
}
