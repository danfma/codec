using System.Data;
using DbUp;
using Npgsql;
using Samples.Person.Persistence;
using Testcontainers.PostgreSql;

namespace Codec.Sample.Tests;

public class PersonProjectTest(PgFixture fixture, ITestOutputHelper output)
    : IClassFixture<PgFixture>
{
    [Fact]
    public async Task Process()
    {
        var dbConnection = fixture.Connection;
        var personService = new PersonService(dbConnection);

        var name = "Daniel";
        var cpf = Cpf.Create("99398311100");
        var email = Email.Create("danfma@gmail.com");
        var cancellationToken = TestContext.Current.CancellationToken;

        await personService.AddIndividual(name, cpf, email, cancellationToken);
        await personService.AddOrganization(
            "Apple Inc",
            "Apple",
            Cnpj.Create("12345678901234"),
            cancellationToken
        );

        var people = await personService.List(cancellationToken);

        foreach (var person in people)
        {
            output.WriteLine($"{person.GetType().Name}@{person.Id.Value}: {person.Name}");
        }
    }
}

public sealed class PgFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("codec-samples")
        .Build();

    private IDbConnection? _connection;

    public IDbConnection Connection
    {
        get
        {
            return _connection
                ?? throw new ArgumentNullException(
                    nameof(Connection),
                    "Connection was not initialized"
                );
        }
        private set => _connection = value;
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        _connection = new NpgsqlConnection(_container.GetConnectionString());

        var upgrader = DeployChanges
            .To.PostgresqlDatabase(_container.GetConnectionString())
            .WithScriptsFromFileSystem("Samples/Person/migrations/postgres/")
            .LogToTrace()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception("PostgreSQL database update failed");
        }
    }

    public async ValueTask DisposeAsync()
    {
        _connection?.Dispose();
        _connection = null;

        await _container.DisposeAsync();
    }
}
