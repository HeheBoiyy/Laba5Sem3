using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EntityModels
{
   public class Student : INotifyPropertyChanged
        {
            private int id;
            private string name;
            private string speciality;
            private string group;

            public int Id
            {
                get => id;
                set
                {
                    id = value;
                    OnPropertyChanged();
                }
            }

            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    OnPropertyChanged();
                }
            }

            public string Speciality
            {
                get => speciality;
                set
                {
                    speciality = value;
                    OnPropertyChanged();
                }
            }

            public string Group
            {
                get => group;
                set
                {
                    group = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public Student()
            {

            }
        }
}
