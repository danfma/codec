using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record ProjectSpec(
    string Name,
    ConfigDefinition? Config,
    ImmutableArray<TypeAlias> TypeAliases,
    ImmutableArray<EnumDefinition> Enums,
    ImmutableArray<ComponentDefinition> Components,
    ImmutableArray<TraitDefinition> Traits,
    ImmutableArray<EntityDefinition> Entities,
    ImmutableArray<AbstractEntityDefinition> AbstractEntities,
    ImmutableArray<ViewDefinition> Views,
    ImmutableArray<ServiceDefinition> Services
);
