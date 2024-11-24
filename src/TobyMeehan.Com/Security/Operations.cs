namespace TobyMeehan.Com.Security;

public sealed class Operations
{
    public sealed class Download
    {
        public const string View = "operation.download.view";
        public const string Manage = "operation.download.manage";
        public const string Edit = "operation.download.edit";
        public const string Delete = "operation.download.delete";
        
        public sealed class Files
        {
            public const string Upload = "operation.download.files.upload";
            public const string Delete = "operation.download.files.delete";
        }
        
        public sealed class Authors
        {
            public const string Invite = "operation.download.authors.invite";
            public const string Kick = "operation.download.authors.kick";
        }
    }
    
    public sealed class File
    {
        public const string View = "operation.file.view";
    }
    
    public sealed class Comment
    {
        public const string View = "operation.comment.view";
        public const string Edit = "operation.comment.edit";
        public const string Delete = "operation.comment.delete";
    }
}