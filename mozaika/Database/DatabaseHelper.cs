using System;
using Microsoft.Data.Sqlite;
using System.IO;
using Mozaika.Models;

namespace Mozaika.Database
{
    public class DatabaseHelper
    {
        private const string DatabaseFile = "mozaika.db";
        private static string ConnectionString => $"Data Source={DatabaseFile}";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }

        public static void InitializeDatabase()
        {
            bool isNewDatabase = !File.Exists(DatabaseFile);

            // Файл базы данных будет создан автоматически при первом соединении

            using (var connection = GetConnection())
            {
                connection.Open();

                // Создание таблиц
                CreateTables(connection);

                // Заполнение тестовыми данными только для новой БД
                if (isNewDatabase)
                {
                    SeedData(connection);
                }
                else
                {
                    // Для существующей БД проверяем и добавляем пользователей, если их нет
                    EnsureUsersExist(connection);
                }
            }
        }

        private static void CreateTables(SqliteConnection connection)
        {
            string[] createTableQueries = {
                @"CREATE TABLE IF NOT EXISTS Suppliers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Type TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Inn TEXT NOT NULL,
                    ContactInfo TEXT,
                    CreatedDate DATETIME NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1
                )",

                @"CREATE TABLE IF NOT EXISTS Materials (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Type TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    SupplierId INTEGER NOT NULL,
                    PackageQuantity INTEGER NOT NULL,
                    Unit TEXT NOT NULL,
                    Description TEXT,
                    Image BLOB,
                    Cost DECIMAL(10,2) NOT NULL,
                    StockQuantity INTEGER NOT NULL DEFAULT 0,
                    MinQuantity INTEGER NOT NULL DEFAULT 0,
                    LastUpdated DATETIME NOT NULL,
                    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
                )",

