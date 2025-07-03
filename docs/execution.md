# Execution

## Commands

- `codec migration add <name>`: Adds a new migration file to the project;
- `codec migration remove`: Remove the last migration file from the project;
- `codec migration up [<version>]`: Apply all pending migrations to the database. If version is specified, all migrations to that version will be applied;
- `codec migration down [<version>]`: Rollback all applied migrations to the database. If version is specified, all migrations to that version will be rolled back;
- `codec migration status`: Show the status of the migrations (up, down, pending);
