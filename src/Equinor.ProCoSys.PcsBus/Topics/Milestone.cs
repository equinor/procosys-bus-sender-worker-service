using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public struct Milestone
    {
        string Code;
        DateTime Planned;
        DateTime Actual;
    }
}
