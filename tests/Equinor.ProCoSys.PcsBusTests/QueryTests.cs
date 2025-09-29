using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests;

[TestClass]
public class QueryTests
{
    /// <summary>
    /// This test just checks that all interfaces are included in the following test.
    /// </summary>
    [TestMethod]
    public void GetTestData_IncludesAllEventInterfaces()
    {
        // Arrange
        const string assemblyName = "Equinor.ProCoSys.PcsServiceBus";
        var  assembly =  AppDomain.CurrentDomain.GetAssemblies()
            .First(a => a.FullName!.Split(',')[0].Equals(assemblyName, StringComparison.OrdinalIgnoreCase));
        
        var parentInterface = typeof(IHasEventType);
        var interfaces = assembly
            .GetTypes()
            .Where(t => t.IsInterface && t.GetInterfaces().Contains(parentInterface))
            .ToArray().ToList();

        // Act
        // Get the second parameter of the test data, which is the interface type
        var testedInterfaces = GetTestData().Select(td=> td[1]).ToList();
        
        // Assert
        foreach (var i in interfaces)
        {
            Assert.IsTrue(testedInterfaces.Contains(i), $"The interface {i} is not included in the test data, please create a test.");
            
        }
    }

    /// <summary>
    /// This test method is used to check that the queries in namespace <see cref="Equinor.ProCoSys.PcsServiceBus.Queries"/> contains the correct fields corresponding to the interface using them.
    /// For example, CheckListQuery is expected to be used to populate the a class that inherits the IChecklistEventV1 interface.
    /// IChecklistEventV1 contains a property called Plant, so the query should contain a field called Plant.
    /// Renaming the Query or the interface without updating the other will cause this test to fail.
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
    public void GetQuery_SelectFields_MatchesEventInterface(Type classType, Type interfaceType, Type[] parameterTypes,
        object[] parameters)
    {
        // Arrange
        var method = classType.GetMethod("GetQuery", parameterTypes);

        // Act
        Assert.IsNotNull(method);
        var result = method.Invoke(null, parameters);
        Assert.IsNotNull(result);
        var (actualQuery, _) = (ValueTuple<string, DynamicParameters>)result;

        // Get all property names from the interface
        var interfaceProperties = interfaceType.GetProperties().Select(p => p.Name);

