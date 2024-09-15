# Bridge

**Bridge** is a versatile library that offers three independent components for building robust, scalable, and flexible applications: **Bridge Bus**, **Bridge Storage**, and **Bridge Workflow**. Each component can be used separately or together, depending on your specific needs.

## Components

### Bridge Bus

- InMemory
- Azure Queue Storage

Stay tuned for updates...

### Bridge Storage

- InMemory
- MongoDB

Stay tuned for updates...

### Bridge Workflow

**Bridge Workflow** is an improved version of the [Workflow-Core](https://github.com/danielgerlag/workflow-core) library, providing advanced capabilities for workflow management. It focuses on enhancing performance, flexibility, and ease of use.

#### Key Improvements

- **Lock-Free Implementation**: Removes locking mechanisms to optimize performance and reduce processing delays.
- **Minimized Task Pooling**: Reduces resource consumption through efficient task management.
- **Decoupled Workflow Producer and Consumer**: Increases scalability by separating the production and consumption of workflows.

#### Roadmap

- **1.x.x**: Achieve full compatibility with [danielgerlag/workflow-core](https://github.com/danielgerlag/workflow-core).
- **2.x.x**: Transition from Azure Queue Storage to Azure Service Bus, while retaining Azure Queue Storage polling to facilitate easier migration.
- **3.x.x**: Move towards a fully lock-free architecture for the Bridge Workflow component and deprecate Azure Queue Storage support.

---

Stay tuned for future updates!
