#r "Codec.Core/bin/Debug/net10.0/Codec.Core.dll"
#r "nuget: Pidgin, 3.4.0"

using Codec.Core.AST;
using Codec.Core.Parsing;
using Pidgin;

var input1 = "entity Individual { val cpf: String }";
var input2 = @"entity Individual {
    val cpf: String
}";

Console.WriteLine($"Single line input: '{input1}'");
var result1 = CodecParser.ParseEntity.Parse(input1);
Console.WriteLine($"Single line result: {result1.Success}");
if (!result1.Success)
{
    Console.WriteLine($"Error: {result1.Error}");
}

Console.WriteLine($"\nMulti line input: '{input2}'");
var result2 = CodecParser.ParseEntity.Parse(input2);
Console.WriteLine($"Multi line result: {result2.Success}");
if (!result2.Success)
{
    Console.WriteLine($"Error: {result2.Error}");
}