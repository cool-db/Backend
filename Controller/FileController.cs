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
        [Route("api/file")]
        public object GetFile(int fileId)
        {
            var file = FileBiz.GetFile(fileId);
            var response = Request.CreateResponse();
            response.Content = new ByteArrayContent(file.Content);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            };

            return response;
        }

        [HttpPost]
        [Route("api/file")]
        public object PostFile()
        {
            HttpContext.Current.Request.ContentEncoding=Encoding.UTF8;
            return FileBiz.PostFile(HttpContext.Current.Request["json"], HttpContext.Current.Request.Files[0]);
        }
        
        [HttpDelete]
        [Route("api/file")]
        public object DeleteFile(object json)
        {
            return FileBiz.DeleteFile(json);
        }
        
        [HttpPut]
        [Route("api/file")]
        public object RenameFile(object json)
        {
            return FileBiz.RenameFile(json);
        }
        
        [HttpGet]
        [Route("api/file/list")]
        public object GetList(int projectId)
        {
            return FileBiz.GetList(projectId);
        }
    }
}