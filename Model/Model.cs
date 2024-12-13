using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using EntityModels;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class Model : IModel
    {
        private readonly IRepository<Student> _studentRepository;

        public Model(IRepository<Student> studentRepository)
        {
            _studentRepository = studentRepository;
        }
        public Student GetStudentById(int id)
        {
            return _studentRepository.GetAll().FirstOrDefault(student => student.Id == id);
        }

        public void AddStudent(Student student)
        {
            _studentRepository.Create(student);
        }

        public List<List<string>> LoadStudents()
        {
            var students = _studentRepository.GetAll();
            return students.Select(student =>
                new List<string> { student.Id.ToString(), student.Name, student.Speciality, student.Group }).ToList();
        }

        public void UpdateStudent(Student student)
        {
            _studentRepository.Update(student);
        }

        public void DeleteStudent(int id)
        {
            _studentRepository.Delete(id);
        }

        public Dictionary<string, int> ReportStudentHistogram()
        {
            var students = _studentRepository.GetAll();
            return students.GroupBy(s => s.Speciality)
                           .ToDictionary(g => g.Key, g => g.Count());
        }
        /// <summary>
        /// Создание DB Context (только для EF Framework)
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private AppDbContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new AppDbContext(optionsBuilder.Options);
        }

        public void ResetStudentIdSequence()
        {
            if (_studentRepository is DapperStudentRepository dapperRepository)
            {
                dapperRepository.ExecuteSQL(@"
                SELECT setval(pg_get_serial_sequence('students', 'id'), 
                (SELECT COALESCE(MAX(id),0) FROM students), false);");
            }
            else if (_studentRepository is EFStudentRepository)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string connectionString = configuration.GetConnectionString("DefaultConnection");

                using (var context = CreateDbContext(connectionString))
                {
                    context.Database.ExecuteSqlRaw(@"
                    SELECT setval(pg_get_serial_sequence('students', 'id'), 
                    (SELECT COALESCE(MAX(id),0) FROM students), false);
                    ");
                }
            }
            else
            {
                throw new NotSupportedException("Текущий репозиторий не поддерживает сброс последовательности ID.");
            }
        }
    }
}
