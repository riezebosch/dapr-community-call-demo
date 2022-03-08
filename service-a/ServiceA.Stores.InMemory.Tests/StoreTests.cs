using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ServiceA.Stores.InMemory.Tests;

public class StoreTests
{
    [Fact]
    public async Task Get()
    {
        var store = new Store();
        var input = await store.New(new Request(""));

        input
            .Id
            .Should()
            .NotBeNullOrEmpty();

        var output = await store.Get(input.Id);
        output
            .Should()
            .Be(input);
    }
    
    [Fact]
    public async Task Update()
    {
        var store = new Store();
        var input = await store.New(new Request(""));

        var output = await store.Update(input with { Status = Status.Approved });
        output
            .Status
            .Should()
            .Be(Status.Approved);
    }
}