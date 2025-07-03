using Codec.Cli.Commands;

var app = ConsoleApp.Create().ConfigureLogging(x => x.AddZLoggerConsole());

app.Add<CodeCommand>("code");
app.Add<MigrationCommand>("migration");
app.Add<DatabaseCommand>("database");

await app.RunAsync(args);
