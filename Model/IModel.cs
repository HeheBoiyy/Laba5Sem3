using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityModels;

namespace Model
{
    public interface IModel
    {
        /// <summary>
        /// Метод для сброса инкремента Id в БД
        /// </summary>
        void ResetStudentIdSequence();
        // <summary>
        /// Получает студента по его ID.
        /// </summary>
        /// <param name="id">ID студента, которого нужно получить.</param>
        /// <returns>Студент с указанным ID или null, если студент не найден.</returns>
        Student GetStudentById(int id);
        /// <summary>
        /// Добавляет нового студента в репозиторий.
        /// </summary>
        /// <param name="student">Студент, которого нужно добавить.</param>
        void AddStudent(Student student);

        /// <summary>
        /// Загружает всех студентов из репозитория.
        /// </summary>
        /// <returns>Список студентов, каждый из которых представлен в виде списка строк.</returns>
        List<List<string>> LoadStudents();

        /// <summary>
        /// Обновляет информацию о существующем студенте в репозитории.
        /// </summary>
        /// <param name="student">Студент с обновленной информацией.</param>
        void UpdateStudent(Student student);

        /// <summary>
        /// Удаляет студента из репозитория по его ID.
        /// </summary>
        /// <param name="id">ID студента, которого нужно удалить.</param>
        void DeleteStudent(int id);

        /// <summary>
        /// Генерирует отчет в виде гистограммы студентов, сгруппированных по их специальности.
        /// </summary>
        /// <returns>Словарь, где ключ - это специальность, а значение - количество студентов в этой специальности.</returns>
        Dictionary<string, int> ReportStudentHistogram();
    }
}
