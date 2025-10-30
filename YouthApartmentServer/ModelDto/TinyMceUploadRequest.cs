using Microsoft.AspNetCore.Http;

namespace YouthApartmentServer.ModelDto;

// 用于 multipart/form-data 的文件上传表单
public class TinyMceUploadRequest
{
    // TinyMCE 默认使用表单字段名 "file"
    public IFormFile? File { get; set; }
}