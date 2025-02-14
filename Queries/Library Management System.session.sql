--@block
SELECT affected_user_id AS AffectedUserId,
        CONCAT(a.first_name, ' ', a.last_name) AS AffectedUserFullName,
        made_by_user_id AS MadeByUserId,
        CONCAT(b.first_name, ' ', b.last_name) AS MadeByUserFullName,
        remaining_balance AS RemainingBalance,
        balance_change AS BalanceChange,
        transaction_date AS TransactionDate
FROM balance_history
        INNER JOIN users a ON affected_user_id = a.id
        LEFT JOIN users b ON made_by_user_id = b.id
ORDER BY transaction_date DESC
LIMIT 5 OFFSET 10;
--@block
SHOW FIELDS
FROM balance_history;
--@block 
SELECT COUNT(book_id)
FROM borrowed_books
WHERE book_id = 212;
--@block
INSERT INTO borrowed_books (book_id, user_id,)
VALUES (212, 1004,);
--@block 
ALTER TABLE balance_history
ADD CONSTRAINT fk_made_by_user FOREIGN KEY (made_by_user_id) REFERENCES users(id) ON UPDATE CASCADE ON DELETE CASCADE;
--@block 
ALTER TABLE borrowed_books
ADD CONSTRAINT fk_user_idx FOREIGN KEY (user_id) REFERENCES users(id) ON UPDATE CASCADE ON DELETE CASCADE;
--@block
UPDATE users
SET id = id + 1000;
--@block
UPDATE users
SET last_name = "dddas"
WHERE id = 1009;
SELECT ROW_COUNT();
--@block
ALTER TABLE books_content
MODIFY COLUMN content MEDIUMTEXT NULL DEFAULT NULL;
--@block
SELECT *
FROM books_content;
--@block
SHOW CREATE TABLE borrowed_books;
--@block
SHOW TRIGGERS;
--@block
SHOW CREATE TRIGGER increase_available_copies;
--@block
CREATE TABLE `books` (
        `id` int NOT NULL AUTO_INCREMENT,
        `author` varchar(40) NOT NULL,
        `title` varchar(40) NOT NULL,
        `genre` varchar(20) NOT NULL,
        `creation_date` datetime DEFAULT CURRENT_TIMESTAMP,
        PRIMARY KEY (`id`)
) ENGINE = InnoDB AUTO_INCREMENT = 100 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
--@block
CREATE TABLE `books_content` (
        `book_id` int NOT NULL,
        `content` mediumtext,
        UNIQUE KEY `unique_book_idx` (`book_id`),
        CONSTRAINT `fk_book_content_idx` FOREIGN KEY (`book_id`) REFERENCES `books` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
--@block
CREATE TABLE `borrowed_books` (
        `transaction_id` int NOT NULL AUTO_INCREMENT,
        `book_id` int DEFAULT NOT NULL,
        `user_id` int DEFAULT NOT NULL,
        `borrowing_date` date NOT NULL DEFAULT (curdate()),
        `return_date` date NOT NULL,
        PRIMARY KEY (`transaction_id`),
        KEY `fk_book_idx` (`book_id`),
        KEY `fk_user_idx` (`user_id`),
        CONSTRAINT `fk_book_idx` FOREIGN KEY (`book_id`) REFERENCES `books` (`id`),
        CONSTRAINT `fk_user_idx` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB AUTO_INCREMENT = 100 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
--@block
CREATE TABLE `inventory` (
        `book_id` int NOT NULL,
        `copies_available` int NOT NULL,
        `borrowing_period` int NOT NULL DEFAULT '14',
        UNIQUE KEY `unique_const` (`book_id`),
        KEY `copies_available_idx` (`book_id`, `copies_available`),
        CONSTRAINT `fk_book_id` FOREIGN KEY (`book_id`) REFERENCES `books` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
--@block
CREATE TABLE `balance_history` (
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
) ENGINE = InnoDB AUTO_INCREMENT = 1000 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
--@block
--@block
CREATE TABLE `users` (
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
) ENGINE = InnoDB AUTO_INCREMENT = 1000 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;
--@block
CREATE TRIGGER `decrease_available_copies`
AFTER
INSERT ON `borrowed_books` FOR EACH ROW BEGIN
UPDATE inventory
SET copies_available = copies_available - 1
WHERE book_id = NEW.book_id;
END;
--@block
CREATE TRIGGER `increase_available_copies`
AFTER DELETE ON `borrowed_books` FOR EACH ROW BEGIN
UPDATE inventory
SET copies_available = copies_available + 1
WHERE book_id = OLD.book_id;
END;