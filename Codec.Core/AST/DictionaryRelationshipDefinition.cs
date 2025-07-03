namespace Codec.Core.AST;

public sealed record DictionaryRelationshipDefinition(
    string Name,
    TypeInfo TargetType,
    TypeInfo KeyType,
    RelationshipSide Side,
    string? ForeignKeyField = null,
    string? MappedByProperty = null,
    OnDeleteAction OnDelete = OnDeleteAction.None
) : RelationshipDefinition(Name, TargetType, Side, ForeignKeyField, MappedByProperty, OnDelete);
