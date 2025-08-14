using Google.Protobuf.WellKnownTypes;
using Riok.Mapperly.Abstractions;

namespace Function.SDK.CSharp.Services;

[Mapper]
public static partial class StructMapper
{
    public static partial void Update(this Struct existing, Struct update);
}
