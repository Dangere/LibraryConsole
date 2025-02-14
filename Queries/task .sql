--@block getting the sum of amount for a specific customer using their name
SELECT SUM(amount) AS "Total amount"
FROM orders
    RIGHT JOIN customers ON orders.customer_id = customers.customer_id
WHERE customers.first_name = "john";
--@block
SELECT a.first_name,
    b.amount,
    (
        SELECT AVG(amount)
        FROM orders
    )
FROM customers AS a
    RIGHT JOIN orders AS b ON a.customer_id = b.customer_id;
--@block 
SELECT first_name,
    age
FROM customers;
--@block
SELECT a.first_name,
    a.age
FROM customers AS a
    LEFT JOIN orders AS b ON a.customer_id = b.customer_id;
--@block
SHOW INDEXES
FROM customers;