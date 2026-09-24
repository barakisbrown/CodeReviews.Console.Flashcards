<div align="center">
FlashCards by Matthew Brown
</div>

<br>

# Instruction:

First clone this application into a directory but before running
the application, you will need to go into the appsettings.json file.

This applications assumes that an Microsoft SQL Server is running in a container.

Inside the appsetting file, there will see a section called connection strings:<br>
* Main points to the database which this applicaton called flashcards.	 
* Backup will point to the master database that MSSQL uses to create the db,etc.

Once this is done, then you simply launch the application via `dotnet run`

# Application Overview

This app represents a tool for managing flashcards and a study notes on those
flash cards that you will create.  I have create a simple menu structure.

You would need to create a stack -- note DEFAULT stack -- is already created
and it can not be deleted/etc.  This is for so that if you do not have any other
stacks created, you can at least point to it.  The edit menu of the Flashcard can
then move it to the correct stack as needed.

# Architecural Choices

This app uses .NET Core 8 and use ado.net with dapper as a light orm to help with
some of the db access. I use MSSQL Server.  I orginally was using the SQL Dev on
my linode server but then I learned how to use docker. I use different projects in the same solution to seperate
what I want done.  

# Reflection

I started this project 3 yrs ago but do some choices I never got it completed until
recently.  I had to rewrite some of them a couple of times. I am glad that I finally finished
it.  
