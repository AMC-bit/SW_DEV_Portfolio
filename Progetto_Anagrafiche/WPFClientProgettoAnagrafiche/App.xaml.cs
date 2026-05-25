using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;

namespace WPFClientProgettoAnagrafiche
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Static instance accessible from anywhere
        public static HttpClient ApiClient { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure HttpClient once at startup
            ApiClient = new HttpClient()
            {
                BaseAddress = new Uri("https://localhost:7251")
            };
        }
    }
}