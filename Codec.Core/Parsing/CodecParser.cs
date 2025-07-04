using System.Collections.Immutable;
using Codec.Core.AST;
using Pidgin;
using static Pidgin.Parser;

namespace Codec.Core.Parsing;

public static class CodecParser
{
    // Basic parsers
    private static readonly Parser<char, char> Space = Char(' ');
    private static readonly Parser<char, IEnumerable<char>> Spaces = Space.AtLeastOnce();
    private static readonly Parser<char, char> EqualsSign = Char('=');
    private static readonly Parser<char, char> OpenBrace = Char('{');
    private static readonly Parser<char, char> CloseBrace = Char('}');
    private static readonly Parser<char, char> OpenParen = Char('(');
    private static readonly Parser<char, char> CloseParen = Char(')');
    private static readonly Parser<char, char> Quote = Char('"');
    private static readonly Parser<char, char> Newline = Char('\n');
    private static readonly Parser<char, char> CarriageReturn = Char('\r');
    private static readonly Parser<char, char> Tab = Char('\t');
    private static readonly Parser<char, Unit> Whitespace = OneOf(
            Space,
            Tab,
            Newline,
            CarriageReturn
        )
        .IgnoreResult();
    private static readonly Parser<char, Unit> OptionalWhitespace = Whitespace
        .Many()
        .IgnoreResult();

    // Identifiers
    private static readonly Parser<char, string> PascalCaseIdentifier = Letter
        .Where(char.IsUpper)
        .Then(LetterOrDigit.Or(Char('_')).ManyString(), (first, rest) => first + rest);

    private static readonly Parser<char, string> SnakeCaseIdentifier = Letter
        .Where(char.IsLower)
        .Then(LetterOrDigit.Or(Char('_')).ManyString(), (first, rest) => first + rest);

    // Keywords
    private static readonly Parser<char, string> TypeKeyword = String("type");
    private static readonly Parser<char, string> ConstraintKeyword = String("constraint");
    private static readonly Parser<char, string> TraitKeyword = String("trait");
    private static readonly Parser<char, string> VarKeyword = String("var");
    private static readonly Parser<char, string> ValKeyword = String("val");
    private static readonly Parser<char, string> EntityKeyword = String("entity");
    private static readonly Parser<char, string> AbstractKeyword = String("abstract");
    private static readonly Parser<char, string> ServiceKeyword = String("service");

    // Primitive types - order matters for longest match first
    private static readonly Parser<char, TypeInfo> PrimitiveType = OneOf(
        String("ByteArray").ThenReturn(TypeInfo.ByteArray),
        String("DateTime").ThenReturn(TypeInfo.DateTime),
        String("TimeSpan").ThenReturn(TypeInfo.TimeSpan),
        String("Float32").ThenReturn(TypeInfo.Float32),
        String("Float64").ThenReturn(TypeInfo.Float64),
        String("Decimal").ThenReturn(TypeInfo.Decimal),
        String("String").ThenReturn(TypeInfo.String),
        String("UInt64").ThenReturn(TypeInfo.UInt64),
        String("UInt32").ThenReturn(TypeInfo.UInt32),
        String("UInt16").ThenReturn(TypeInfo.UInt16),
        String("Int64").ThenReturn(TypeInfo.Int64),
        String("Int32").ThenReturn(TypeInfo.Int32),
        String("Int16").ThenReturn(TypeInfo.Int16),
        String("UInt8").ThenReturn(TypeInfo.UInt8),
        String("Int8").ThenReturn(TypeInfo.Int8),
        String("Bool").ThenReturn(TypeInfo.Bool),
        String("Date").ThenReturn(TypeInfo.Date),
        String("Uuid").ThenReturn(TypeInfo.Uuid),
        String("Json").ThenReturn(TypeInfo.Json)
    );

    // Constraint parsers
    private static readonly Parser<char, string> QuotedString = Quote
        .Then(AnyCharExcept('"').ManyString())
        .Before(Quote);

