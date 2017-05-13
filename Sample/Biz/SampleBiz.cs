using System;
using System.Data.Entity;
using Backend.Sample.Model;

namespace Backend.Sample.Biz
{
    public static class SampleBiz
    {
        public static Object GetFromSampleDb()
        {
            using (var context = new SampleContext())
            {
                var data = new SampleData()
                {
                    Title = "It is title",
                    Content = "It is content",
                    CreateDate = DateTime.Now
                };
                context.Category.Add(data);
                context.SaveChanges();

                return data;
            }
        }
    }
}