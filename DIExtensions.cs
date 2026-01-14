using System;
using Microsoft.Extensions.DependencyInjection;

namespace Pathfinding;

public static class DIExtensions
{
    public static IServiceCollection AddPathfinding(this IServiceCollection services)
    {
        services.AddSingleton<IPathfindingService, PathfindingService>();
        return services;
    }
}
