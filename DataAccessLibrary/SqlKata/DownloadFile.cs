using System;
using Slapper;

namespace TobyMeehan.Com.Data.SqlKata;

public class DownloadFile : IDownloadFile
{
    public Id<IDownloadFile> Id { get; set; }

    public IFile InnerFile { get; set; }
    
    public string Filename { get; set; }

    public string DownloadLink => InnerFile.DownloadLink;
    public string MediaLink => InnerFile.MediaLink;
    public MediaType ContentType => InnerFile.ContentType;
}

public class DownloadFileActivator : AutoMapper.Configuration.ITypeActivator
{
    public object Create(Type type)
    {
        return new DownloadFile();
    }

    public bool CanCreate(Type type)
    {
        return type == typeof(IDownloadFile);
    }

    public int Order => 110;
}