        // Assert
        foreach (var property in interfaceProperties)
        {
            if (property == "InstallationCode" && interfaceType == typeof(IDocumentEventV1))
         
            {
                // Special case: InstallationCode is not included in DocumentQuery, but queried seperatly after
                continue;
            }
            // Check if the SQL query contains the property name in the expected format
            // e.g. abc.property_name as PropertyName
            var expectedInQuery = $" as {property}";
            var expectedInQueryUpper = $" AS {property}";
            Assert.IsTrue(actualQuery.Contains(expectedInQuery) || actualQuery.Contains(expectedInQueryUpper),
                $"The query does not contain the property {property} from the interface {interfaceType.FullName}.");
        }
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return
        [
            typeof(ActionQuery),
            typeof(IActionEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];

        yield return
        [
            typeof(NotificationQuery),
            typeof(INotificationEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, null }
        ];

        yield return
        [
            typeof(CallOffQuery), 
            typeof(ICallOffEventV1), 
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        yield return
        [
            typeof(ChecklistQuery),
            typeof(IChecklistEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        yield return
        [
            typeof(CommPkgQuery), 
            typeof(ICommPkgEventV1), 
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        yield return
        [
            typeof(CommPkgMilestoneQuery),
            typeof(ICommPkgMilestoneEventV1), 
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];
        yield return
        [
            typeof(CommPkgOperationQuery), 
            typeof(ICommPkgOperationEventV1), 
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(CommPkgQueryQuery),
            typeof(ICommPkgQueryEventV1),
            new[] { typeof(long?),typeof(long?), typeof(string) },
            new object[] { 1L, 2L, "testPlant" }
        ];

        yield return
        [
            typeof(CommPkgTaskQuery),
            typeof(ICommPkgTaskEventV1),
            new[] { typeof(long?),typeof(long?), typeof(string) },
            new object[] { 1L, 3L, "testPlant" }
        ];

        yield return
        [
            typeof(DocumentQuery),
            typeof(IDocumentEventV1),
            new[] { typeof(long?), typeof(string), typeof(string) },
            new object[] { 1L, "testPlant", "noExtra" }
        ];
        
        yield return
        [
            typeof(HeatTraceQuery),
            typeof(IHeatTraceEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(LibraryFieldQuery),
            typeof(ILibraryFieldEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { Guid.NewGuid().ToString(), "testPlant" }
        ];
        
        yield return
        [
            typeof(LibraryQuery),
            typeof(ILibraryEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];

        yield return
        [
            typeof(LibraryToLibraryQuery),
            typeof(ILibraryToLibraryEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { Guid.NewGuid().ToString(), "testPlant" }
        ];
        
        yield return
        [
            typeof(LoopContentQuery),
            typeof(ILoopContentEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];
        
        yield return
        [
            typeof(McPkgMilestoneQuery), 
            typeof(IMcPkgMilestoneEventV1), 
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];
        
        yield return
        [
            typeof(McPkgQuery), 
            typeof(IMcPkgEventV1), 
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];

        yield return
        [
            typeof(PipingRevisionQuery),
            typeof(IPipingRevisionEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(PipingSpoolQuery), 
            typeof(IPipingSpoolEventV1), 
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];

        yield return
        [
            typeof(ProjectQuery),
            typeof(IProjectEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "1L", "testPlant" }
        ];

        yield return
        [
            typeof(PersonQuery),
            typeof(IPersonEventV1),
            new[] { typeof(string) },
            new object[] { "1L" }
        ];

        yield return
        [
            typeof(PunchListItemQuery),
            typeof(IPunchListItemEventV1),
            new[] { typeof(long?), typeof(string), typeof(string) },
            new object[] { 1L, "testPlant", null }
        ];
        
        yield return
        [
            typeof(QueryQuery),
            typeof(IQueryEventV1),
            new[] { typeof(long?),  typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(QuerySignatureQuery),
            typeof(IQuerySignatureEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(ResponsibleQuery),
            typeof(IResponsibleEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(StockQuery),
            typeof(IStockEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(SwcrAttachmentQuery),
            typeof(IAttachmentEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "1L, 2L,", "testPlant" }
        ];
        
        yield return
        [
            typeof(SwcrOtherReferenceQuery),
            typeof(ISwcrOtherReferenceEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { Guid.NewGuid().ToString(), "testPlant" }
        ];
        
        yield return
        [
            typeof(SwcrQuery),
            typeof(ISwcrEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(SwcrSignatureQuery),
            typeof(ISwcrSignatureEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(SwcrTypeQuery),
            typeof(ISwcrTypeEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];
        
        yield return
        [
            typeof(TagEquipmentQuery),
            typeof(ITagEquipmentEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];
        
        yield return
        [
            typeof(TagQuery),
            typeof(ITagEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return 
        [
        typeof(TagDocumentQuery),
            typeof(ITagDocumentEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { Guid.NewGuid().ToString(), "testPlant" }
        ];
        
        yield return
        [
            typeof(TaskQuery),
            typeof(ITaskEventV1),
            new[] { typeof(long?), typeof(string) },
            new object[] { 1L, "testPlant" }
        ];
        
        yield return
        [
            typeof(WorkOrderChecklistQuery),
            typeof(IWorkOrderChecklistEventV1),
            new[] { typeof(long?), typeof(long?), typeof(string) },
            new object[] { 1L, 2L, "testPlant" }
        ];
        
        yield return
        [
            typeof(WorkOrderCutoffQuery),
            typeof(IWorkOrderCutoffEventV1),
            new[] { typeof(long?),typeof(string), typeof(string),typeof(string),typeof(IEnumerable<long>) },
            new object[] { 123L, "testPlant", null, null, null}
        ];
        
        yield return
        [
            typeof(WorkOrderMaterialQuery),
            typeof(IWorkOrderMaterialEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];
        
        yield return
        [
            typeof(WorkOrderMilestoneQuery),
            typeof(IWorkOrderMilestoneEventV1),
            new[] { typeof(long?), typeof(long?), typeof(string) },
            new object[] { 1L, 2L, "testPlant" }
        ];
        
        yield return
        [
            typeof(WorkOrderQuery),
            typeof(IWorkOrderEventV1),
            new[] { typeof(long?), typeof(string), typeof(string) },
            new object[] { 1L, "testPlant", "noExtra" }
        ];

        yield return
        [
            typeof(HeatTracePipeTestQuery),
            typeof(IHeatTracePipeTestEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];
        
        yield return
        [
            typeof(NotificationWorkOrderQuery),
            typeof(INotificationWorkOrderEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];

        yield return
        [
            typeof(NotificationCommPkgOtherQuery),
            typeof(INotificationCommPkgEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];

        yield return
        [
            typeof(NotificationCommPkgBoundaryQuery),
            typeof(INotificationCommPkgEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];

        yield return
        [
            typeof(NotificationSignatureQuery),
            typeof(INotificationSignatureEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];

        yield return
        [
            typeof(PunchPriorityLibraryRelationQuery),
            typeof(IPunchPriorityLibRelationEventV1),
            new[] { typeof(string), typeof(string) },
            new object[] { "abc", "testPlant" }
        ];


    }
}