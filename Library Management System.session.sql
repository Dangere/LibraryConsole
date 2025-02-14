--@block
SELECT *
FROM inventory
WHERE IF(TRUE, copies_available, 1) > 0;