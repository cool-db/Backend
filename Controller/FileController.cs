using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using Backend.Biz;

namespace Backend.Controller
{
    public class FileController : ApiController
    {
        [HttpGet]
        public object GetFile(int FileId)
        {
            var file = FileBiz.GetFile(FileId);
            var response = Request.CreateResponse();
            response.Content = new ByteArrayContent(file.Content);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            };
//            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.Type);

            return response;
        }

        [HttpPost]
        public object PostFile()
        {
            HttpContext.Current.Request.ContentEncoding=Encoding.UTF8;
            return FileBiz.PostFile(HttpContext.Current.Request["json"], HttpContext.Current.Request.Files[0]);
        }
        
        [HttpDelete]
        public object DeleteFile(object json)
        {
            return FileBiz.DeleteFile(json);
        }
        
        [HttpPut]
        public object RenameFile(object json)
        {
            return FileBiz.RenameFile(json);
        }
        
        [HttpGet]
        [Route("api/file/list")]
        public object GetList(int taskId)
        {
            return FileBiz.GetList(taskId);
        }
    }
}