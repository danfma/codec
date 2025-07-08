using System.Data;
using System.Diagnostics;
using Dapper;
using InterpolatedSql.Dapper;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Samples.PersonManagement.Persistence;

public readonly record struct Email(string Value)
{
    public static Email Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Email cannot be null or empty", nameof(value));

        if (value.Length < 1 || value.Length > 120)
            throw new ArgumentException(
                "Email must be between 1 and 120 characters",
                nameof(value)
            );

        if (
            !System.Text.RegularExpressions.Regex.IsMatch(
                value,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
            )
        )
            throw new ArgumentException("Invalid email format", nameof(value));

        return new(value);
    }
}

public readonly record struct Cpf(string Value)
{
    public static Cpf Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("CPF cannot be null or empty", nameof(value));

        if (value.Length != 11)
            throw new ArgumentException("CPF must be exactly 11 characters", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]{11}$"))
            throw new ArgumentException("CPF must contain only digits", nameof(value));

        return new(value);
    }
}

public readonly record struct Cnpj(string Value)
{
    public static Cnpj Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("CNPJ cannot be null or empty", nameof(value));

        if (value.Length != 14)
            throw new ArgumentException("CNPJ must be exactly 14 characters", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9]{14}$"))
            throw new ArgumentException("CNPJ must contain only digits", nameof(value));

        return new(value);
    }
}

public interface INamed
{
    public string Name { get; set; }
}

public readonly record struct PersonId(Guid Value)
{
    public static readonly PersonId Empty = new(Guid.Empty);

    public static PersonId Create(Guid value) => new(value);

    public static implicit operator Guid(PersonId id) => id.Value;
}

public abstract class Person : INamed
{
    public PersonId Id { get; set; } = PersonId.Empty;
    public required string Name { get; set; }
}

public sealed class Individual : Person
{
    public required Cpf Cpf { get; set; }
    public required Email Email { get; set; }
}

public sealed class Organization : Person
{
    public required string DoingBusinessAs { get; set; }
    public required Cnpj Cnpj { get; set; }
}

public interface IPersonService
{
    ValueTask<PersonId> AddIndividual(
        string name,
        Cpf cpf,
        Email email,
        CancellationToken cancellationToken = default
    );

    ValueTask<PersonId> AddOrganization(
        string name,
        string doingBusinessAs,
        Cnpj cnpj,
        CancellationToken cancellationToken = default
    );

    ValueTask UpdateIndividual(
        PersonId id,
        string? name,
        Email? email,
        CancellationToken cancellationToken = default
    );

    ValueTask UpdateOrganization(
        PersonId id,
        string? name,
        string? doingBusinessAs,
        Cnpj? cnpj,
        CancellationToken cancellationToken = default
    );

    ValueTask Remove(PersonId id, CancellationToken cancellationToken = default);

    ValueTask<Person> FindById(PersonId id, CancellationToken cancellationToken = default);

    ValueTask<Individual?> FindIndividualByEmail(
        Email email,
        CancellationToken cancellationToken = default
    );

    ValueTask<List<Person>> List(CancellationToken cancellationToken = default);
}

