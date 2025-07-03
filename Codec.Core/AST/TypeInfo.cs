namespace Codec.Core.AST;

public sealed record TypeInfo(string Name)
{
    public static readonly TypeInfo Bool = new(nameof(Bool));
    public static readonly TypeInfo String = new(nameof(String));
    public static readonly TypeInfo Int8 = new(nameof(Int8));
    public static readonly TypeInfo Int16 = new(nameof(Int16));
    public static readonly TypeInfo Int32 = new(nameof(Int32));
    public static readonly TypeInfo Int64 = new(nameof(Int64));
    public static readonly TypeInfo UInt8 = new(nameof(UInt8));
    public static readonly TypeInfo UInt16 = new(nameof(UInt16));
    public static readonly TypeInfo UInt32 = new(nameof(UInt32));
    public static readonly TypeInfo UInt64 = new(nameof(UInt64));
    public static readonly TypeInfo Date = new(nameof(Date));
    public static readonly TypeInfo DateTime = new(nameof(DateTime));
    public static readonly TypeInfo TimeSpan = new(nameof(TimeSpan));
    public static readonly TypeInfo Decimal = new(nameof(Decimal));
    public static readonly TypeInfo Float32 = new(nameof(Float32));
    public static readonly TypeInfo Float64 = new(nameof(Float64));
    public static readonly TypeInfo Uuid = new(nameof(Uuid));
    public static readonly TypeInfo ByteArray = new(nameof(ByteArray));
    public static readonly TypeInfo Json = new(nameof(Json));
}
