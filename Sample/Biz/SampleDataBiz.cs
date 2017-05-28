using System;
using System.Data.Entity;
using System.Web;
using Backend.Sample.Model;

namespace Backend.Sample.Biz
{
    public static class SampleDataBiz
    {
        public static object GetFromSampleDb()
        {
            using (var context = new SampleContext())
            {
                var data = new SampleData()
                {
                    Title = "It is title",
                    Content = "It is content",
                    CreateDate = DateTime.Now
                };
                context.Datas.Add(data);
                context.SaveChanges();
                
                return data;
            }
        }
    }
}