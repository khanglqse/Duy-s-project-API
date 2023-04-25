namespace DuyProject.API.ViewModels
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; }

        public ServiceResult(T data)
        {
            Success = true;
            Data = data;
            Message = "";
        }

        public ServiceResult(string message)
        {
            Success = false;
            Message = message;
            Data = default;
        }
    }
}
