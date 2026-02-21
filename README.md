# 📌 LunchList Web App "Würschtel"
The Office Lunch Order System is a small internal web application developed during my internship.

In our office, employees occasionally bought lunch together (e.g., sausages, bread rolls, and other snacks). Previously, orders were collected manually using a paper sheet that was passed around the office.

This web application replaces the manual process with a simple digital solution.

(note: The website and naming of variables, functions, etc. are mostly in German as I worked at a german company)

## Features
### 🛒 Ordering ("Bestellungen")
Users are automatically signed in with their windows username. The available products are listed with their respective price. The user can choose individual quantities and confirm the order which gets saved in a database.

### ⚙️ Orders Display ("Bestellungen ansehen")
Today's orders get shown on the "Bestellungen ansehen" page. The listed orders show the product quantities and subtotals. Prices can easily be changed at the top of the table due to them changing frequently. Additionally there are checkboxes to mark orders as "payed" which doesn't change price calculation it just helps keeping track of already payed orders.
<img width="769" height="352" alt="image" src="https://github.com/user-attachments/assets/96f6afb3-e0fb-49c1-a8db-59816bbc7181" />

### 📱 Responsive design with bootstrap
Elements colapse or reorder according to viewport width.

### 🏗️ Dynamic website
Elements get build dynamically according to the database. That way products can be added or removed without needing to rewrite anything in the website's code.

## MySql Database
In case you want to check out the website for yourself you need to set up your own schema in MySql and change the connection setting in Web.config. The database only contains two tables. Here is the create command:

```MySql
CREATE DATABASE `praktikum` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

CREATE TABLE `bestellungen` ( /* orders */
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `yourproduct(quantity)` int DEFAULT '0', /* insert a column here for each product you want to display/use */
  `datum` datetime DEFAULT NULL,  /* date */
  `bezahlt` tinyint DEFAULT '0', /* paid */
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `produkte` ( /* products */
  `name` varchar(45) NOT NULL,
  `preis` float DEFAULT '0', /* price */
  PRIMARY KEY (`name`),
  UNIQUE KEY `name_UNIQUE` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```
