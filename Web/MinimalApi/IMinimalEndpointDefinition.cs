using Microsoft.AspNetCore.Routing;

namespace Fundation.Abstractions.Web.MinimalApi;

public interface IMinimalEndpointDefinition
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
