using System.Collections.Immutable;
using Codec.Core.AST;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Codec.Core.Parsing;

public static class CodecParser
{
    // Basic parsers
    private static readonly Parser<char, char> Space = Char(' ');
    private static readonly Parser<char, IEnumerable<char>> Spaces = Space.AtLeastOnce();
    private static readonly Parser<char, char> EqualsSign = Char('=');

    // Identifiers
    private static readonly Parser<char, string> PascalCaseIdentifier = Letter
        .Where(char.IsUpper)
        .Then(LetterOrDigit.Or(Char('_')).ManyString(), (first, rest) => first + rest);

    // Keywords
    private static readonly Parser<char, string> TypeKeyword = String("type");

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

    // Type alias parser - simple version without annotations for now

    internal static readonly Parser<char, TypeAlias> ParseTypeAlias = Map(
        (_, _, name, _, _, _, type) => new TypeAlias(name, type, ImmutableArray<Annotation>.Empty),
        TypeKeyword,
        Spaces,
        PascalCaseIdentifier,
        Space,
        EqualsSign,
        Space,
        PrimitiveType
    );

    internal static readonly Parser<char, ImmutableArray<TypeAlias>> ParseTypeAliases =
        ParseTypeAlias.Many().Select(aliases => aliases.ToImmutableArray());
}
