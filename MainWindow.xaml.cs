using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Autoclicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _output = null!;
        private bool _isAutoClickOn = false;
        private DispatcherTimer _timer = new();
        private Stopwatch _stopwatch = new();
        private string _time = "00:00:00";
        private readonly UserAPI _userAPI = new();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            foreach (var i in Enumerable.Range(0, 1))
            {
                Loaded += (sender, e) => Output = "XD.";
            }
            _timer.Tick += OnTimerTick;
            _timer.Interval = TimeSpan.FromMilliseconds(Int32.Parse(_intervalValueText.Text));
            Deactivated += MainWindow_Deactivated;
            _userAPI.KeyPressed += OnStopCombinationPressed;
        }
        private void OnStopCombinationPressed(object? sender, EventArgs e)
        {
            ToggleAutoClick();
        }
        private void MainWindow_Deactivated(object? sender, EventArgs e)
        {
            var window = sender as Window;
            if (window is not null)
            {
                window.Topmost = true;
            }
        }
        public string Output
        {
            get { return _output; }
            set
            {
                _output += value + Environment.NewLine;
                OnPropertyChanged();
                _scrollViewer.ScrollToEnd();
            }
        }
        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }
        private void ToggleAutoClick()
        {
            _isAutoClickOn = !_isAutoClickOn;
            Output = "Autoclick mode is " + (_isAutoClickOn ? "on." : "off.");
         
            if (_isAutoClickOn)
            {
                var interval = Int32.Parse(_intervalValueText.Text);
                Output = $"Interval is set to {interval}.";
                _timer.Interval = TimeSpan.FromMilliseconds(interval);
                _stopwatch.Reset();
                _stopwatch.Start();
                _timer.Start();
            }
            else
            {
                var timeSpan = _stopwatch.Elapsed;
                Output = $"Autoclick was on for {String.Format("{0} h {1} min {2} sec.", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds)}";
                _stopwatch.Stop();
                _timer.Stop();
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Output = "Program started successfully.";
            Output = "Autoclick mode is currently " + (_isAutoClickOn ? "on." : "off.");
        }
        private void TextBoxInterval_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleAutoClick();
        }
        private void OnTimerTick(object? sender, EventArgs e)
        {
            var timeSpan = _stopwatch.Elapsed;
            Time = String.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            _userAPI.ClickLeftMouseButton(_userAPI.MousePosition);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName: propertyName));
            }
        }
    }
}