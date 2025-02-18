# Library Console App
A console-based library system built with .NET that follows clean architecture principles (Repository Pattern, Dependency Injection, etc.). It uses Dapper to communicate with a MySQL database and includes robust security features such as password salting and hashing.

### Overview
- Guest/User Functionality:
	- Guests can browse the inventory, view available books (including the number of copies and borrowing period), and search by book ID.
	- Registered users can borrow books, view the content, and check detailed information like remaining days until the expected return date and overdue days.
- Admin Functionality:
  - Admins have access to a dedicated admin panel.
  - They can create new books, add them to the inventory, adjust the number of available copies, and change the borrowing period (without affecting books already borrowed).
  - Admins can manage users by searching for a user ID to view details like balance, email, phone, and borrowed books. They also have the ability to adjust user balances and update personal information.
  - A balance history screen is available to review all changes to user balances, including system-imposed penalties when books are returned late.

### Features
- User Side:
  - Inventory browsing with search functionality by book ID.
  - User registration and secure login (passwords are stored securely using salting and hashing).
  - Borrowing books with full details on borrow/return dates, overdue days, and penalties.
  - Viewing book content directly in the console.
  - Users info are reverified whenever they try accessing book's content to ensure greater security.
- Admin Side:
	- Admin panel with secure login.
  - Create and manage books, including adding content and metadata.
  - Manage library inventory: adjust copies available and borrowing periods.
  - Comprehensive user management: update user details (email, phone, name, etc.) and manage account balances.
  - View a detailed balance history log, showing all transactions (both admin actions and system-generated penalty adjustments).
  - Admin status are reverified whenever an action affects users/books to ensure greater security.

### Technologies
- .NET:  
Built with modern .NET, leveraging clean coding practices and SOLID principles.
- Dapper:  
A lightweight ORM used for mapping SQL query results to C# models.
- MySQL:  
The application uses a MySQL database for data storage.
- Clean Architecture Patterns:  
Utilizes the Repository Pattern, Dependency Injection, and other best practices to ensure maintainability and scalability.
- Security:  
Passwords are secured using salting and hashing to prevent storing plain-text passwords.

### Setup Instructions
#### Prerequisites
- .NET SDK: Make sure you have .NET 6 (or later) installed.
- MySQL: A MySQL database server.
- Database Initialization:
The project includes a `DatabaseInitializer` class that creates the necessary tables and triggers. You need to run this initializer when you first connect to your database.

#### Configuration
1. Clone the Repository:
```bash
git clone https://github.com/Dangere/library-console-app.git
```
2. Configure the Connection String:
```csharp
string ConnectionString = "Server=your-host;Database=library;Uid=your-username;Pwd=your-password;";
```
3. Initialize the Database:  
Trigger the DatabaseInitializer.Initialize(ConnectionString) method which creates the necessary schema (tables, relationships, and triggers).

#### Running the App
- Compile the Application: Use your favorite IDE (like Visual Studio or VS Code) or run:
``` bash
dotnet build
 ```
- Run the Console App:
``` bash
dotnet run
```
### Security Notes
- Password Security: User passwords are not stored in plain text. The app uses salting and hashing to secure user credentials.
- Connection String: For production, consider using environment variables or secure secret management tools to store sensitive information rather than hard-coding them.


## User Interface Screenshots
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/MainMenu.PNG" width="300" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/RegisterScreen.PNG" width="300" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/Inventory.PNG" width="500" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/ViewInventoryBook.PNG" width="300" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/BorrowedBooksList.PNG" width="300" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/ViewingBorrowedBook.PNG" width="300" />

<!-- ![Main Menu](Screenshots/MainMenu.PNG | width=100)
![Register Screen](Screenshots/RegisterScreen.PNG | width=100)
![Inventory Screen](Screenshots/Inventory.PNG | width=100)
![Inspecting Inventory Book](Screenshots/ViewInventoryBook.PNG | width=100)
![Borrowed Books List](Screenshots/BorrowedBooksList.PNG | width=100)
![Inspecting Borrowed Book](Screenshots/ViewingBorrowedBook.PNG | width=100) -->

## Admin Interface Screenshots
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/admin_UsersManagementScreen.PNG" width="500" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/admin_BalanceHistory.PNG" width="500" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/admin_AddingBooks.PNG" width="300" />
<img src="https://github.com/Dangere/library-console-app/blob/main/Screenshots/admin_EditingBooks.PNG" width="300" />


