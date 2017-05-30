namespace Backend.Sample.Model
{
    public class SampleFile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }

        public byte[] Content { get; set; }
    }
}