    private static readonly Parser<char, int> IntegerLiteral = Digit
        .AtLeastOnce()
        .Select(digits => int.Parse(string.Join("", digits)));

    private static readonly Parser<char, Annotation> MinLenConstraint = Map(
        (_, _, _, value, _) => (Annotation)new Min(value),
        ConstraintKeyword,
        OptionalWhitespace,
        String("min_len"),
        OpenParen.Then(OptionalWhitespace).Then(IntegerLiteral).Before(OptionalWhitespace),
        CloseParen
    );

    private static readonly Parser<char, Annotation> MaxLenConstraint = Map(
        (_, _, _, value, _) => (Annotation)new Max(value),
        ConstraintKeyword,
        OptionalWhitespace,
        String("max_len"),
        OpenParen.Then(OptionalWhitespace).Then(IntegerLiteral).Before(OptionalWhitespace),
        CloseParen
    );

    private static readonly Parser<char, Annotation> RegexConstraint = Map(
        (_, _, _, pattern, _) => (Annotation)new Regex(pattern),
        ConstraintKeyword,
        OptionalWhitespace,
        String("regex"),
        OpenParen.Then(OptionalWhitespace).Then(QuotedString).Before(OptionalWhitespace),
        CloseParen
    );

    private static readonly Parser<char, Annotation> UniqueConstraint = Map(
        (_, _, _) => (Annotation)new Unique(),
        ConstraintKeyword,
        OptionalWhitespace,
        String("unique")
    );

    private static readonly Parser<char, Annotation> ConstraintAnnotation = OneOf(
        MinLenConstraint,
        MaxLenConstraint,
        RegexConstraint,
        UniqueConstraint
    );

    internal static readonly Parser<char, ImmutableArray<Annotation>> ParseConstraintBlock =
        from openBrace in OpenBrace
        from ws1 in OptionalWhitespace
        from constraint in ConstraintAnnotation.Optional()
        from ws2 in OptionalWhitespace
        from closeBrace in CloseBrace
        select constraint.HasValue
            ? ImmutableArray.Create(constraint.Value)
            : ImmutableArray<Annotation>.Empty;

    // Type alias parser - supports both simple and constraint block versions
    internal static readonly Parser<char, TypeAlias> ParseTypeAlias = OptionalWhitespace
        .Then(TypeKeyword)
        .Then(Spaces)
        .Then(PascalCaseIdentifier, (_, name) => name)
        .Before(OptionalWhitespace)
        .Before(EqualsSign)
        .Before(OptionalWhitespace)
        .Then(PrimitiveType, (name, type) => new { name, type })
        .Before(OptionalWhitespace)
        .Then(
            ParseConstraintBlock.Optional(),
            (nameType, constraints) =>
                new TypeAlias(
                    nameType.name,
                    nameType.type,
                    constraints.GetValueOrDefault(ImmutableArray<Annotation>.Empty)
                )
        );

    internal static readonly Parser<char, ImmutableArray<TypeAlias>> ParseTypeAliases =
        ParseTypeAlias.Many().Select(aliases => aliases.ToImmutableArray());

    // Field definition parsers
    private static readonly Parser<char, char> Colon = Char(':');

    private static readonly Parser<char, FieldDefinition> VarFieldDefinition =
        from leadingWs in OptionalWhitespace
        from varKeyword in VarKeyword
        from ws1 in OptionalWhitespace
        from name in SnakeCaseIdentifier
        from ws2 in OptionalWhitespace
        from colon in Colon
        from ws3 in OptionalWhitespace
        from type in PrimitiveType
        select new FieldDefinition(name, type, true, ImmutableArray<Annotation>.Empty);

    private static readonly Parser<char, FieldDefinition> ValFieldDefinition =
        from leadingWs in OptionalWhitespace
        from valKeyword in ValKeyword
        from ws1 in OptionalWhitespace
        from name in SnakeCaseIdentifier
        from ws2 in OptionalWhitespace
        from colon in Colon
        from ws3 in OptionalWhitespace
        from type in PrimitiveType
        select new FieldDefinition(name, type, false, ImmutableArray<Annotation>.Empty);

