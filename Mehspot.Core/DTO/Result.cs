using Mehspot.Core.Dto;

namespace Mehspot.Core.DTO
{
    public class Result
    {
        public string ErrorMessage { get; set; }
        public ModelStateDto ModelState { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class Result<T>:Result
    {
        public T Data { get; set; }
    }
}