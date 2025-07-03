namespace Codec.Core.AST;

public abstract record RelationshipDefinition(
    string Name,
    TypeInfo TargetType,
    RelationshipSide Side,
    string? ForeignKeyField = null,
    string? MappedByProperty = null,
    OnDeleteAction OnDelete = OnDeleteAction.None
);
