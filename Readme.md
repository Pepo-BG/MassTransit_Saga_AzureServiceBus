# Installation

* Set your connection string in appsettings.json -> `ConnectionStrings:StateDB`,
* Set your Azure Service Bus host in appsettings.json -> `ServiceBusHost`.

# Migrations
To create the database, open Package Manager console and execute `Update-Database -Context StateDbContext`