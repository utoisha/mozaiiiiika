using System;
using Microsoft.Data.Sqlite;
using Mozaika.Database;
using Mozaika.Models;

namespace Mozaika.Services
{
    public static class AuthService
    {
        private static User? _currentUser;

        public static User? CurrentUser => _currentUser;

        public static bool Login(string username, string password)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT Id, Username, Password, FullName, Role, CreatedDate, IsActive
                                FROM Users
                                WHERE Username = @Username AND Password = @Password AND IsActive = 1";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _currentUser = new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                FullName = reader.GetString(3),
                                Role = (UserRole)reader.GetInt32(4),
                                CreatedDate = reader.GetDateTime(5),
                                IsActive = reader.GetBoolean(6)
                            };
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void Logout()
        {
            _currentUser = null;
        }

        public static bool HasPermission(UserRole requiredRole)
        {
            if (_currentUser == null) return false;

            // Admin имеет все права
            if (_currentUser.Role == UserRole.Admin) return true;

            // Manager имеет права на Manager функции
            if (_currentUser.Role == UserRole.Manager && requiredRole == UserRole.Manager) return true;

            // Master имеет права на Master функции
            if (_currentUser.Role == UserRole.Master && requiredRole == UserRole.Master) return true;

            return false;
        }

        public static bool IsAdmin => _currentUser?.Role == UserRole.Admin;
        public static bool IsManager => _currentUser?.Role == UserRole.Manager;
        public static bool IsMaster => _currentUser?.Role == UserRole.Master;
    }
}

