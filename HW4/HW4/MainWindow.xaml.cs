using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HW4
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        HttpClient hClient;
        public MainWindow()
        {
            hClient = new HttpClient();
            this.DataContext = this;
            InitializeComponent();
        }
        private string _cityName;
        public string CityName
        {
            get { return _cityName; }
            set
            {
                if (value != _cityName)
                {
                    _cityName = value;
                    PropertyChanged?.Invoke(this, new
                    PropertyChangedEventArgs(nameof(CityName)));
                }
            }
        }
        private string _temperature;
        public string Temperature
        {
            get { return _temperature; }
            set
            {
                if (value != _temperature)
                {
                    _temperature = value;
                    PropertyChanged?.Invoke(this, new
                    PropertyChangedEventArgs(nameof(Temperature)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private async void btnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            var url =
            $"http://api.openweathermap.org/data/2.5/weather?q={_cityName}&appid=719224426489343967c35e705deb38ca";
            var resString = await hClient.GetStringAsync(url);
            var resObject = JsonConvert.DeserializeObject<RootObject>(resString);
            Temperature = ToCelsius(resObject.main.temp);
        }

        private string ToCelsius(double temp)
        {
            return (temp - 273.15).ToString();
        }

    }
}
