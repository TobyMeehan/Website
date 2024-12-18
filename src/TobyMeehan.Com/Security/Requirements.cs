using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TobyMeehan.Com.Security;

public sealed class Requirements
{
    public sealed class Download
    {
        public static readonly OperationAuthorizationRequirement View = new()
            { Name = Operations.Download.View };

        public static readonly OperationAuthorizationRequirement Manage = new()
            { Name = Operations.Download.Manage };

        public static readonly OperationAuthorizationRequirement Edit = new()
            { Name = Operations.Download.Edit };

        public static readonly OperationAuthorizationRequirement Delete = new()
            { Name = Operations.Download.Delete };

        public sealed class Files
        {
            public static readonly OperationAuthorizationRequirement Upload = new()
                { Name = Operations.Download.Files.Upload };
        }

        public sealed class Authors
        {
            public static readonly OperationAuthorizationRequirement Invite = new()
                { Name = Operations.Download.Authors.Invite };

            public static readonly OperationAuthorizationRequirement Kick = new()
                { Name = Operations.Download.Authors.Kick };
        }
    }

    public sealed class File
    {
        public static readonly OperationAuthorizationRequirement View = new()
            { Name = Operations.File.View };

        public static readonly OperationAuthorizationRequirement Edit = new()
            { Name = Operations.File.Edit };

        public static readonly OperationAuthorizationRequirement Delete = new()
            { Name = Operations.File.Delete };
    }

    public sealed class Comment
    {
        public static readonly OperationAuthorizationRequirement View = new()
            { Name = Operations.Comment.View };

        public static readonly OperationAuthorizationRequirement Edit = new()
            { Name = Operations.Comment.Edit };

        public static readonly OperationAuthorizationRequirement Delete = new()
            { Name = Operations.Comment.Delete };
    }
}