using TobyMeehan.Com.Models;
using TobyMeehan.Com.Models.Application;

namespace TobyMeehan.Com.Builders.Application;

/// <summary>
/// Builder structure used to update an application.
/// </summary>
public struct UpdateApplicationBuilder : IUpdateApplication
{
    /// <summary>
    /// Sets the <see cref="Download"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateApplicationBuilder WithDownload(Id<IDownload> value) => this with { Download = value };
    
    /// <summary>
    /// The download to be associated with the application.
    /// </summary>
    public Optional<Id<IDownload>?> Download { get; set; }
    
    /// <summary>
    /// Sets the <see cref="Name"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateApplicationBuilder WithName(string value) => this with { Name = value };
    
    /// <summary>
    /// The new name of the application.
    /// </summary>
    public Optional<string> Name { get; set; }

    /// <summary>
    /// Sets the <see cref="Description"/> property of the application.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateApplicationBuilder WithDescription(string? value) => this with { Description = value };
    
    /// <summary>
    /// The new description of the application.
    /// </summary>
    public Optional<string?> Description { get; set; }

    /// <summary>
    /// Sets the <see cref="Icon"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateApplicationBuilder WithIcon(IFileUpload? value) => this with { Icon = Optional<IFileUpload?>.Of(value) };
    
    /// <summary>
    /// The new icon of the application.
    /// </summary>
    public Optional<IFileUpload?> Icon { get; set; }
}