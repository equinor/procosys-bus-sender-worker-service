using System.Text.Json;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Extensions;

public static class DefaultSerializerHelper
{
    public static JsonSerializerOptions SerializerOptions { get; } = new()
    {
        Converters = { new DateOnlyJsonConverter() }
    };
}
