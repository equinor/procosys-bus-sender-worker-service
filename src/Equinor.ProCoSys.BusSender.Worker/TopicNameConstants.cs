using System.Collections.Generic;

namespace Equinor.ProCoSys.BusSender.Worker;

public class TopicNameConstants
{
    public static List<string> TopicNames { get; } =
    [
        "tagequipment",
        "swcrattachment",
        "swcrotherreference",
        "swcrtype",
        "action",
        "task",
        "commpkgtask",
        "commpkg",
        "mcpkg",
        "wo",
        "person",
        "project",
        "responsible",
        "tagfunction",
        "tag",
        "punchlistitem",
        "library",
        "checklist",
        "milestone",
        "wocutoff",
        "swcr",
        "swcrsignature",
        "query",
        "querysignature",
        "certificate",
        "wochecklist",
        "womaterial",
        "womilestone",
        "stock",
        "piperevision",
        "document",
        "pipingspool",
        "commpkgoperation",
        "loopcontent",
        "commpkgquery",
        "calloff",
        "heattrace",
        "libraryfield",
        "mcpkgmilestone",
        "commpkgmilestone",
        "heattracepipetest",
        "notification",
        "notificationworkorder",
        "punchprioritylibraryrelation",
        "punchlistitemattachment",
        "notificationcommpkg",
        "notificationsignature"
    ];
}
