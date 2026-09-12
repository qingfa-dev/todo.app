using Shouldly;

using Todo.Api.SharedKernel.Models;
using SortDirection = Todo.Api.SharedKernel.Models.SortDirection;

namespace Todo.Api.UnitTests.SharedKernel.Models;

public class PagedParametersTests
{
    [Fact]
    public void PagedParameters_ShouldHaveDefaultValues()
    {
        var parameters = new PagedParameters();

        parameters.Page.ShouldBe(PagingParameterConstant.Defaults.Page);
        parameters.PageSize.ShouldBe(PagingParameterConstant.Defaults.PageSize);
        parameters.SortBy.ShouldBe(SortingParameterConstant.Defaults.SortBy);
        parameters.SortDirection.ShouldBe(SortingParameterConstant.Defaults.SortDirection);
    }

    [Fact]
    public void PagingParameterConstant_Defaults_ShouldHaveCorrectValues()
    {
        PagingParameterConstant.Defaults.Page.ShouldBe(1);
        PagingParameterConstant.Defaults.PageSize.ShouldBe(20);
        PagingParameterConstant.Defaults.MaxPageSize.ShouldBe(100);
    }

    [Fact]
    public void SortingParameterConstant_Defaults_ShouldHaveCorrectValues()
    {
        SortingParameterConstant.Defaults.SortBy.ShouldBe(string.Empty);
        SortingParameterConstant.Defaults.SortDirection.ShouldBe(Todo.Api.SharedKernel.Models.SortDirection.Ascending);
    }
}
