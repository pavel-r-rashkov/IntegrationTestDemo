using System.Reflection;
using DbUp;

namespace Database;

public class Program
{
    public static void Main()
    {
        // var connectionString = "";
        // PerformUpgrade(connectionString);
    }

    public static void PerformUpgrade(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
			.SqlDatabase(connectionString)
			.LogToConsole()
			.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
			.Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
			throw new Exception("Database migration exception: {Error}", result.Error);
        }
    }
}