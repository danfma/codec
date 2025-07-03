namespace Codec.Core.AST;

public sealed record SingleRelationshipDefinition(
    string Name,
    TypeInfo TargetType,
    RelationshipSide Side,
    string? ForeignKeyField = null,
    string? MappedByProperty = null,
    OnDeleteAction OnDelete = OnDeleteAction.None
) : RelationshipDefinition(Name, TargetType, Side, ForeignKeyField, MappedByProperty, OnDelete);
