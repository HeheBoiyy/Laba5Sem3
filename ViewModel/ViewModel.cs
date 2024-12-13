using DataAccessLayer;
using EntityModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;
using LiveCharts;
using Ninject;

namespace ViewModel
{
    public class Logic : INotifyPropertyChanged
    {
        private ObservableCollection<Student> students = new ObservableCollection<Student>();
        private string nameToAdd;
        private string specToAdd;
        private string groupToAdd;
        private Student selectedStudent;
        private readonly IModel model;
        private ChartValues<int> values;
        private string newName;
        private string newGroup;
        private string newSpec;

        /// <summary>
        /// Событие, возникающее при изменении свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Получает или устанавливает значения для графика.
        /// </summary>
        public ChartValues<int> Values
        {
            get { return values; }
            set
            {
                values = value;
                OnPropertyChanged(nameof(Values));
            }
        }

        /// <summary>
        /// Получает или устанавливает метки для графика.
        /// </summary>
        public ChartValues<string> Labels { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public Logic(Model.Model model)
        {
            this.model = model;
            InitializeCommands();
            students = new ObservableCollection<Student>(GetStudents());
            Values = new ChartValues<int>();
            Labels = new ChartValues<string>(Specialities);
            UpdateChartValues();
        }

        /// <summary>
        /// Получает или устанавливает коллекцию студентов.
        /// </summary>
        public ObservableCollection<Student> Students
        {
            get { return GetStudents(); }
            set { }
        }

        /// <summary>
        /// Получает или устанавливает выбранного студента.
        /// </summary>
        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }

        /// <summary>
        /// Получает или устанавливает имя для добавления нового студента.
        /// </summary>
        public string NameToAdd
        {
            get { return nameToAdd; }
            set
            {
                nameToAdd = value;
                OnPropertyChanged(nameof(NameToAdd));
            }
        }

        /// <summary>
        /// Получает или устанавливает специальность для добавления нового студента.
        /// </summary>
        public string SpecToAdd
        {
            get { return specToAdd; }
            set
            {
                specToAdd = value;
                OnPropertyChanged(nameof(SpecToAdd));
            }
        }

        /// <summary>
        /// Получает или устанавливает группу для добавления нового студента.
        /// </summary>
        public string GroupToAdd
        {
            get { return groupToAdd; }
            set
            {
                groupToAdd = value;
                OnPropertyChanged(nameof(GroupToAdd));
            }
        }

        /// <summary>
        /// Получает список доступных специальностей.
        /// </summary>
        public List<string> Specialities { get; } = new List<string>
        {
            "ИБ",
            "ИСИТ",
            "ИВТ",
            "ПИ",
            "КБ"
        };

        /// <summary>
        /// Метод дл добавления нового студента.
        /// </summary>
        /// <param name="name">Имя студента.</param>
        /// <param name="speciality">Специальность студента.</param>
        /// <param name="group">Группа студента.</param>
        public void AddStudent(string name, string speciality, string group)
        {
            Student student = new Student
            {
                Name = name,
                Speciality = speciality,
                Group = group
            };
            model.AddStudent(student);
            OnPropertyChanged(nameof(Students));
        }

        /// <summary>
        /// Метод для обновления значений на графике.
        /// </summary>
        private void UpdateChartValues()
        {
            Values.Clear();
            var specialityDistribution = GetSpecialityDistribution();
            foreach (var speciality in Specialities)
            {
                Values.Add(specialityDistribution.ContainsKey(speciality) ? specialityDistribution[speciality] : 0);
            }
        }

        /// <summary>
        /// Метод для удаления студента по id.
        /// </summary>
        /// <param ID="id">id студента для удаления.</param>
        public bool DeleteStudent(int id)
        {
            var studentToRemove = GetStudents().FirstOrDefault(s => s.Id == id);
            if (studentToRemove != null)
            {
                model.DeleteStudent(id);
                OnPropertyChanged(nameof(Students));
                return true;
            }

            return false;
        }
        /// <summary>
        /// Команда для добавления студента.
        /// </summary>
        public ICommand AddStudentCommand { get; set; }

