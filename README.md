# Bounded context generator 🔣 -> 📝

## Description

This repository is a *proof of concept* to generate a **Bounded Context Canvas** directly from source code.

The idea is to analyse **symbols and annotations** in order to detects DDD tactical patterns,
(commands, aggregate, entity, value object, domain event, integration event, ... ) and draw diagrams
and text description.

This generated documentation **lives with the code** and is a support to **communicate** with teams and domain experts.

## Command line generation

Current :

    ./BoundedContextGenerator.exe path/to/sln

Later :
    
    dotnet tool install --global BoundedContextCanvasGenerator
    dotnet tool run BoundedContextCanvasGenerator /path/to/sln

## Configuration

The configuration will be in yaml format.

    description:
        - assemblyinfo
            - description
    command:
        - class
        - implementing
            - SomeNamespace.ICommand
    domain-event:
        - class
        - implementing
            - SomeNamespace.IDomainEvent
    aggregate:
        - class
        - extending
            - SomeNamespace.AggregateRoot

## Definition

### DDD
Domain-driven design is a software design approach focusing **on modelling software 
to match a domain** according to input from that domain's experts. In terms of object-oriented programming 
it means that the structure and language of software code **should match the business domain**.

### Bounded context
In Domain Driven Design (DDD), a bounded context is a sub-system in a software architecture 
aligned to a part of your domain.

More links:
- https://martinfowler.com/bliki/BoundedContext.html

### Canvas
A Bounded Context Canvas is a collaborative tool for designing and documenting 
the design of a single bounded context. It might also represent context interactions in order to
detail teams interaction.

Links:
- https://github.com/ddd-crew/bounded-context-canvas


