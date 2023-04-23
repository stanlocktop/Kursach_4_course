using System.ComponentModel;
using System.Windows;
using CourseWork.Services;
using CourseWork.Models;
namespace CourseWork.Views;

public partial class RegisterWindow : Window
{
    private readonly ILogger _logger;
    private readonly AuthenticationService _authenticationService;
    private readonly AuthorizationService _authorizationService;

    public RegisterWindow(AuthenticationService authenticationService,
                          AuthorizationService authorizationService,
                          ILogger logger)
    {
        InitializeComponent();
        _logger = logger;
        _authenticationService = authenticationService;
        _authorizationService = authorizationService;

        App.Worker.ProgressChanged += WorkerProgress;
        Closing += (o, e) => App.Worker.ProgressChanged -= WorkerProgress;
    }

    private void WorkerProgress(object? o, ProgressChangedEventArgs e){
        var workerState = (WorkerState)e.UserState!;
        var window = this;
        Dispatcher.Invoke(() =>
        {
            switch (workerState.WorkerResult)
            {
                case WorkerResult.CannotRenewed:
                    new MainWindow(_authenticationService, _authorizationService, _logger).Show();
                    Close();
                    break;
                case WorkerResult.TokenExpired:
                    new MainWindow(_authenticationService, _authorizationService, _logger).Show();
                    Close();
                    break;
            }
        });
    }

    public void Register_Click(object sender, RoutedEventArgs e)
    {
        var authenticatedUsername = _authenticationService.GetUser(Client.Token)?.Username;
        if (authenticatedUsername is not null)
            _logger.Log($"Пользователь {Username.Text} успешно зарегестрировался {authenticatedUsername}");
        else
            _logger.Log($"Пользователь {Username.Text} прошел регистрацию");
        _authenticationService.Register(Username.Text, Password.Password);

        MessageBox.Show("Вы успешно зарегистрировались!");
        var mainWindow = new MainWindow(_authenticationService, _authorizationService, _logger);
        mainWindow.Show();
        Close();
    }

    public void Back_Click(object sender, RoutedEventArgs e)
    {
        var username = _authenticationService.GetUser(Client.Token)?.Username;
        if (username is not null)
            _logger.Log($"Пользователь {username}  вернулся в главное окно");
        else
            _logger.Log("Пользователь вернулся в главное окно");
        var mainWindow = new MainWindow(_authenticationService, _authorizationService,  _logger);
        mainWindow.Show();
        Close();
    }
}