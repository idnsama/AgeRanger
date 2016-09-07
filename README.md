This is an implementation of AgeRanger test project. See https://github.com/Debitsuccess/AgeRanger for details.

This is a single-page application with paginated person list display. Front-end and back-end validations are in place.
Tests cover all API controller methods and underlyind data layer methods. Local test copy of the SQLite database is used for testing.
Please be careful with website publishing as default empty copy of the database is set to overwrite the existing file in App_Data folder for testing convenience.

********************************************************************************************************************************************

WEB API:

/api/getFilteredList?filterString={filterString}&position={pos}&length={len}
Returns a portion of the person list.
Parameters:
{filterString}: sets a filter on the output. Only records with first or last name matching filterString will be returned. Empty filterString disables filter.
{pos}: position of the first element on the current page in the list. Defauls is 0.
{len}: length of the page. Defailt is 10.



/api/getTotalCount?filterString={filterString}
Returns a total count of records in the list.
Parameters:
{filterString}: sets a filter on the output. Only records with first or last name matching filterString will be counted. Empty filterString disables filter.


/api/postAddPerson
Adds a new person record. Returns true if successfull. Record must not match existing records. All fields in the model must present. Age must be a 0 or positive integer.
Data model: { firstName: {firstname}, lastName: {lastname}, age: {age} }


/api/postUpdatePerson
Adds a new person record. Returns true if successfull. Record must not match existing records. All fields in the model must present. Age must be a 0 or positive integer.
Data model: { id: {id}, firstName: {firstname}, lastName: {lastname}, age: {age} }


/api/getDeletePerson?id={id}
Deletes a record. Returns true if successfull.

********************************************************************************************************************************************


TODO:

0) Set up SQL database and test AgeRangerDataSQLServer (DAL class for SQL Server) on it. Project may be switched to use SQL Server via AppSetting DBType key and updating the AgeRangerDBConnectionString.

1) Add detailed error logging to convenient parseable storage (to database?). Possibly include client identification (IP etc).

2) Improve user-friendly error messages.

3) Rework GetTotalCount and GetTotalCountFiltered for increased responsiveness on a large Person table: add counting table or caching for total person count request.

4) Introduce efficient index-based pagination queries (i.e Scrolling Cursor). Current queries would be suboptimal for a large Person table.

5) Improve code-behind validation.

6) Consider adding confirmation popup for Update and Delete.

7) Add page size selector.


********************************************************************************************************************************************

Possible points to discuss about existing database schema and application in general:

1) Consider to change data types for person table fields to a more strict ones.

2) It's not reliable to identify a person by the name and age only. This will cause too many collisions on a DB size of a few thousand or more.
   Identification of a person on global scale must include other data as well, like DOB, place of birth etc.
   That is unless there's no requirement to avoid collisions in the first place, so as collisions are actually acceptable and several different persons with the same Name and Age may occupy the same record.

3) Consider to use DOB instead of age to ensure the data is valid over the time. Obviously, if only Age is stored then Person records would become invalid within a year.

4) Due to potential number of Person records it's recommended to add indexes on FirstName, LastName and possibly Age fields to speed up lookups.

5) If application is expected to reach global scale and popularity, we might consider to make Person ID long instead of integer. Int is too short comparing to the potential number of records on the global scale. Might also consider splitting data between several tables if more than a few million records expected.

6) Application might benefit if there's a way to authorise users.








********************************************************************************************************************************************
Original test job description:

AgeRanger is a world leading application designed to identify person's age group!
The only problem with it is... It is not implemented - except a SQLite DB called AgeRanger.db.

To help AgeRanger to conquer the world please implement a web application that communicates with the DB mentioned above, and does the following:

 - Allows user to add a new person - every person has the first name, last name, and age;
 - Displays a list of people in the DB with their First and Last names, age and their age group. The age group should be determened based on the AgeGroup DB table - a person belongs to the age group where person's age >= 
 than group's MinAge and < than group's MaxAge. Please note that MinAge and MaxAge can be null;
 - Allows user to search for a person by his/her first or last name and displays all relevant information for the person - first and last names, age, age group.

In our fantasies AgeRanger is a single page application, and our DBA has already implied that he wants us to migrate it to SQL Server some time in the future.
And unit tests! We love unit tests!

Last, but not the least - our sales manager suggests you'll get bonus points if the application will also allow user to edit existing person records and expose a WEB API.

Please fork the project.

You are free to use any technology and frameworks you need. However if you decide to go with third party package manager or dev tool - don't forget to mention them in the README.md of your fork.

Good luck!