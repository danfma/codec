# Codec Language Specification

## 1. Vision & Overview

**Codec** is a domain-specific language for defining a project's entire data layer from a single, authoritative source of truth. It is designed to model data structures, relationships, and business logic in a way that is portable across multiple databases and programming languages.

By defining your schema and operations once in `.codec` files, you can generate:

- **Database Schemas:** For relational databases like PostgreSQL, SQL Server, etc.
- **Type-Safe Client Code:** Idiomatic models and data access services for languages like C#, TypeScript, Go, and more.
- **Database Migrations:** Safe, reviewable, and version-controlled migration scripts.

The core philosophy of Codec is to provide a clear, declarative, and highly organized system that eliminates drift and duplication between the database and the backend.

## 2. Core Language Features

- **Database Agnostic Model**: Define your data model once for multiple database systems.
- **Rich, Composable Structures**: First-class support for `entity`s, `component`s, `view`s, `trait`s, `enum`s, and `type` aliases.
- **Polymorphic Entities**: Model "is-a" relationships using `abstract entity` with single-table or joined-table inheritance strategies.
- **Expressive Relationships**: Annotation-based syntax for relationships.
- **Modern Scripting Language (CSL)**: A powerful, SQL-inspired language for both querying and procedural logic.
- **Type Safety & Strong Typing**: Prevent common errors like primitive obsession by generating value objects from `type` aliases.
- **Organized & Reusable Logic**: Group `func`tions into `service`s for a clean, modular architecture that translates to both database and client code.

## 3. Language Syntax & Abstractions

### 3.1. General Conventions

- **Casing**: All type names (`entity`, `trait`, `component`, `service`, `enum`, `type`) use `PascalCase`. All identifiers (properties, parameters, function names) use `snake_case`.
- **Keywords**: Keywords are lowercase (e.g., `from`, `select`, `insert`).
- **File Extension**: Codec definition files use the `.codec` extension.
- **Mutability**: Fields are declared with `val` (immutable) or `var` (mutable).

### 3.2. Top-Level Declarations

A `.codec` file can contain the following top-level declarations: `config`, `trait`, `entity`, `abstract entity`, `view`, `service`, `enum`, and `type`.

### 3.3. Structural Blueprints

#### Type Aliases