                @"CREATE TABLE IF NOT EXISTS Partners (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Type TEXT NOT NULL,
                    CompanyName TEXT NOT NULL,
                    LegalAddress TEXT NOT NULL,
                    Inn TEXT NOT NULL,
                    DirectorName TEXT NOT NULL,
                    Phone TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Logo BLOB,
                    Rating INTEGER NOT NULL DEFAULT 0,
                    SalesLocations TEXT,
                    TotalSalesVolume DECIMAL(15,2) NOT NULL DEFAULT 0,
                    CreatedDate DATETIME NOT NULL
                )",

                @"CREATE TABLE IF NOT EXISTS Employees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT NOT NULL,
                    BirthDate DATETIME NOT NULL,
                    PassportData TEXT NOT NULL,
                    BankDetails TEXT NOT NULL,
                    HasFamily INTEGER NOT NULL DEFAULT 0,
                    HealthStatus TEXT,
                    Position TEXT NOT NULL,
                    HireDate DATETIME NOT NULL,
                    Salary DECIMAL(10,2) NOT NULL DEFAULT 0,
                    EquipmentAccess TEXT
                )",

                @"CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Article TEXT NOT NULL UNIQUE,
                    Type TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Image BLOB,
                    MinPartnerPrice DECIMAL(10,2) NOT NULL,
                    PackageLength DECIMAL(8,2) NOT NULL,
                    PackageWidth DECIMAL(8,2) NOT NULL,
                    PackageHeight DECIMAL(8,2) NOT NULL,
                    WeightWithoutPackage DECIMAL(8,2) NOT NULL,
                    WeightWithPackage DECIMAL(8,2) NOT NULL,
                    QualityCertificate BLOB,
                    StandardNumber TEXT,
                    CostPrice DECIMAL(10,2) NOT NULL,
                    WorkshopNumber INTEGER NOT NULL,
                    ProductionWorkersCount INTEGER NOT NULL,
                    ProductionTime TEXT NOT NULL,
                    RequiredMaterials TEXT,
                    StockQuantity INTEGER NOT NULL DEFAULT 0
                )",

                @"CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PartnerId INTEGER NOT NULL,
                    CreatedDate DATETIME NOT NULL,
                    PrepaymentDeadline DATETIME,
                    PrepaymentDate DATETIME,
                    CompletionDate DATETIME,
                    Status INTEGER NOT NULL DEFAULT 0,
                    TotalAmount DECIMAL(15,2) NOT NULL DEFAULT 0,
                    FOREIGN KEY (PartnerId) REFERENCES Partners(Id)
                )",

                @"CREATE TABLE IF NOT EXISTS OrderItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    UnitPrice DECIMAL(10,2) NOT NULL,
                    ProductionDate DATETIME,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )",

                @"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Role INTEGER NOT NULL,
                    CreatedDate DATETIME NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1
                )"
            };

            foreach (var query in createTableQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedData(SqliteConnection connection)
        {
            // Проверка, есть ли уже пользователи
            using (var command = new SqliteCommand("SELECT COUNT(*) FROM Users", connection))
            {
                var count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0) return; // Пользователи уже созданы
            }

            // Добавление тестовых поставщиков
            string[] supplierQueries = {
                "INSERT INTO Suppliers (Type, Name, Inn, ContactInfo, CreatedDate, IsActive) VALUES ('Производитель', 'ООО \"Керамика Плюс\"', '1234567890', '+7(495)123-45-67, supplier1@email.com', '2024-01-01', 1)",
                "INSERT INTO Suppliers (Type, Name, Inn, ContactInfo, CreatedDate, IsActive) VALUES ('Поставщик сырья', 'ЗАО \"Минералы\"', '0987654321', '+7(495)987-65-43, supplier2@email.com', '2024-01-01', 1)"
            };

            foreach (var query in supplierQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление тестовых материалов
            string[] materialQueries = {
                "INSERT INTO Materials (Type, Name, SupplierId, PackageQuantity, Unit, Description, Cost, StockQuantity, MinQuantity, LastUpdated) VALUES ('Глина', 'Каолин белый', 1, 1000, 'кг', 'Высококачественная глина для производства плитки', 150.00, 5000, 1000, '2024-01-01')",
                "INSERT INTO Materials (Type, Name, SupplierId, PackageQuantity, Unit, Description, Cost, StockQuantity, MinQuantity, LastUpdated) VALUES ('Красители', 'Керамические пигменты', 2, 25, 'кг', 'Пигменты для окрашивания керамики', 500.00, 100, 25, '2024-01-01')"
            };

            foreach (var query in materialQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление тестовых партнеров
            string[] partnerQueries = {
                "INSERT INTO Partners (Type, CompanyName, LegalAddress, Inn, DirectorName, Phone, Email, Rating, SalesLocations, TotalSalesVolume, CreatedDate) VALUES ('Розничный магазин', 'Магазин \"Керамика Дом\"', 'г. Москва, ул. Ленина, д. 10', '1111111111', 'Иванов Иван Иванович', '+7(495)111-11-11', 'info@keramikadom.ru', 85, 'Москва, Санкт-Петербург', 1500000.00, '2024-01-01')",
                "INSERT INTO Partners (Type, CompanyName, LegalAddress, Inn, DirectorName, Phone, Email, Rating, SalesLocations, TotalSalesVolume, CreatedDate) VALUES ('Оптовый склад', 'ООО \"СтройМатериалы\"', 'г. Москва, ул. Пушкина, д. 5', '2222222222', 'Петров Петр Петрович', '+7(495)222-22-22', 'info@stroymat.ru', 92, 'Москва, Московская область', 5000000.00, '2024-01-01')"
            };

            foreach (var query in partnerQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление тестовых сотрудников
            string[] employeeQueries = {
                "INSERT INTO Employees (FullName, BirthDate, PassportData, BankDetails, HasFamily, HealthStatus, Position, HireDate, Salary, EquipmentAccess) VALUES ('Сидоров Алексей Михайлович', '1985-05-15', '45 12 345678', 'Сбербанк, р/с 12345678901234567890', 1, 'Хорошее', 'Мастер производства', '2020-03-01', 75000.00, 'Печь №1, Пресс №2, Глазуровочная машина')",
                "INSERT INTO Employees (FullName, BirthDate, PassportData, BankDetails, HasFamily, HealthStatus, Position, HireDate, Salary, EquipmentAccess) VALUES ('Кузнецова Мария Сергеевна', '1990-08-22', '46 23 456789', 'Тинькофф, р/с 09876543210987654321', 0, 'Отличное', 'Менеджер по продажам', '2021-06-15', 65000.00, 'Компьютер, Телефон')"
            };

            foreach (var query in employeeQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление тестовых продуктов
            string[] productQueries = {
                "INSERT INTO Products (Article, Type, Name, Description, MinPartnerPrice, PackageLength, PackageWidth, PackageHeight, WeightWithoutPackage, WeightWithPackage, StandardNumber, CostPrice, WorkshopNumber, ProductionWorkersCount, ProductionTime, RequiredMaterials, StockQuantity) VALUES ('КП-001', 'Керамическая плитка', 'Плитка настенная белая 20x30', 'Качественная керамическая плитка для внутренней отделки', 250.00, 0.25, 0.35, 0.08, 1.2, 1.5, 'ГОСТ 13996-2019', 150.00, 1, 3, '02:00:00', 'Каолин белый 2кг, Керамические пигменты 0.1кг', 500)",
                "INSERT INTO Products (Article, Type, Name, Description, MinPartnerPrice, PackageLength, PackageWidth, PackageHeight, WeightWithoutPackage, WeightWithPackage, StandardNumber, CostPrice, WorkshopNumber, ProductionWorkersCount, ProductionTime, RequiredMaterials, StockQuantity) VALUES ('КП-002', 'Керамическая плитка', 'Плитка напольная коричневая 30x30', 'Износостойкая напольная плитка', 350.00, 0.35, 0.35, 0.10, 2.1, 2.5, 'ГОСТ 13996-2019', 220.00, 2, 4, '02:30:00', 'Каолин белый 3кг, Керамические пигменты 0.2кг', 300)"
            };

            foreach (var query in productQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление тестовых пользователей
            string[] userQueries = {
                "INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive) VALUES ('admin', 'admin123', 'Администратор системы', 0, '2024-01-01', 1)",
                "INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive) VALUES ('manager', 'manager123', 'Менеджер по продажам', 1, '2024-01-01', 1)",
                "INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive) VALUES ('master', 'master123', 'Мастер производства', 2, '2024-01-01', 1)"
            };

            foreach (var query in userQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление тестовых заказов
            string[] orderQueries = {
                "INSERT INTO Orders (PartnerId, CreatedDate, Status, TotalAmount) VALUES (1, '2024-01-15', 6, 150000.00)",
                "INSERT INTO Orders (PartnerId, CreatedDate, Status, TotalAmount) VALUES (2, '2024-01-20', 5, 250000.00)",
                "INSERT INTO Orders (PartnerId, CreatedDate, Status, TotalAmount) VALUES (1, '2024-01-25', 3, 80000.00)"
            };

            foreach (var query in orderQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление позиций заказов (предполагая, что Orders имеют ID 1, 2, 3)
            string[] orderItemQueries = {
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (1, 1, 10, 250.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (1, 2, 5, 350.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 1, 15, 240.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 2, 8, 340.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (3, 1, 5, 250.00)"
            };

            foreach (var query in orderItemQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EnsureUsersExist(SqliteConnection connection)
        {
            // Проверка и добавление каждого пользователя по отдельности
            EnsureUserExists(connection, "admin", "admin123", "Администратор системы", 0);
            EnsureUserExists(connection, "manager", "manager123", "Менеджер по продажам", 1);
            EnsureUserExists(connection, "master", "master123", "Мастер производства", 2);

            // Проверка и добавление тестовых заказов
            EnsureOrdersExist(connection);
        }

        private static void EnsureUserExists(SqliteConnection connection, string username, string password, string fullName, int role)
        {
            // Проверяем, существует ли пользователь
            using (var command = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                var count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0) return; // Пользователь уже существует
            }

            // Добавляем пользователя
            using (var command = new SqliteCommand(
                "INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive) VALUES (@Username, @Password, @FullName, @Role, @CreatedDate, @IsActive)",
                connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@FullName", fullName);
                command.Parameters.AddWithValue("@Role", role);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@IsActive", 1);
                command.ExecuteNonQuery();
            }
        }

        private static void EnsureOrdersExist(SqliteConnection connection)
        {
            // Проверка, есть ли уже заказы
            using (var command = new SqliteCommand("SELECT COUNT(*) FROM Orders", connection))
            {
                var count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0) return; // Заказы уже существуют
            }

            // Добавление тестовых заказов
            string[] orderQueries = {
                "INSERT INTO Orders (PartnerId, CreatedDate, Status, TotalAmount) VALUES (1, '2024-01-15', 6, 150000.00)",
                "INSERT INTO Orders (PartnerId, CreatedDate, Status, TotalAmount) VALUES (2, '2024-01-20', 5, 250000.00)",
                "INSERT INTO Orders (PartnerId, CreatedDate, Status, TotalAmount) VALUES (1, '2024-01-25', 3, 80000.00)"
            };

            foreach (var query in orderQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            // Добавление позиций заказов
            string[] orderItemQueries = {
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (1, 1, 10, 250.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (1, 2, 5, 350.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 1, 15, 240.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (2, 2, 8, 340.00)",
                "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (3, 1, 5, 250.00)"
            };

            foreach (var query in orderItemQueries)
            {
                using (var command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
