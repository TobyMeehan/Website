using FastEndpoints.Testing;

namespace Api.Tests;

[CollectionDefinition(Name)]
public class ApiDefinition : TestCollection<ApiApp>
{
    public const string Name = nameof(ApiDefinition);
}