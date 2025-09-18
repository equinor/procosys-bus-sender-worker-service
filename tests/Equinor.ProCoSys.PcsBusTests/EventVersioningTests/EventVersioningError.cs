namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

public static class EventVersioningError
{
    public static string ErrorMessage = @"If this tests fails, its most likely because the versioning contract is breached. Consider creating a new version instead of modifying the existing one when making breaking changes. 
                                        If new properties are added to the interface (non breaking), this test should be updated with the new properties.";
}