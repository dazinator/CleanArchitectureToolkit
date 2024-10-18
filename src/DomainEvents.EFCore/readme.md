## Domain Events

Demonstrate domain event patterns with EF Core.

## Overview

- Whilst using the EF Core models, they have methods that internally create "domain events".
- The Domain Events are then processed when `SaveChanges` is called on the DbContext.
- Handlers can then be implemented to respond to those events.

## Handler Types

- **Participating**.
  - A participating handler gets to participate within the current DbContext database transaction at the point SaveChanges is called and the event is processed.
  - If this type of handler causes an exception, it will cause the overall transaction / `SaveChanges` to fail.
  - Use this type of handler when you need seperate areas of the domain to participate atomically in the same transaction.
- **Eventually consistent**
  - An Eventually consistent handler will be invoked **after** the transaction that generated the Domain Event it is responding to, has completed.
  - If this type of handler fails, there must be some path to recovery else the domain could be in an inconsistent state.
    - e.g fix the bug with a new deployment and reprocess the domain event.
  - Use this type of handler when its not critical that the subsystem of the domain update immediately but can become "eventually consistent". This will help to keep database transactions smaller and less risk of a side effect failing a transaction deemed more critical.