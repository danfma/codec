using System.Data;
using InterpolatedSql.Dapper;

// ReSharper disable once CheckNamespace
namespace Samples.PersonManagement.Persistence;

public readonly record struct Email(string Value)
{
    public static Email Create(string value) => new(value);
}

public readonly record struct Cpf(string Value)
{
    public static Cpf Create(string value) => new(value);
}

public readonly record struct Cnpj(string Value)
{
    public static Cnpj Create(string value) => new(value);
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

public sealed class PersonService(IDbConnection connection) : IPersonService
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
        var id = Guid.CreateVersion7();
        var type = nameof(Individual);

        var query = connection.SqlBuilder(
            $"""
            WITH person_insert AS (
                INSERT INTO person (id, name, type)
                VALUES ({id}, {name}, {type})
                RETURNING id
            )
            INSERT INTO individual (id, cpf, email)
            SELECT id, {cpf.Value}, {email.Value}
            FROM person_insert
            RETURNING id
            """
        );

        var result = await query.QuerySingleAsync<Guid>(cancellationToken: cancellationToken);

        return PersonId.Create(result);
    }

    public async ValueTask<PersonId> AddOrganization(
        string name,
        string doingBusinessAs,
        Cnpj cnpj,
        CancellationToken cancellationToken = default
    )
    {
        var id = Guid.CreateVersion7();
        var type = nameof(Organization);

        var query = connection.SqlBuilder(
            $"""
            WITH person_insert AS (
                INSERT INTO person (id, name, type)
                VALUES ({id}, {name}, {type})
                RETURNING id
            )
            INSERT INTO organization (id, doing_business_as, cnpj)
            SELECT id, {doingBusinessAs}, {cnpj.Value}
            FROM person_insert
            RETURNING id
            """
        );

        var result = await query.QuerySingleAsync<Guid>(cancellationToken: cancellationToken);

        return PersonId.Create(result);
    }

    public async ValueTask UpdateIndividual(
        PersonId id,
        string? name,
        Email? email,
        CancellationToken cancellationToken = default
    )
    {
        var query = connection.SqlBuilder(
            $"""
            WITH person_update AS (
                UPDATE person 
                SET name = COALESCE({name}, name)
                WHERE id = {id.Value}
                RETURNING id
            )
            UPDATE individual 
            SET email = COALESCE({email?.Value}, email)
            WHERE id = {id.Value}
            AND EXISTS (SELECT 1 FROM person_update WHERE person_update.id = individual.id)
            """
        );

        await query.ExecuteAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask UpdateOrganization(
        PersonId id,
        string? name,
        string? doingBusinessAs,
        Cnpj? cnpj,
        CancellationToken cancellationToken = default
    )
    {
        var query = connection.SqlBuilder(
            $"""
            WITH person_update AS (
                UPDATE person 
                SET name = COALESCE({name}, name)
                WHERE id = {id.Value}
                RETURNING id
            )
            UPDATE organization 
            SET 
                doing_business_as = COALESCE({doingBusinessAs}, doing_business_as),
                cnpj = COALESCE({cnpj?.Value}, cnpj)
            WHERE id = {id.Value}
            AND EXISTS (SELECT 1 FROM person_update WHERE person_update.id = organization.id)
            """
        );

        await query.ExecuteAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask Remove(PersonId id, CancellationToken cancellationToken = default)
    {
        var query = connection.SqlBuilder($"DELETE FROM person WHERE id = {id.Value}");

        await query.ExecuteAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask<Person> FindById(
        PersonId id,
        CancellationToken cancellationToken = default
    )
    {
        var query = connection.SqlBuilder(
            $"""
            SELECT 
                p.id AS "Id",
                p.name AS "Name",
                p.type AS "Type",
                i.cpf AS "Cpf",
                i.email AS "Email",
                o.doing_business_as AS "DoingBusinessAs",
                o.cnpj AS "Cnpj"
            FROM person p
            LEFT JOIN individual i ON p.id = i.id
            LEFT JOIN organization o ON p.id = o.id
            WHERE p.id = {id.Value}
            """
        );

        var result = await query.QuerySingleOrDefaultAsync<PersonRow>(
            cancellationToken: cancellationToken
        );

        if (result == null)
            throw new InvalidOperationException("Person not found");

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
        var query = connection.SqlBuilder(
            $"""
            SELECT 
                p.id AS "Id",
                p.name AS "Name",
                i.cpf AS "Cpf",
                i.email AS "Email"
            FROM person p
            INNER JOIN individual i ON p.id = i.id
            WHERE i.email = {email.Value}
            LIMIT 1
            """
        );

        var result = await query.QuerySingleOrDefaultAsync<IndividualRow>(
            cancellationToken: cancellationToken
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
        var query = connection.SqlBuilder(
            $"""
            SELECT 
                p.id AS "Id",
                p.name AS "Name",
                p.type AS "Type",
                i.cpf AS "Cpf",
                i.email AS "Email",
                o.doing_business_as AS "DoingBusinessAs",
                o.cnpj AS "Cnpj"
            FROM person p
            LEFT JOIN individual i ON p.id = i.id
            LEFT JOIN organization o ON p.id = o.id
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

        return persons;
    }
}

public static class ObjectExtensions
{
    public static T MustBeNotNull<T>(this T? target)
    {
        ArgumentNullException.ThrowIfNull(target);

        return target;
    }
}