Creates a named, reusable definition from a primitive type, applying specific constraints via annotations. The compiler generates a distinct, strongly-typed value object (e.g., a C# `record struct`) to enforce type safety.

```codec
type Email = String {
  constraint min_len(1)
  constraint max_len(120)
  constraint regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
}

type ZipCode = String {
  constraint max_len(10)
}
```

#### Traits

A `trait` is a reusable "mixin" of fields and constraints that can be implemented by `entity`s or `view`s.

```codec
trait Identifiable {
  val id: Uuid = Uuid::new()
}

trait Timestamped {
  val created_at: DateTime = DateTime::utc_now()
  var updated_at: DateTime?

  on(update) {
    updated_at = DateTime::utc_now()
  }
}
```

### 3.4. Data Structures

#### Entities

An `entity` represents a database table. It can implement traits and define fields with inline constraints.

```codec
entity User : Identifiable, Timestamped {
  var email: Email {
    constraint unique
  }
  
  // Inverse side of a one-to-many relationship
  posts: List<Post> mapped by Post.author
}

entity Post : Identifiable, Timestamped {
  var title: String {
    constraint max_len(200)
  }
  val author_id: Uuid

  // "Owning" side of a many-to-one relationship
  author: User {
    constraint foreign_key(author_id)
    constraint on_delete(cascade)
  }
}
```

#### Polymorphic Entities (Inheritance)

Model "is-a" hierarchies using `abstract entity`. The inheritance discriminator is defined using the `inheritance discriminator` annotation on a field.

```codec
trait Named {
    var name: String
}

abstract entity Person : Named {
    val type: String {
      constraint max_length(12)
      constraint one_of("Individual", "Organization")
      inheritance discriminator
    }
}

// The value for the discriminator is set in the declaration.
entity Individual : Person(type = "Individual") {
    val cpf: Cpf {
        constraint unique
    }
    var email: Email {
        constraint unique
    }
}

entity Organization : Person(type = "Organization") {
    var doing_business_as: String
    var cnpj: Cnpj {
        constraint unique
    }
}
```

#### Views

A `view` is a named, read-only query defined using CSL.

```codec
view ActiveUserDetails : Identifiable {
  email: Email
  post_count: Int64

  query = from User u
          where u.is_deleted = false
          select {
            u.id,
            u.email,
            post_count: count(u.posts)
          }
}
```

### 3.5. Services and Functions

A `service` is a logical grouping of `func`tions.

```codec
service PersonService {
    // A query that returns a single, non-nullable result.
    func find_by_id(id: Uuid) = codec {
        return from Person p
               where p.id = id
               select p
               single or error("Person not found")
    }

    // A function with a procedural CSL script.
    func add_individual(name: String, email: Email) = codec {
        let individual = insert Individual {
            name: name,
            email: email,
        }
        return individual.id
    }

    // A function calling another function in the same service.
    func deactivate_user(id: Uuid) = codec {
        let user = self.find_by_id(id)
        if user != null {
            delete Person p where p.id = id
        }
    }
}
```

## 4. CSL (Codec Shaping Language)

CSL is the primary language for writing logic in `codec` blocks.

### 4.1. Querying

- **Syntax:** `from... [join]... [where]... select...`
- **Cardinality:** Queries return `List<T>` by default. Suffix the query with a cardinality keyword to change the result:
  - `single or null`: Returns `T?`.
  - `single or error("message")`: Returns `T`. Throws an error if not exactly one result.
  - `first or null`: Returns `T?`.
- **Shaping:** The `select { ... }` block defines the result shape.

### 4.2. Procedural Scripting (within `codec` blocks)

- **Variables:**
  - `let name = value`: Declares an immutable constant.
  - `var name = value`: Declares a mutable variable.
- **DML as Expressions:** `insert`, `update`, `delete` statements can return the data they affected using a `returning` clause.
- **Control Flow:** `if condition { ... } else { ... }` and `for item in collection { ... }`.
- **Transactions:** The `transaction { ... }` block ensures all operations within it are atomic.
- **Function Calls:**
  - `self.func_name(args)`: Calls another function within the same service.
  - `ServiceName.func_name(args)`: Calls a function in an external service.

### 4.2.1. Advanced CSL Features

- **Anonymous Structs:** For convenience, you can define ad-hoc structures for parameters without creating a formal `component`.

  ```codec
  // Use an anonymous struct for a collection parameter
  func add_all(entries: List<{ name: String, email: Email }>) = codec {
    for entry in entries {
      self.add(entry.name, entry.email)
    }
  }
  ```

- **Null-Coalescing Operator (`??`):** Provides a clean way to handle nullable values. It returns the left-hand operand if it's not null; otherwise, it returns the right-hand operand.

  ```codec
  // Use `??` to update a field only if a new value is provided
  update Individual {
      name: name ?? .name,
      email: email ?? .email
  } where .id = id
  ```

- **Parameter Injection (`@`):** The `@` prefix is used to inject parameters into `native` queries. In CSL `codec` blocks, it is optional and primarily used to disambiguate a parameter from an entity field with the same name.

  ```codec
  // `id` is a parameter. It could be written as `@id` for clarity.
  update Individual {
      name: name ?? .name
  } where .id = id

  // In native SQL, the '@' is required.
  func update_individual(...) = native("sql") for "postgres" {
      """
      UPDATE individual SET name = @name WHERE id = @id
      """
  }
  ```

### 4.3. Native Code

Use `func ... = native(<dialect>) for <db> { ... }` for dialect-specific code. Use `#{...}` for name injection and `@...` for parameter injection.

## 5. Migration System

The Codec CLI provides a robust migration system to manage database schema evolution safely.

- **Philosophy:** The `.codec` schema is the source of truth. Migrations are declarative and non-destructive by default.
- **Workflow:**
  1.  Modify your `.codec` files.
  2.  Run `codec migration add <name>` to generate a timestamped, reviewable SQL migration script.
  3.  Run `codec migration apply` to apply pending migrations to the database.
- **Introspection & Drift:**
  - `codec db pull`: Reverse-engineers an existing database into a starting set of `.codec` files.
  - `codec db check`: Detects "drift" between the live database and the expected schema state.

## 6. Data Types & Keywords

### 6.1. Data Types

A rich set of primitive types is provided, including `String`, `Text`, `Int8`-`Int64`, `UInt8`-`UInt64`, `Decimal`, `Bool`, `DateTime`, `Date`, `Uuid`, and `Json`. All type names use `PascalCase`.

### 6.2. Field-Level Modifiers & Constraints

Field behavior is defined using mutability keywords and inline annotations.

- **Mutability:**
  - `val`: Declares an immutable field (read-only after creation).
  - `var`: Declares a mutable field.
  - `?`: Marks a field as nullable (e.g., `var name: String?`).
  - `= <value>`: Provides a default value (e.g., `Uuid::new()`, `true`, `100`).

- **Constraints (Annotations):**
  - `constraint min_len(n)` / `constraint max_len(n)`: String/collection length constraints.
  - `constraint min(n)` / `constraint max(n)`: Numeric range constraints.
  - `constraint regex(pattern)`: Regex validation for strings.
  - `constraint unique`: Ensures the value is unique in the table.
  - `constraint foreign_key(field)`: Defines a foreign key relationship.
  - `constraint on_delete(action)`: Specifies the action on delete (`cascade`, `set_null`, etc.).
  - `constraint one_of(values...)`: Restricts field values to a specific set of options.
  - `inheritance discriminator`: Marks a field as the discriminator for polymorphic inheritance.

## 7. Generated Code Examples

This section provides examples of how Codec `service` functions can be translated into dialect-specific SQL, such as PL/pgSQL for PostgreSQL.

### 7.1. `add_all` Function

The `add_all` function in CSL takes a `List` of anonymous structs. In PostgreSQL, this can be implemented by accepting a `JSONB` array and iterating over it.

**CSL:**

```codec
// service PersonStorage
func add_all(entries: List<{ name: String, email: Email }>) = codec {
  for entry in entries {
    insert Individual i {
        i.name = entry.name,
        i.email = entry.email
    }
  }
}
```

**Generated PL/pgSQL:**

```sql
CREATE OR REPLACE FUNCTION add_all_persons(entries JSONB)
RETURNS VOID AS $
DECLARE
    entry JSONB;
BEGIN
    -- Validate that the input is a JSON array
    IF NOT jsonb_typeof(entries) = 'array' THEN
        RAISE EXCEPTION 'Input must be a JSONB array';
    END IF;

    -- Loop over each object in the JSONB array
    FOR entry IN SELECT * FROM jsonb_array_elements(entries)
    LOOP
        -- Insert data into the Person table, extracting values from JSON
        INSERT INTO "Person" ("id", "name", "email")
        VALUES (
            gen_random_uuid(), -- Corresponds to Uuid::new() in Codec
            entry->>'name',
            entry->>'email'
        );
    END LOOP;
END;
$ LANGUAGE plpgsql;
```

### 7.2. `update` Function

The CSL `update` function uses the null-coalescing operator (`??`) to conditionally update fields. This translates directly to the `COALESCE` function in SQL.

**CSL:**

```codec
// service PersonStorage
func update(id: Uuid, name: String?, email: Email?) = codec {
    update Person {
      name: name ?? .name,
      email: email ?? .email
    } where .id = id
}
```

**Generated PL/pgSQL:**

```sql
CREATE OR REPLACE FUNCTION update_person(
    p_id UUID,
    p_name TEXT DEFAULT NULL,
    p_email TEXT DEFAULT NULL
)
RETURNS VOID AS $
BEGIN
    UPDATE "Person"
    SET
        -- COALESCE returns the first non-null argument.
        -- If p_name is null, the existing "name" value is kept.
        "name" = COALESCE(p_name, "name"),

        -- If p_email is null, the existing "email" value is kept.
        "email" = COALESCE(p_email, "email")
    WHERE
        "id" = p_id;
END;
$ LANGUAGE plpgsql;
```
