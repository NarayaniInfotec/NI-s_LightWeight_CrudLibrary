DataLite

DataLite is a lightweight, database-agnostic CRUD utility library for .NET.
It provides simple Insert, Update, Delete, and Select operations without requiring an ORM or database-specific code.

Designed for:

Web APIs

WinForms / WPF

Console applications

Microservices

Reporting tools

✨ Key Features

Database-agnostic (via DbProviderFactories)

Parameterized SQL (safe from injection)

Minimal configuration

No ORM dependency

Optional logging

Stored procedure support

Transaction support

Suitable for small to medium applications

📦 Installation
dotnet add package MyCompany.DataLite

⚙️ Basic Configuration
var config = new DbConfig
{
    ProviderName = "Microsoft.Data.SqlClient",
    ConnectionString = "your-connection-string"
};

IDatabase database = new Database(config);
ICrudRepository crud = new CrudRepository(database);

🗂 Table Name Best Practice

Always define table names as constants to avoid SQL injection risks.

public static class DbTables
{
    public const string Members = "Members";
}

➕ INSERT
crud.Insert(
    DbTables.Members,
    new Dictionary<string, object>
    {
        { "Name", "Nilesh" },
        { "Email", "test@email.com" },
        { "CreatedOn", DateTime.UtcNow }
    });

Notes

Keys = column names

Values = column values

null values are converted to DBNull.Value

✏️ UPDATE
crud.Update(
    DbTables.Members,
    new Dictionary<string, object>
    {
        { "Email", "updated@email.com" },
        { "UpdatedOn", DateTime.UtcNow }
    },
    "MemberId = @MemberId",
    new Dictionary<string, object>
    {
        { "@MemberId", 10 }
    });

Notes

Only provided fields are updated

WHERE clause must be parameterized

❌ DELETE
crud.Delete(
    DbTables.Members,
    "MemberId = @MemberId",
    new Dictionary<string, object>
    {
        { "@MemberId", 10 }
    });

Notes

Always use a WHERE clause

Prevents accidental mass deletion

🔍 SELECT
DataTable table = crud.Select(
    DbTables.Members,
    columns: "MemberId, Name, Email",
    whereClause: "IsActive = @IsActive",
    whereParams: new Dictionary<string, object>
    {
        { "@IsActive", true }
    });

Select All Rows
DataTable table = crud.Select(DbTables.Members);

🧪 Stored Procedure Execution
var spExecutor = new StoredProcedureExecutor(database);

DataTable result = spExecutor.Execute(
    "GetActiveMembers",
    new Dictionary<string, object>
    {
        { "@DepartmentId", 2 }
    });

### Insert And Return Identity

```csharp
int id = db.InsertAndReturnId(
    "Datasets",
    new Dictionary<string, object>
    {
        ["DatasetName"] = "Finance Report",
        ["Year"] = 2025,
        ["Department"] = "Accounts"
    }
);

🔁 Transaction Support
using var scope = new DbTransactionScope(database);

crud.Insert(DbTables.Orders, orderFields);
crud.Insert(DbTables.OrderItems, itemFields);

scope.Commit();

🧾 Logging (Optional)

Implement IDbLogger to trace SQL execution:

public class ConsoleDbLogger : IDbLogger
{
    public void Log(string sql, Dictionary<string, object> parameters)
    {
        Console.WriteLine(sql);
    }
}


Pass it to CrudRepository if required.

🔐 Security Notes

Table names must come from trusted code (constants)

Values are always parameterized

Avoid passing raw user input as identifiers

🧠 When to Use DataLite

✔ Simple CRUD needs
✔ Reporting systems
✔ Legacy databases
✔ Lightweight services

❌ Complex domain modeling
❌ Heavy object tracking
❌ LINQ-based querying

📄 License

MIT License

🚀 Roadmap

Async CRUD methods

Pagination helpers

Insert-and-return-id support

Soft delete utilities

🤝 Contribution

Contributions, issues, and suggestions are welcome.