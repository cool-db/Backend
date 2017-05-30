using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Backend.Sample.Biz;

namespace Backend.Sample.Controller
{
    public class SampleFileController : ApiController
    {
        [HttpGet]
        public object GetFile(int FileId)
        {
            var file = SampleFileBiz.GetFile(FileId);
            var response = Request.CreateResponse();
            response.Content = new ByteArrayContent(file.Content);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.Type);

            return response;
        }

        [HttpPost]
        public object PostFile(object json)
        {
            return SampleFileBiz.PostFile(json, HttpContext.Current.Request.Files[0]);
        }
    }
}