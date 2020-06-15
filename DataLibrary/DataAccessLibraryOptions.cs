using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TobyMeehan.Com.Data
{
    public class DataAccessLibraryOptions
    {
        public Func<IDbConnection> ConnectionFactory { get; set; }
        public GoogleCredential StorageCredential { get; set; }
        public string DownloadStorageBucket { get; set; }
        public string ProfilePictureStorageBucket { get; set; }

    }
}
