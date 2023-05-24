namespace Equinor.ProCoSys.BusSenderWorker.Core.Mappers;

/***
 * If and when we merge topics, we will use this class to map.
    ie,  if(e == 'tag') return PcsTopicConstants.Completion.
 */
internal static class BusEventToTopicMapper
{
    public static string Map(string e) => e;
}
