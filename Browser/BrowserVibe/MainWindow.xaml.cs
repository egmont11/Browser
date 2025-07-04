using System.Windows;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;

namespace BrowserVibe
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                // Initialize WebView2
                await WebView.EnsureCoreWebView2Async(null);
                
                // Disable privacy-invasive features
                WebView.CoreWebView2.Settings.IsReputationCheckingRequired = false;
                WebView.CoreWebView2.Settings.AreHostObjectsAllowed = true;
                WebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                WebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                
                // Set up event handlers
                WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                WebView.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
                WebView.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
                
                // Navigate to default page
                WebView.CoreWebView2.Navigate("https://www.google.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            // Update address bar when navigation occurs
            Dispatcher.Invoke(() =>
            {
                AddressBar.Text = WebView.CoreWebView2.Source;
            });
        }

        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // Update button states based on navigation history
            Dispatcher.Invoke(() =>
            {
                BackButton.IsEnabled = WebView.CoreWebView2.CanGoBack;
                ForwardButton.IsEnabled = WebView.CoreWebView2.CanGoForward;
            });
        }

        private void CoreWebView2_DocumentTitleChanged(object sender, object e)
        {
            // Update window title with page title
            Dispatcher.Invoke(() =>
            {
                this.Title = $"{WebView.CoreWebView2.DocumentTitle} - Simple WPF Browser";
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebView.CoreWebView2 != null && WebView.CoreWebView2.CanGoBack)
            {
                WebView.CoreWebView2.GoBack();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebView.CoreWebView2 != null && WebView.CoreWebView2.CanGoForward)
            {
                WebView.CoreWebView2.GoForward();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebView.CoreWebView2 != null)
            {
                WebView.CoreWebView2.Reload();
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrl();
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrl();
            }
        }

        private void NavigateToUrl()
        {
            if (WebView.CoreWebView2 != null)
            {
                string url = AddressBar.Text.Trim();
                
                // Add protocol if missing
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    // Check if it looks like a URL or treat as search
                    if (url.Contains(".") && !url.Contains(" "))
                    {
                        url = "https://" + url;
                    }
                    else
                    {
                        // Treat as DuckDuckGo search (more privacy-friendly)
                        url = "https://duckduckgo.com/?q=" + Uri.EscapeDataString(url);
                    }
                }
                
                WebView.CoreWebView2.Navigate(url);
            }
        }
    }
}