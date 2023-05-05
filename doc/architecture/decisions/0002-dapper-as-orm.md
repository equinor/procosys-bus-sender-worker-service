# 2. Dapper as ORM

Date: 2023-05-05

## Status

Accepted

## Context

We are moving from queries returning a finnished json string, to a query that should map to typed objects to then be serialized. 

## Decision

We have decided to use Dapper on top of Entity framework as it is simple and supports writing queries directly. This is something the team should be used to as its close to how hbm.xml (NHibernate) files work in ProCoSys Main. 

We will not change the code that use EF core to find Busevent, as its working fine and also is used for writing.

## Consequences

No longer needed to "Wash" strings of special caracters as this will be handler by Serialzing in code.
Type safety, clear contracts and code readability should be easier to create and maintain as we move away from building json with sql queries. Using Entity framework was also considered, but Dapper seems easier with the current version of EF Core. 