public sealed partial class PersonService(IDbConnection connection, ILogger<PersonService> logger)
    : IPersonService
{
    private sealed record PersonRow(
        Guid Id,
        string Name,
        string Type,
        string? Cpf,
        string? Email,
        string? DoingBusinessAs,
        string? Cnpj
    );

    private sealed record IndividualRow(Guid Id, string Name, string Cpf, string Email);

    public async ValueTask<PersonId> AddIndividual(
        string name,
        Cpf cpf,
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        var start = Stopwatch.GetTimestamp();
        var id = Guid.CreateVersion7();

        var query = connection.SqlBuilder(
            $"""
            SELECT id
            FROM person_service_add_individual({name}, {cpf.Value}, {email.Value}, {id})
            LIMIT 1
            """
        );

        var result = await query.QuerySingleAsync<Guid>(cancellationToken: cancellationToken);

        LogExecutionOfCommandTookTimeMs(
            logger,
            nameof(AddIndividual),
            Stopwatch.GetElapsedTime(start)
        );

        return PersonId.Create(result);
    }

    public async ValueTask<PersonId> AddOrganization(
        string name,
        string doingBusinessAs,
        Cnpj cnpj,
        CancellationToken cancellationToken = default
    )
    {
        var start = Stopwatch.GetTimestamp();
        var id = Guid.CreateVersion7();

        var query = connection.SqlBuilder(
            $"""
            SELECT id
            FROM person_service_add_organization({name}, {doingBusinessAs}, {cnpj.Value}, {id})
            LIMIT 1
            """
        );

        var result = await query.QuerySingleAsync<Guid>(cancellationToken: cancellationToken);

        LogExecutionOfCommandTookTimeMs(
            logger,
            nameof(AddOrganization),
            Stopwatch.GetElapsedTime(start)
        );

        return PersonId.Create(result);
    }

    public async ValueTask UpdateIndividual(
        PersonId id,
        string? name,
        Email? email,
        CancellationToken cancellationToken = default
    )
    {
        var start = Stopwatch.GetTimestamp();

        var query = connection.SqlBuilder(
            $"""
            SELECT person_service_update_individual({id.Value}, {name}, {email?.Value})
            """
        );

        await query.ExecuteAsync(cancellationToken: cancellationToken);

        LogExecutionOfCommandTookTimeMs(
            logger,
            nameof(UpdateIndividual),
            Stopwatch.GetElapsedTime(start)
        );
    }

    public async ValueTask UpdateOrganization(
        PersonId id,
        string? name,
        string? doingBusinessAs,
        Cnpj? cnpj,
        CancellationToken cancellationToken = default
    )
    {
        var start = Stopwatch.GetTimestamp();

        var query = connection.SqlBuilder(
            $"""
            SELECT person_service_update_organization({id.Value}, {name}, {doingBusinessAs}, {cnpj?.Value})
            """
        );

        await query.ExecuteAsync(cancellationToken: cancellationToken);

        LogExecutionOfCommandTookTimeMs(
            logger,
            nameof(UpdateOrganization),
            Stopwatch.GetElapsedTime(start)
        );
    }

    public async ValueTask Remove(PersonId id, CancellationToken cancellationToken = default)
    {
        var start = Stopwatch.GetTimestamp();

        var query = connection.SqlBuilder($"SELECT person_service_remove({id.Value})");

        await query.ExecuteAsync(cancellationToken: cancellationToken);

        LogExecutionOfCommandTookTimeMs(logger, nameof(Remove), Stopwatch.GetElapsedTime(start));
    }

    public async ValueTask<Person> FindById(
        PersonId id,
        CancellationToken cancellationToken = default
    )
    {
        var start = Stopwatch.GetTimestamp();

        var query = connection.SqlBuilder(
            $"""
            SELECT 
                id AS "Id",
                name AS "Name",
                type AS "Type",
                cpf AS "Cpf",
                email AS "Email",
                doing_business_as AS "DoingBusinessAs",
                cnpj AS "Cnpj"
            FROM person_service_find_by_id({id.Value})
            """
        );

        var result = await query.QuerySingleOrDefaultAsync<PersonRow>(
            cancellationToken: cancellationToken
        );

        if (result == null)
            throw new InvalidOperationException("Person not found");

        LogExecutionOfCommandTookTimeMs(logger, nameof(FindById), Stopwatch.GetElapsedTime(start));

        return result.Type switch
        {
            nameof(Individual) => new Individual
            {
                Id = PersonId.Create(result.Id),
                Name = result.Name,
                Cpf = new Cpf(result.Cpf.MustBeNotNull()),
                Email = new Email(result.Email.MustBeNotNull()),
            },
            nameof(Organization) => new Organization
            {
                Id = PersonId.Create(result.Id),
                Name = result.Name,
                DoingBusinessAs = result.DoingBusinessAs.MustBeNotNull(),
                Cnpj = new Cnpj(result.Cnpj.MustBeNotNull()),
            },
            _ => throw new InvalidOperationException($"Unknown person type: {result.Type}"),
        };
    }

    public async ValueTask<Individual?> FindIndividualByEmail(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        var start = Stopwatch.GetTimestamp();

        var query = connection.SqlBuilder(
            $"""
            SELECT 
                id AS "Id",
                name AS "Name",
                cpf AS "Cpf",
                email AS "Email"
            FROM person_service_find_individual_by_email({email.Value})
            """
        );

        var result = await query.QuerySingleOrDefaultAsync<IndividualRow>(
            cancellationToken: cancellationToken
        );

        LogExecutionOfCommandTookTimeMs(
            logger,
            nameof(FindIndividualByEmail),
            Stopwatch.GetElapsedTime(start)
        );

        if (result == null)
            return null;

        return new Individual
        {
            Id = PersonId.Create(result.Id),
            Name = result.Name,
            Cpf = new Cpf(result.Cpf),
            Email = new Email(result.Email),
        };
    }

    public async ValueTask<List<Person>> List(CancellationToken cancellationToken = default)
    {
        var start = Stopwatch.GetTimestamp();

        var query = connection.SqlBuilder(
            $"""
            SELECT 
                id AS "Id",
                name AS "Name",
                type AS "Type",
                cpf AS "Cpf",
                email AS "Email",
                doing_business_as AS "DoingBusinessAs",
                cnpj AS "Cnpj"
            FROM person_service_list()
            """
        );

        var results = await query.QueryAsync<PersonRow>(cancellationToken: cancellationToken);
        var persons = new List<Person>();

        foreach (var result in results)
        {
            Person person = result.Type switch
            {
                nameof(Individual) => new Individual
                {
                    Id = PersonId.Create(result.Id),
                    Name = result.Name,
                    Cpf = new Cpf(result.Cpf!),
                    Email = new Email(result.Email!),
                },
                nameof(Organization) => new Organization
                {
                    Id = PersonId.Create(result.Id),
                    Name = result.Name,
                    DoingBusinessAs = result.DoingBusinessAs!,
                    Cnpj = new Cnpj(result.Cnpj!),
                },
                _ => throw new InvalidOperationException($"Unknown person type: {result.Type}"),
            };

            persons.Add(person);
        }

        LogExecutionOfCommandTookTimeMs(logger, nameof(List), Stopwatch.GetElapsedTime(start));

        return persons;
    }

    [LoggerMessage(LogLevel.Debug, "Execution of {Command} took {Time} ms")]
    private static partial void LogExecutionOfCommandTookTimeMs(
        ILogger<PersonService> logger,
        string command,
        TimeSpan time
    );
}

public static class ObjectExtensions
{
    public static T MustBeNotNull<T>(this T? target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return target;
    }
}