        /// <summary>
        /// Команда для удаления студента.
        /// </summary>
        public ICommand DeleteStudentCommand { get; set; }
        /// <summary>
        /// команда для обновления студента.
        /// </summary>
        public ICommand UpdateStudentCommand { get; set; }

        /// <summary>
        /// Метод для инициализации команд.
        /// </summary>
        private void InitializeCommands()
        {
            AddStudentCommand = new RelayCommand(AddStudentButtonClick);
            DeleteStudentCommand = new RelayCommand(DeleteSelectedStudent);
            UpdateStudentCommand = new RelayCommand(UpdateStudent);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить студента".
        /// </summary>
        private void AddStudentButtonClick(object sender)
        {
            if (CanAddStudent(sender as Student))
            {
                AddStudent(NameToAdd, SpecToAdd, GroupToAdd);
                NameToAdd = string.Empty;
                SpecToAdd = string.Empty;
                GroupToAdd = string.Empty;
                OnPropertyChanged(nameof(Students));
                UpdateChartValues();
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// Метод для определения, можно ли добавить студента.
        /// </summary>
        private bool CanAddStudent(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NameToAdd) &&
                   !string.IsNullOrWhiteSpace(SpecToAdd) &&
                   !string.IsNullOrWhiteSpace(GroupToAdd);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Удалить студента".
        /// </summary>
        private void DeleteSelectedStudent(object parameter)
        {
            if (SelectedStudent != null)
            {
                int studentId = SelectedStudent.Id;
                if (DeleteStudent(studentId))
                {
                    model.DeleteStudent(studentId);
                    SelectedStudent = null;
                    UpdateChartValues();
                }
            }
        }
        /// <summary>
        /// Метод для получения коллекции студентов.
        /// </summary>
        public ObservableCollection<Student> GetStudents()
        {
            students = new ObservableCollection<Student>(model.LoadStudents().Select(s => new Student
            {
                Id = int.Parse(s[0]),
                Name = s[1],
                Speciality = s[2],
                Group = s[3]
            }));
            return students;
        }


        /// <summary>
        /// Метод для получения распределения по специальностям.
        /// </summary>
        public Dictionary<string, int> GetSpecialityDistribution()
        {
            return model.LoadStudents()
                .GroupBy(s => s[2])
                .ToDictionary(g => g.Key, g => g.Count());
        }
        /// <summary>
        /// Вызывает событие PropertyChanged для указанного свойства.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// получает или устанавливает новое имя
        /// </summary>
        public string NewName
        {
            get { return newName; }
            set
            {
                newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }
        /// <summary>
        /// получает или устанавливает новую группу
        /// </summary>
        public string NewGroup
        {
            get { return newGroup; }
            set
            {
                newGroup = value;
                OnPropertyChanged(nameof(NewGroup));
            }
        }
        /// <summary>
        /// получает или устанавливает новую специальность
        /// </summary>
        public string NewSpec
        {
            get { return newSpec; }
            set
            {
                newSpec = value;
                OnPropertyChanged(nameof(NewSpec));
            }
        }


        /// <summary>
        /// Метод обновления студента
        /// </summary>
        /// <param name="parameter"></param>
        private void UpdateStudent(object parameter)
        {
            if (SelectedStudent != null)
            {
                if (newName == string.Empty)
                {

                }
                else
                {
                    SelectedStudent.Name = NewName;
                }
                if (newGroup == string.Empty)
                {

                }
                else
                {
                    SelectedStudent.Group = NewGroup;
                }
                SelectedStudent.Speciality = NewSpec;
                model.UpdateStudent(SelectedStudent);
                OnPropertyChanged(nameof(Students));
                UpdateChartValues();
            }
        }

        /// <summary>
        /// Метод выбора студента
        /// </summary>
        /// <param name="student"></param>
        public void SelectStudent(Student student)
        {
            SelectedStudent = student;
            NewName = student.Name;
            NewGroup = student.Group;
            NewSpec = student.Speciality;
        }
    }
}
