using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record CodecProject(
    string? PackageName,
    ImmutableArray<TypeAlias> TypeAliases,
    ImmutableArray<TraitDefinition> Traits,
    ImmutableArray<AbstractEntityDefinition> AbstractEntities,
    ImmutableArray<EntityDefinition> Entities,
    ImmutableArray<ServiceDefinition> Services,
    ImmutableArray<ConfigDefinition> Configs
);