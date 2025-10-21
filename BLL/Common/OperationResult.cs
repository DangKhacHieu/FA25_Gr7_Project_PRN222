using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common
{
    public class OperationResult
    {
        //giúp trả về 1 đoạn string và 1 bool kết quả 
        //sử dụng cho các chức năng có kèm thông báo cho user
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static OperationResult Ok(string message) =>
            new OperationResult { Success = true, Message = message };

        public static OperationResult Fail(string message) =>
            new OperationResult { Success = false, Message = message };
    }
}

