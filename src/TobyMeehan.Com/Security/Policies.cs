using Microsoft.AspNetCore.Authorization;

namespace TobyMeehan.Com.Security;

public static class Policies
{
    public const string CreateDownload = "policies.download.create";
    public const string ViewDownload = "policies.download.view";
    public const string ManageDownload = "policies.download.manage";
    public const string EditDownload = "policies.download.edit";
    public const string DeleteDownload = "policies.download.delete";
    
    public const string UploadFile = "policies.download.files.upload";
    
    public const string InviteAuthor = "policies.download.authors.invite";
    public const string KickAuthor = "policies.download.authors.kick";
    
    public const string ViewFile = "policies.file.view";
    public const string EditFile = "policies.file.edit";
    public const string DeleteFile = "policies.file.delete";
    
    public const string CreateComment = "policies.comment.create";
    public const string ViewComment = "policies.comment.view";
    public const string EditComment = "policies.comment.edit";
    public const string DeleteComment = "policies.comment.delete";

    public static AuthorizationBuilder RegisterPolicies(this AuthorizationBuilder authorizationBuilder) =>
        authorizationBuilder

            .AddPolicy(ViewDownload, builder => builder
                .AddRequirements(Requirements.Download.View))
            .AddPolicy(ViewFile, builder => builder
                .AddRequirements(Requirements.File.View))
            .AddPolicy(ViewComment, builder => builder
                .AddRequirements(Requirements.Comment.View))
                
            .AddPolicy(CreateDownload, builder => builder
                .RequireAuthenticatedUser())
            .AddPolicy(ManageDownload, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Download.Manage))
            .AddPolicy(EditDownload, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Download.Edit))
            .AddPolicy(DeleteDownload, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Download.Delete))
        
            .AddPolicy(UploadFile, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Download.Files.Upload))
            .AddPolicy(EditFile, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.File.Edit))
            .AddPolicy(DeleteFile, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.File.Delete))
        
            .AddPolicy(InviteAuthor, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Download.Authors.Invite))
            .AddPolicy(KickAuthor, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Download.Authors.Kick))
        
            .AddPolicy(CreateComment, builder => builder
                .RequireAuthenticatedUser())
            .AddPolicy(EditComment, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Comment.Edit))
            .AddPolicy(DeleteComment, builder => builder
                .RequireAuthenticatedUser()
                .AddRequirements(Requirements.Comment.Delete));
}