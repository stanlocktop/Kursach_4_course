using System.ComponentModel;
using System.Windows;
using CourseWork.Models;
using CourseWork.Services;

namespace CourseWork.Views;

public partial class SecuredResourceWindow : Window
{
    private readonly AuthenticationService _authenticationService;
    private readonly ILogger _logger;
    private readonly AuthorizationService _authorizationService;

    public SecuredResourceWindow(AuthenticationService authenticationService,
     AuthorizationService authorizationService,
     ILogger logger)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _authenticationService = authenticationService;
        InitializeComponent();
        Init();
    }

    private void Init()
    {
        if(!_authorizationService.Authorize(Client.Token))
        {
            SecuredResource.Text = "У вас нет доступа к этому защищенному ресурсу!";
            return;
        }
        var user = _authenticationService.GetUser(Client.Token);
        if (user is null)
            SecuredResource.Text = "У вас нет доступа к этому защищенному ресурсу!";
        else{
SecuredResource.Text =
                $"Здравствуйте, {user.Username}! У вас есть доступ к этому ресурсу, ваш уровень доступа - {user.AccessLevel}";
                TextBlock.Visibility = Visibility.Visible;
        }
             App.Worker.ProgressChanged += WorkerProgress;
        Closing += (o, e) => App.Worker.ProgressChanged -= WorkerProgress;
        
    }

       private void WorkerProgress(object? o, ProgressChangedEventArgs e)
    {
        var workerState = (WorkerState)e.UserState!;
        var window=this;
            Dispatcher.Invoke(() =>
            {
                switch (workerState.WorkerResult)
                {
                    case WorkerResult.CannotRenewed:
                        MessageBox.Show("Срок действия токена истек, пожалуйста, пройдите авторизацию снова", "Срок действия токена истёк",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        SecuredResource.Text = "Авторизация неуспешна";
                        TextBlock.Visibility = Visibility.Collapsed;
                        break;
                    case WorkerResult.TokenRenewed:
                        SecuredResource.Text = $"Здравствуйте, {workerState.User?.Username}! У вас есть доступ к этому ресурсу, ваш уровень доступа - {workerState.User?.AccessLevel}";
                        TextBlock.Visibility = Visibility.Visible;
                        break;
                    case WorkerResult.TokenExpired:
                        SecuredResource.Text = "Авторизация неуспешна";
                        TextBlock.Visibility = Visibility.Collapsed;
                        break;
                }
            });
    }


    public void Logout_Click(object sender, RoutedEventArgs e)
    {
        _logger.Log($"User {_authenticationService.GetUser(Client.Token)?.Username} Вы вышли");
        _authenticationService.Logout();
        var mainWindow = new MainWindow(_authenticationService, _authorizationService, _logger);
        mainWindow.Show();
        Close();
    }

    public void Back_Click(object sender, RoutedEventArgs e)
    {
        _logger.Log($"User {_authenticationService.GetUser(Client.Token)?.Username} Вернуться в главное окно.");
        var mainWindow = new MainWindow(_authenticationService  , _authorizationService, _logger);
        mainWindow.Show();
        Close();
    }
}