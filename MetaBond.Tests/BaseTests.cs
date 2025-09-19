using System.Reflection;
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Infrastructure.Persistence.Context;
using Program = MetaBond.Presentation.Api;

namespace MetaBond.Tests;

public abstract class BaseTests
{
    protected static readonly Assembly DomainAssembly = typeof(Domain.Models.Communities).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(MetaBondContext).Assembly;
}