using Xunit;

namespace Todo.Api.Tests.Fixtures;

[CollectionDefinition("Api")]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
}
