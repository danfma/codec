# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Codec** is a domain-specific language (DSL) compiler for defining data layer schemas. The project generates database schemas, type-safe client code, and migration scripts from `.codec` files.

## Development Commands

### Build & Test
```bash
# Build the solution
dotnet build

# Format code with CSharpier
dotnet csharpier .

# Clean build artifacts
dotnet clean
```

### Working with Dependencies
```bash
# Restore packages
dotnet restore

# Add package to specific project
dotnet add Codec.Core package PackageName
```

## Architecture

### Core Components
- **Codec.Core**: Main library containing AST definitions and core types
- **AST Namespace**: Contains fundamental type system definitions including:
  - `TypeInfo`: Built-in primitive types (Bool, String, Int8-64, UInt8-64, Date, DateTime, Decimal, etc.)
  - `TypeAlias`: Named type definitions with constraints/annotations
  - `ComponentDefinition`: Embeddable structures for entities
  - `TraitDefinition`: Reusable mixins for entities/views
  - `Annotation`: Constraint system (@max, @min, @regex, etc.)

### Language Features
The Codec language supports:
- **Entities**: Database table definitions with trait implementation
- **Components**: Embeddable field groups (flattened into parent table)
- **Traits**: Reusable field/constraint mixins
- **Views**: Read-only queries with CSL syntax
- **Services**: Function groupings for business logic
- **Type Aliases**: Strong typing to prevent primitive obsession

### Dependencies
- **Pidgin**: Parser combinator library for language parsing
- **CSharpier**: Code formatting tool (configured in dotnet-tools.json)

## Type System Notes
- All type names use PascalCase (entities, traits, components, primitives)
- All identifiers use snake_case (properties, parameters, function names)
- Strong emphasis on type safety with value objects generated from type aliases
- Rich annotation system for validation and database constraints