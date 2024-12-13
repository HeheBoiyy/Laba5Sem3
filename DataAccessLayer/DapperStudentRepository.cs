using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModels;
using Microsoft.Extensions.Configuration;


namespace DataAccessLayer
{
    /// <summary>
    /// Реализация IRepository для фреймворка Dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DapperStudentRepository : IStudentRepository
    {
        private static string connectionString;
        private NpgsqlConnection db;
        /// <summary>
        /// Инициализирует новый экземпляр класса DapperRepository.
        /// </summary>
        public DapperStudentRepository(string _connectionString)
        {
            connectionString = _connectionString;
        }
        public void Create(Student t)
        {
            using (var db = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = "INSERT INTO Students (Name, \"Group\", Speciality) " +
                      "VALUES (@Name, @Group, @Speciality) RETURNING Id";

                int id = db.ExecuteScalar<int>(sqlQuery, new
                {
                    Name = t.Name,
                    Group = t.Group,
                    Speciality = t.Speciality
                });

                t.Id = id;
            }
        }

        public IEnumerable<Student> GetAll()
        {
            using (var db = new NpgsqlConnection(GetConnectionString()))
            {
                return db.Query<Student>("SELECT * FROM Students");
            }
        }

        public Student Get(int id)
        {
            using (var db = new NpgsqlConnection(GetConnectionString()))
            {
                return db.QueryFirstOrDefault<Student>("SELECT * FROM Students WHERE Id = @Id", new { Id = id });
            }
        }


        public void Update(Student entity)
        {
            using (var db = new NpgsqlConnection(GetConnectionString()))
            {
                string sqlQuery = "UPDATE Students SET Name = @Name, \"Group\" = @Group, Speciality = @Speciality WHERE Id = @Id";
                db.Execute(sqlQuery, entity);
            }
        }

        public void Delete(int id)
        {
            using (var db = new NpgsqlConnection(GetConnectionString()))
            {
                db.Execute("DELETE FROM Students WHERE Id = @Id", new { Id = id });
            }
        }

        /// <summary>
        /// Освобождает ресурсы, используемые объектом DapperRepository.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освобождает неуправляемые (а при необходимости и управляемые) ресурсы.
        /// </summary>
        /// <param name="disposing">Значение true для освобождения управляемых и неуправляемых ресурсов; значение false для освобождения только неуправляемых ресурсов.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
        }

        /// <summary>
        /// Выполняет произвольный SQL-запрос без возврата результатов.
        /// </summary>
        /// <param name="sql">SQL-запрос для выполнения.</param>
        /// <remarks>
        /// Этот метод открывает новое соединение с базой данных, выполняет указанный SQL-запрос
        /// и автоматически закрывает соединение после выполнения. 
        /// </remarks>
        public void ExecuteSQL(string sql)
        {
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                connection.Execute(sql);
            }
        }
        /// <summary>
        /// Получает строку подключения к базе данных из файла конфигурации appsettings.json.
        /// </summary>
        /// <returns>Строка подключения к базе данных.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если строка подключения 'DefaultConnection' не найдена в appsettings.json.</exception>
        /// <remarks>
        /// Этот метод читает файл appsettings.json, извлекает строку подключения 'DefaultConnection'
        /// и возвращает её. Если строка подключения не найдена, генерируется исключение.
        /// </remarks>
        private string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
            }

            return connectionString;
        }
    }
}