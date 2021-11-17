using BingMapsRESTToolkit;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Project_Work_WPF.Commands;
using Project_Work_WPF.Models;
using Project_Work_WPF.Navigation;
using Project_Work_WPF.Services;
using Project_Work_WPF.Views;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	class User_Page_ViewModel : BaseViewModel, IPageViewModel
	{
		public bool GetWithDoubleClick { get; set; } = false;

		static bool departure_finished = true;
		public static bool rotate_cliked = false;
		public ApplicationIdCredentialsProvider Provider { get; set; } =
			new ApplicationIdCredentialsProvider(ConfigurationManager.AppSettings["apiKey"]);

		#region Variables

		public Microsoft.Maps.MapControl.WPF.Location center { get; set; }

		Departure currentdeparture = new Departure();

		public double zoomlevel { get; set; }

		List<Pushpin> taxies = new List<Pushpin>();

		public Visibility From_Pushpin_Visibility { get; set; } = Visibility.Collapsed;
		public Visibility To_Pushpin_Visibility { get; set; } = Visibility.Collapsed;

		public Microsoft.Maps.MapControl.WPF.Location From_Pushpin_Location { get; set; }
		public Microsoft.Maps.MapControl.WPF.Location To_Pushpin_Location { get; set; }

		Pushpin Taxi;

		ImageBrush imgB = new ImageBrush();

		MapPolyline routeLine;
		MapPolyline Taxi_routeLine;

		int taxi_bound;
		int route_bound;

		DispatcherTimer timer = new DispatcherTimer();

		private static readonly HttpClient _httpClient = new HttpClient();

		public ObservableCollection<UIElement> Route { get; set; } = new ObservableCollection<UIElement>();
		public ObservableCollection<Departure> Departures { get; set; } = new ObservableCollection<Departure>();

		public string From { get; set; }

		public string To { get; set; } = "Qara Qarayev Baku";

		public string Price { get; set; }

		#endregion

		#region Relay Commands

		public RelayCommand Rotate_Command { get; set; }

		public RelayCommand Get_Taxi_Command { get; set; }

		public RelayCommand<object> Map_DoubleClick_Command { get; set; }

		public RelayCommand Log_Out { get; set; }

		public RelayCommand History_Command { get; set; }

		#endregion

		Predicate<object> departure_finished_object = new Predicate<object>(x => departure_finished == true);
		Predicate<object> Get_Taxi_Predicate = new Predicate<object>(x => departure_finished == true && rotate_cliked == true);

		public static async Task<Response> GetResponse(Uri uri)
		{
			System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
			var response = await client.GetAsync(uri);
			using (var stream = await response.Content.ReadAsStreamAsync())
			{
				DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
				return ser.ReadObject(stream) as Response;
			}
		}

		int counter = 0;

		#region Timer Ticks

		private void Timer_Tick(object sender, EventArgs e)
		{
			Taxi.Location = Taxi_routeLine.Locations[0];
			center = Taxi.Location;
			Taxi_routeLine.Locations.Remove(Taxi_routeLine.Locations[0]);
			counter++;
			if (counter > taxi_bound - 1)
			{
				Route.Add(routeLine);
				counter = 0;
				timer.Tick -= Timer_Tick;
				timer.Tick += Timer_Tick_2;
				timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
				currentdeparture.Date = DateTime.Now;
				currentdeparture.StartTime = DateTime.Now;
				From_Pushpin_Visibility = Visibility.Hidden;
				Route.Remove(Taxi_routeLine);
				timer.Stop();
				timer.Start();
			}
		}

		private void Timer_Tick_2(object sender, EventArgs e)
		{
			Taxi.Location = routeLine.Locations[0];
			center = Taxi.Location;
			routeLine.Locations.Remove(routeLine.Locations[0]);
			counter++;
			if (counter > route_bound - 1)
			{
				Route.Remove(routeLine);
				currentdeparture.EndTime = DateTime.Now;
				currentdeparture.Duration = currentdeparture.EndTime.Subtract(currentdeparture.StartTime);
				Departures.Add(currentdeparture);
				History_Page_ViewModel.Departures = Departures;
				From = string.Empty;
				To = string.Empty;
				Price = string.Empty;
				departure_finished = true;
				rotate_cliked = false;

				To_Pushpin_Visibility = Visibility.Collapsed;
				EvaluationWindow evaluationWindow = new EvaluationWindow();
				evaluationWindow.Height = 280;
				evaluationWindow.Width = 400;
				evaluationWindow.ShowDialog();
				timer.Tick -= Timer_Tick_2;
				timer.Tick += Timer_Tick;
				timer.Stop();
			}
		}

		#endregion

		double Distance = 0;
		double FromLatitude = 0;
		double FromLongitude = 0;

		private async void Rotate()
		{
			Distance = 0;
			rotate_cliked = false;

			if (GetWithDoubleClick)
			{
				Route = new ObservableCollection<UIElement>();
				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
						  User_Page_UserControl.From_Location.Latitude + "," +
						  User_Page_UserControl.From_Location.Longitude + "&wp.1=" +
						  User_Page_UserControl.To_Location.Latitude + "," +
						  User_Page_UserControl.To_Location.Longitude + "&optmz=distance&rpo=Points&key=" +
						  ConfigurationManager.AppSettings["apiKey"];

				var geocodeRequest = new Uri(URL);
				var r = await GetResponse(geocodeRequest);
				route_bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.Length;

				FromLatitude = User_Page_UserControl.From_Location.Latitude;
				FromLongitude = User_Page_UserControl.From_Location.Longitude;

				routeLine = new MapPolyline();
				routeLine.Locations = new LocationCollection();
				routeLine.Stroke = new SolidColorBrush(Colors.Blue);
				routeLine.Opacity = 150;

				for (int i = 0; i < route_bound; i++)
				{

					routeLine.Locations.Add(new Microsoft.Maps.MapControl.WPF.Location
					{
						Latitude = ((Route)
							  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][0],
						Longitude = ((Route)
							  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][1]
					});
				}

				Route.Add(routeLine);
			}

			else
			{
				try
				{
					currentdeparture = new Departure();
					Distance = 0;
					taxies.Clear();
					Route = new ObservableCollection<UIElement>();

					string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=" +
				   $"{From}" + ",MN&wp.1=" + $"{To}" +
				   ",MN&optmz=distance&routeAttributes=routePath&key=" + ConfigurationManager.AppSettings["apiKey"];

					var geocodeRequest = new Uri(URL);
					var r = await GetResponse(geocodeRequest);

					FromLatitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][0];
					FromLongitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][1];


					float currentdeparture_price = (float)(((Route)(r.ResourceSets[0].Resources[0])).TravelDistance * 0.4);
					currentdeparture.Cost = currentdeparture_price.ToString() + " AZN";
					Price = currentdeparture.Cost;
					currentdeparture.Distance = (float)((Route)(r.ResourceSets[0].Resources[0])).TravelDistance;



					var location = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude, FromLongitude);

					From_Pushpin_Location = location;
					From_Pushpin_Visibility = Visibility.Visible;

					routeLine = new MapPolyline();
					routeLine.Locations = new LocationCollection();
					routeLine.Stroke = new SolidColorBrush(Colors.Blue);
					routeLine.Opacity = 150;

					route_bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.Length;

					var FromLatitude_2 =
						((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[route_bound - 1][0];
					var FromLongitude_2 =
						((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[route_bound - 1][1];

					var location_2 = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude_2, FromLongitude_2);

					To_Pushpin_Visibility = Visibility.Hidden;
					To_Pushpin_Location = location_2;
					To_Pushpin_Visibility = Visibility.Visible;

					zoomlevel = ((1 / ((Route)(r.ResourceSets[0].Resources[0])).TravelDistance) * 150);

					center = new Microsoft.Maps.MapControl.WPF.Location((location.Latitude + location_2.Latitude) / 2, (location.Longitude + location_2.Longitude) / 2);

					for (int i = 0; i < route_bound; i++)
					{
						routeLine.Locations.Add(new Microsoft.Maps.MapControl.WPF.Location
						{
							Latitude = ((Route)
							  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][0],
							Longitude = ((Route)
							  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][1]
						});
					}

					Route.Add(routeLine);
				}

				catch (Exception)
				{
					MessageBox.Show("Error Occured !!! Please Try Again");
				}
			}

			try
			{

				string Latitude, Longitude, Latitude_2, Longitude_2, url;
				Uri geocodeRequest_2;
				Response r_2;
				double fromLatitude, fromLongitude;
				int bound_2, index;
				Random random = new Random();

				for (int i = 0; i < 5; i++)
				{
					double a = random.NextDouble() * (0.017 - 0.005) + 0.005;
					double b = random.NextDouble() * (0.017 - 0.005) + 0.005;

					if (random.Next(0, 2) == 0)
					{
						a = a * -1;
					}

					if (random.Next(0, 2) == 0)
					{
						b = b * -1;
					}

					Pushpin pushpin = new Pushpin();

					pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude + a, FromLongitude + b);

					Latitude = pushpin.Location.Latitude.ToString().Replace(',', '.');
					Longitude = pushpin.Location.Longitude.ToString().Replace(',', '.');
					Latitude_2 = FromLatitude.ToString().Replace(',', '.');
					Longitude_2 = FromLongitude.ToString().Replace(',', '.');

					url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					   Latitude + "," +
					   Longitude + "&wp.1=" +
					   Latitude_2 + "," +
					   Longitude_2 + "&optmz=distance&rpo=Points&key=" +
					   ConfigurationManager.AppSettings["apiKey"];

					geocodeRequest_2 = new Uri(url);
					r_2 = await GetResponse(geocodeRequest_2);


					bound_2 = ((Route)(r_2.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.Length;

					index = random.Next(0, bound_2 - 1);

					fromLatitude = ((Route)(r_2.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[index][0];
					fromLongitude = ((Route)(r_2.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[index][1];

					pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(fromLatitude, fromLongitude);

					pushpin.Background = imgB;

					taxies.Add(pushpin);
					Route.Add(pushpin);

					Latitude = pushpin.Location.Latitude.ToString().Replace(',', '.');
					Longitude = pushpin.Location.Longitude.ToString().Replace(',', '.');
					Latitude_2 = FromLatitude.ToString().Replace(',', '.');
					Longitude_2 = FromLongitude.ToString().Replace(',', '.');

					url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					  Latitude + "," +
					  Longitude + "&wp.1=" +
					  Latitude_2 + "," +
					  Longitude_2 + "&optmz=distance&rpo=Points&key=" +
					  ConfigurationManager.AppSettings["apiKey"];

					geocodeRequest_2 = new Uri(url);
					r_2 = await GetResponse(geocodeRequest_2);

					if (Distance == 0)
					{
						Distance = ((Route)(r_2.ResourceSets[0].Resources[0])).TravelDistance;
					}

					if (Distance > ((Route)(r_2.ResourceSets[0].Resources[0])).TravelDistance)
					{
						Distance = ((Route)(r_2.ResourceSets[0].Resources[0])).TravelDistance;
					}
				}

				rotate_cliked = true;
			}

			catch (Exception)
			{
				MessageBox.Show("Error Occured !!! Please Try Again");
			}
		}
		 
		private async void Get_Taxi()
		{
			if (taxies.Count > 0)
			{
				timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
				departure_finished = false;
				string Latitude, Longitude, Latitude_2, Longitude_2, url;
				Uri geocodeRequest;
				Response r;

				foreach (var item in taxies)
				{
					Latitude = item.Location.Latitude.ToString().Replace(',', '.');
					Longitude = item.Location.Longitude.ToString().Replace(',', '.');
					Latitude_2 = FromLatitude.ToString().Replace(',', '.');
					Longitude_2 = FromLongitude.ToString().Replace(',', '.');

					url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					   Latitude + "," +
					   Longitude + "&wp.1=" +
					   Latitude_2 + "," +
					   Longitude_2 + "&optmz=distance&rpo=Points&key=" +
					   ConfigurationManager.AppSettings["apiKey"];

					geocodeRequest = new Uri(url);
					r = await GetResponse(geocodeRequest);

					if (Distance != ((Route)(r.ResourceSets[0].Resources[0])).TravelDistance)
					{
						Route.Remove(item);
					}

					else
					{
						Taxi = item;
					}

				}


				string latitude = Taxi.Location.Latitude.ToString().Replace(',', '.');
				string longitude = Taxi.Location.Longitude.ToString().Replace(',', '.');
				string latitude_2 = FromLatitude.ToString().Replace(',', '.');
				string longitude_2 = FromLongitude.ToString().Replace(',', '.');

				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					latitude + "," +
					longitude + "&wp.1=" +
					latitude_2 + "," +
					longitude_2 + "&optmz=distance&rpo=Points&key=" +
					ConfigurationManager.AppSettings["apiKey"];

				geocodeRequest = new Uri(URL);
				r = await GetResponse(geocodeRequest);

				taxi_bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.Length;

				Taxi_routeLine = new MapPolyline();
				Taxi_routeLine.Locations = new LocationCollection();
				Taxi_routeLine.Stroke = new SolidColorBrush(Colors.Red);
				Taxi_routeLine.StrokeThickness = 2;
				Taxi_routeLine.Opacity = 350;

				for (int i = 0; i < taxi_bound; i++)
				{
					Taxi_routeLine.Locations.Add(new Microsoft.Maps.MapControl.WPF.Location
					{
						Latitude = ((Route)
						  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][0],
						Longitude = ((Route)
						  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][1]
					});
				}

				Route.Remove(routeLine);
				Route.Add(Taxi_routeLine);
				timer.Start();
			}
		}

		public User_Page_ViewModel()
		{

			History_Command = new RelayCommand(
				  x =>
				  {
					  Mediator.Notify("GoToHistory", "");
				  }, departure_finished_object
			);

			Log_Out = new RelayCommand(
				 x =>
				 {
					 Mediator.Notify("GoToLogIn", "");
				 }, departure_finished_object
			);

			Rotate_Command = new RelayCommand(
				a =>
				{
					Rotate();
				}, departure_finished_object
			);

			Get_Taxi_Command = new RelayCommand(
				a =>
				{
					Get_Taxi();
				}, Get_Taxi_Predicate
			);

			center = new Microsoft.Maps.MapControl.WPF.Location(40.4093, 49.8671);
			zoomlevel = 14;
			imgB.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Taxi Icon.png"));
			imgB.Viewport = new Rect(-0.29, -0.3, 1.7, 1.7);
			timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
			timer.Tick += Timer_Tick;
			GetCurrentLocation();
		}

		private static async Task<string> GetIPAddress()
		{
			var ipAddress = await _httpClient.GetAsync($"http://ipinfo.io/ip");
			if (ipAddress.IsSuccessStatusCode)
			{
				var json = await ipAddress.Content.ReadAsStringAsync();
				return json.ToString();
			}
			return "";
		}

		public async void GetCurrentLocation()
		{
			if (Route.Count == 0)
			{
				var ipAddress = await GetIPAddress();

				IpInfo ipInfo = new IpInfo();

				try
				{
					string info = new WebClient().DownloadString("http://ipinfo.io/" + ipAddress);
					ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
					double lat = double.Parse(ipInfo.Loc.Split(',')[0]);
					double lon = double.Parse(ipInfo.Loc.Split(',')[1]);
					lat += 0.001;
					lon += 0.001;
					From_Pushpin_Location = new Microsoft.Maps.MapControl.WPF.Location(lat, lon);
					center = From_Pushpin_Location;

					Uri geocodeRequest = new Uri("http://dev.virtualearth.net/REST/v1/Locations/" + lat.ToString() + "," + lon.ToString() +
						"?key=" + ConfigurationManager.AppSettings["apiKey"]);
					Response r = await GetResponse(geocodeRequest);

					From = ((BingMapsRESTToolkit.Location)r.ResourceSets[0].Resources[0]).Address.AddressLine + " Baku";
					From_Pushpin_Visibility = Visibility.Visible;
				}
				catch (Exception)
				{
				}
			}
		}

	}
}
