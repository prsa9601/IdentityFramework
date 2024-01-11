using Microsoft.AspNetCore.Mvc;

namespace cleanshop1.Controllers
{
    public class BaseController : ControllerBase
    {
        public IActionResult CustomOk(object data, string message = "")
        {
            return Ok(new Result()
            {
                Message = message,
                Data = data,
                Status = Status.Success
            });
        }
        public IActionResult CustomError(object data, string message = "")
        {
            return BadRequest(new Result()
            {
                Message = message,
                Data = data,
                Status = Status.Success
            });
        }
    }
    public class Result
    {
        public object Data { get; set; }
        public Status Status { get; set; }
        public string Message { get; set; }
    }
    public enum Status
    {
        Success = 1,
        Failed = -1
    }
}
