﻿using Plugin.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using UnitechMobileApp.ConnectionHelper;
using UnitechMobileApp.Model;
using UnitechMobileApp.DependencyServices;

namespace UnitechMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadPage : ContentPage
    {
        public LoadPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод, вызывающийся непосредственно перед появлением текущей страницы на экране.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            TryToSignIn();
        }


        /// <summary>
        /// Проверяет подключение к интернету, в случае его наличия выполняет вход, иначе выводит сообщение об ошибке
        /// </summary>
        private async void TryToSignIn()
        {
            bool res = await Connection.IsAvailable();
            if (res)
            {
                SignIn();
            }
            else
            {
                ShowConnectionFailrueDialog();
            }
        }

        /// <summary>
        /// Этот метод вызывается при наличии подключения к интернету. Выполняет авторизацию
        /// </summary>
        private void SignIn()
        {
            var login = CrossSettings.Current.GetValueOrDefault("UserLogin", null);
            var pass = CrossSettings.Current.GetValueOrDefault("UserPassword", null);
            
            bool result = false;

            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(pass))
            {
                Client.Auth(login, pass, out result);
            }

            if (result)
            {
                Navigation.PushModalAsync(new MainPage());
            }
            else
            {
                Navigation.PushModalAsync(new AuthPage());
            }
        }

        /// <summary>
        /// Выводит на экран устройства сообщение об ошибке подключения и предлагает повторить попытку подключения или выйти из приложения
        /// </summary>
        async void ShowConnectionFailrueDialog()
        {
            //todo: перенести весь текст в ресурсы
            bool tryAgain = await DisplayAlert("Ошибка", "Проверьте подключение вашего устройства к сети", "Повторить", "Выход");
            if(tryAgain)
            {
                TryToSignIn();
            }
            else
            {
                ICloseApp closer = DependencyService.Get<ICloseApp>();
                closer.Shutdown();
            }
        }
    }
}