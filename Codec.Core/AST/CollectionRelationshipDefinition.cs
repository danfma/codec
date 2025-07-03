namespace Codec.Core.AST;

public sealed record CollectionRelationshipDefinition(
    string Name,
    TypeInfo TargetType,
    CollectionType CollectionType,
    RelationshipSide Side,
    string? ForeignKeyField = null,
    string? MappedByProperty = null,
    OnDeleteAction OnDelete = OnDeleteAction.None
) : RelationshipDefinition(Name, TargetType, Side, ForeignKeyField, MappedByProperty, OnDelete);
