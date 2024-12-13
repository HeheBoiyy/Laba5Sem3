using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;
using Ninject;
using DI;
using EntityModels;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IKernel kernel = new StandardKernel(new SimpleConfigModule());
            DataContext = kernel.Get<Logic>();
        }

        private void dataGridStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridStudents.SelectedItem is Student selectedStudent)
            {
                (DataContext as Logic)?.SelectStudent(selectedStudent);
            }
        }
    }
}