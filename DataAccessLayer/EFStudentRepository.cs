using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    /// <summary>
    /// Реализация репозитория с использованием Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">Тип сущности, наследующийся от Student и реализующий IDomainObject.</typeparam>
    public class EFStudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        
        public EFStudentRepository(DbContextOptions<AppDbContext> options)
        {
            _context = new AppDbContext(options);
        }


        public IEnumerable<Student> GetAll()
        {
            return _context.Set<Student>().ToList();
        }


        public Student Get(int id)
        {
            return _context.Set<Student>().Find(id);
        }


        public void Create(Student entity)
        {
            _context.Set<Student>().Add(entity);
            Save();
        }


        public void Update(Student entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            Save();
        }


        public void Delete(int id)
        {

            var entity = _context.Set<Student>().Find(id);
            if (entity != null)
            {
                _context.Set<Student>().Remove(entity);
                Save();
            }
        }

        /// <summary>
        /// Сохранение изменений в БД
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Освобождает ресурсы, используемые репозиторием.
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
                _context?.Dispose();
            }
        }
    }
}