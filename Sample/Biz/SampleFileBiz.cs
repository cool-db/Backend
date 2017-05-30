using System;
using System.Linq;
using System.Text;
using System.Web;
using Backend.Sample.Model;

namespace Backend.Sample.Biz
{
    public class SampleFileBiz
    {
        public static SampleFile GetFile(int FileId)
        {
            using (var context = new SampleContext())
            {
                var sampleFile = (from file in context.Files
                    where file.Id == FileId
                    select file).SingleOrDefault();
                return sampleFile;
            }
        }

        public static object PostFile(object json, HttpPostedFile file)
        {
            var stream = file.InputStream;
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            using (var context = new SampleContext())
            {
                var newFile = new SampleFile()
                {
                    Name = file.FileName,
                    Type = file.ContentType,
                    Size = file.InputStream.Length,
                    Content = bytes,
                };
                context.Files.Add(newFile);
                context.SaveChanges();
                return newFile.Id;
            }
        }
    }
}