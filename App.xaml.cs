using System.Windows;
using DI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DependencyResolver.Initialize(); // Инициализация зависимостей

        var resolver = new DependencyResolver(); // Создайте экземпляр DependencyResolver
        var mainViewModel = new MainViewModel(resolver); // Передайте его в MainViewModel
        // Инициализация главного окна и передача mainViewModel
    }
}