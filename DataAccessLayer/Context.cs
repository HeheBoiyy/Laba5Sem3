using Microsoft.EntityFrameworkCore;
using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    /// <summary>
    /// Представляет контекст базы данных для приложения, обеспечивающий доступ к данным студентов.
    /// Этот класс настраивает подключение к базе данных и определяет структуру таблицы Students.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Получает или устанавливает DbSet студентов. Это представляет таблицу Students в базе данных.
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса AppDbContext.
        /// </summary>
        /// <param name="options">Параметры, используемые DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }

        /// <summary>
        /// Настраивает модель, которая была обнаружена по соглашению из типов сущностей,
        /// представленных в свойствах DbSet в производном контексте.
        /// </summary>
        /// <param name="modelBuilder">Построитель, используемый для конструирования модели для этого контекста.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("students");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Speciality).HasColumnName("speciality");
                entity.Property(e => e.Group).HasColumnName("Group");
            });
        }
    }
}
