using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("downloadfiles")]
    public class DownloadFile : IEntity
    {
        public string Id { get; set; }
        public string DownloadId { get; set; }
        public string Filename { get; set; }
        public string RandomName { get; set; }
    }
}