    private static readonly Parser<char, FieldDefinition> ParseFieldDefinition = OneOf(
        VarFieldDefinition,
        ValFieldDefinition
    );

    // Trait definition parser
    internal static readonly Parser<char, TraitDefinition> ParseTrait =
        from leadingWs in OptionalWhitespace
        from traitKeyword in TraitKeyword
        from ws1 in Spaces
        from name in PascalCaseIdentifier
        from ws2 in OptionalWhitespace
        from openBrace in OpenBrace
        from ws3 in OptionalWhitespace
        from field in ParseFieldDefinition.Optional()
        from ws4 in OptionalWhitespace
        from closeBrace in CloseBrace
        select new TraitDefinition(
            name,
            field.HasValue
                ? ImmutableHashSet.Create(field.Value)
                : ImmutableHashSet<FieldDefinition>.Empty
        );

    // Entity parsers
    private static readonly Parser<char, TraitRef> TraitReference =
        from name in PascalCaseIdentifier
        select new TraitRef(name);

    private static readonly Parser<char, ImmutableArray<TraitRef>> TraitInheritance =
        from colon in Colon
        from ws1 in OptionalWhitespace
        from trait in TraitReference
        select ImmutableArray.Create(trait);

    internal static readonly Parser<char, EntityDefinition> ParseEntity =
        from leadingWs in OptionalWhitespace
        from entityKeyword in EntityKeyword
        from ws1 in Spaces
        from name in PascalCaseIdentifier
        from ws2 in OptionalWhitespace
        from traits in TraitInheritance.Optional()
        from ws3 in OptionalWhitespace
        from openBrace in OpenBrace
        from ws4 in OptionalWhitespace
        from field in ParseFieldDefinition.Optional()
        from ws5 in OptionalWhitespace
        from closeBrace in CloseBrace
        select new EntityDefinition(
            name,
            null, // BaseType
            traits.GetValueOrDefault(ImmutableArray<TraitRef>.Empty),
            field.HasValue
                ? ImmutableArray.Create(field.Value)
                : ImmutableArray<FieldDefinition>.Empty,
            ImmutableArray<RelationshipDefinition>.Empty,
            ImmutableArray<Constraint>.Empty
        );

    internal static readonly Parser<char, AbstractEntityDefinition> ParseAbstractEntity =
        from leadingWs in OptionalWhitespace
        from abstractKeyword in AbstractKeyword
        from ws1 in Spaces
        from entityKeyword in EntityKeyword
        from ws2 in Spaces
        from name in PascalCaseIdentifier
        from ws3 in OptionalWhitespace
        from traits in TraitInheritance.Optional()
        from ws4 in OptionalWhitespace
        from openBrace in OpenBrace
        from ws5 in OptionalWhitespace
        from field in ParseFieldDefinition.Optional()
        from ws6 in OptionalWhitespace
        from closeBrace in CloseBrace
        select new AbstractEntityDefinition(
            name,
            traits.GetValueOrDefault(ImmutableArray<TraitRef>.Empty),
            field.HasValue
                ? ImmutableArray.Create(field.Value)
                : ImmutableArray<FieldDefinition>.Empty,
            new InheritanceStrategy(InheritanceStrategyType.JoinedTable)
        );

    // Service definition parser - simplified to only parse service name
    internal static readonly Parser<char, ServiceDefinition> ParseService =
        from leadingWs in OptionalWhitespace
        from serviceKeyword in ServiceKeyword
        from ws1 in Spaces
        from name in PascalCaseIdentifier
        from ws2 in OptionalWhitespace
        from openBrace in OpenBrace
        from ws3 in OptionalWhitespace
        from closeBrace in CloseBrace
        select new ServiceDefinition(name, ImmutableArray<FunctionDefinition>.Empty);
}
