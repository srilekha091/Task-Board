To run this project,

1. Open Microsoft SQL Server Management Studio on you machine
2. Right-click on Databases and click "New Database.." and provide a database name.
3. Then in the project solution, open Web.config file and replace the name of the database to the one created above in the connectionstring.
4. For example, the current connection string in Web.config is 
<add name="DBCS" connectionString="Server=localhost;Database=TaskBoard;Trusted_Connection=True; integrated security=SSPI" providerName="System.Data.SqlClient"/>

Replace "TaskBoard" with the name of the database created in step 2.