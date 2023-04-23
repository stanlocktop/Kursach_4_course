using System.Windows;
using CourseWork.Models;
using CourseWork.Services;

namespace CourseWork.Views;

public partial class LoginWindow : Window
{
    private readonly ILogger _logger;
    private readonly AuthenticationService _authenticationService;
    private readonly AuthorizationService _authorizationService;

    public LoginWindow(AuthenticationService authenticationService,
                       AuthorizationService authorizationService,
                       ILogger logger)
    {
        _logger = logger;
        _authenticationService = authenticationService;
        _authorizationService = authorizationService;
        InitializeComponent();
    }

    public void Login_Click(object sender, RoutedEventArgs e)
    {
        var authenticatedUsername = _authenticationService.GetUser(Client.Token)?.Username;
        if(authenticatedUsername is not null)
            _logger.Log($"Пользователь {authenticatedUsername} пытался войти через {Username.Text}");
        else
            _logger.Log($"Пользователь {Username.Text} пытался войти через");
        var result = _authenticationService.Login(Username.Text, Password.Password);
        if (result)
        {
            var token = _authenticationService.GetAccessToken(Username.Text, Password.Password);
            Client.Token = token;
            MessageBox.Show("Вы успешно вошли в систему!");
            var mainWindow = new MainWindow(_authenticationService, _authorizationService, _logger);
            mainWindow.Show();
            Close();
        }
        else
        {
            MessageBox.Show("Неправильное имя пользователя или пароль!");
        }
    }

    public void Back_Click(object sender, RoutedEventArgs e)
    {
        var username = _authenticationService.GetUser(Client.Token)?.Username;
        if(username is not null)
            _logger.Log($"Пользователь {username} вернулся в главное окно");
        else
            _logger.Log("Пользователь вернулся в главное окно");
        var mainWindow = new MainWindow(_authenticationService, _authorizationService, _logger);
        mainWindow.Show();
        Close();
    }
}