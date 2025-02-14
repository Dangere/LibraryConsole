using System.Data.Common;
using Dapper;
using MySql.Data.MySqlClient;

namespace LibraryManagementSystem.Utilities;
public class DatabaseInitializer
{

    public static void Initialize(string ConnectionString)
    {
        using DbConnection dbConnection = new MySqlConnection(ConnectionString);

        // dbConnection.BeginTransaction();

        // string createDB = @"CREATE DATABASE IF NOT EXISTS library_management_system;";
        // string useDb = @"USE library_management_system;";

        string createUsersTable = @"
        CREATE TABLE IF NOT EXISTS `users` (
        `id` int NOT NULL AUTO_INCREMENT,
        `first_name` varchar(20) NOT NULL,
        `last_name` varchar(20) NOT NULL,
        `email` varchar(30) NOT NULL,
        `phone` varchar(20) NOT NULL,
        `balance` decimal(10, 2) NOT NULL DEFAULT 0.00,
        `creation_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
        `hashed_password` varchar(255) NOT NULL,
        `salt` varchar(255) NOT NULL,
        `admin` tinyint(1) DEFAULT 0,
        PRIMARY KEY (`id`),
        UNIQUE KEY `unique_email` (`email`),
        -- Creates a unique index
        KEY `email_hash_idx` (`email`, `hashed_password`) -- Composite index
        ) ENGINE = InnoDB AUTO_INCREMENT = 1000 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;";

        string createBalanceHistoryTable = @"
        CREATE TABLE IF NOT EXISTS `balance_history` (
        `transaction_id` int NOT NULL AUTO_INCREMENT,
        `affected_user_id` int NOT NULL,
        `made_by_user_id` int DEFAULT NULL,
        `remaining_balance` decimal(10, 2) NOT NULL,
        `balance_change` decimal(10, 2) NOT NULL,
        `transaction_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
        PRIMARY KEY (`transaction_id`),
        KEY `fk_affected_user` (`affected_user_id`),
        KEY `fk_made_by_user` (`made_by_user_id`),
        CONSTRAINT `fk_affected_user` FOREIGN KEY (`affected_user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
        CONSTRAINT `fk_made_by_user` FOREIGN KEY (`made_by_user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
        ) ENGINE = InnoDB AUTO_INCREMENT = 1000 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;";

        string createBooksTable = @"
        CREATE TABLE IF NOT EXISTS `books` (
        `id` int NOT NULL AUTO_INCREMENT,
        `author` varchar(40) NOT NULL,
        `title` varchar(40) NOT NULL,
        `genre` varchar(20) NOT NULL,
        `creation_date` datetime DEFAULT CURRENT_TIMESTAMP,
        PRIMARY KEY (`id`)
        ) ENGINE = InnoDB AUTO_INCREMENT = 100 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;";

        string createBooksContentTable = @"
        CREATE TABLE IF NOT EXISTS `books_content` (
        `book_id` int NOT NULL,
        `content` mediumtext,
        UNIQUE KEY `unique_book_idx` (`book_id`),
        CONSTRAINT `fk_book_content_idx` FOREIGN KEY (`book_id`) REFERENCES `books` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
        ) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;";

        string createBorrowedBooksTable = @"
        CREATE TABLE IF NOT EXISTS `borrowed_books` (
        `transaction_id` int NOT NULL AUTO_INCREMENT,
        `book_id` int NOT NULL,
        `user_id` int NOT NULL,
        `borrowing_date` date NOT NULL DEFAULT (curdate()),
        `return_date` date NOT NULL,
        PRIMARY KEY (`transaction_id`),
        KEY `fk_book_idx` (`book_id`),
        KEY `fk_user_idx` (`user_id`),
        CONSTRAINT `fk_book_idx` FOREIGN KEY (`book_id`) REFERENCES `books` (`id`),
        CONSTRAINT `fk_user_idx` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
        ) ENGINE = InnoDB AUTO_INCREMENT = 100 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;";

        string createInventoryTable = @"
        CREATE TABLE IF NOT EXISTS `inventory` (
        `book_id` int NOT NULL,
        `copies_available` int NOT NULL,
        `borrowing_period` int NOT NULL DEFAULT '14',
        UNIQUE KEY `unique_const` (`book_id`),
        KEY `copies_available_idx` (`book_id`, `copies_available`),
        CONSTRAINT `fk_book_id` FOREIGN KEY (`book_id`) REFERENCES `books` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
        ) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;";

        string dropTriggers = @"
        DROP TRIGGER IF EXISTS decrease_available_copies;
        DROP TRIGGER IF EXISTS increase_available_copies;";

        string createTriggers = @"
        CREATE TRIGGER `decrease_available_copies`
        AFTER
        INSERT ON `borrowed_books` FOR EACH ROW BEGIN
        UPDATE inventory
        SET copies_available = copies_available - 1
        WHERE book_id = NEW.book_id;
        END;

        CREATE TRIGGER `increase_available_copies`
        AFTER DELETE ON `borrowed_books` FOR EACH ROW BEGIN
        UPDATE inventory
        SET copies_available = copies_available + 1
        WHERE book_id = OLD.book_id;
        END;
        ";

        dbConnection.Open();

        using DbTransaction tran = dbConnection.BeginTransaction();
        // Execute your queries here
        dbConnection.Execute(createUsersTable);
        dbConnection.Execute(createBalanceHistoryTable);
        dbConnection.Execute(createBooksTable);
        dbConnection.Execute(createBooksContentTable);
        dbConnection.Execute(createBorrowedBooksTable);
        dbConnection.Execute(createInventoryTable);
        dbConnection.Execute(dropTriggers);
        dbConnection.Execute(createTriggers);
        tran.Commit(); //Or rollback 
    }

}