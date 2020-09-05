using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace TOC
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool firstRun = true;
		private readonly List<string> PlayersOnline = new List<string>();
		private readonly List<string> NextPlayersOnline = new List<string>();

		readonly DispatcherTimer timer = new DispatcherTimer();
		readonly SoundPlayer soundPlayer = new SoundPlayer();

		public MainWindow()
		{
			InitializeComponent();
			btnStop.IsEnabled = false;
			GetGuilds();
		}

		private void BtnStart_Click(object sender, RoutedEventArgs e)
		{
			timer.Interval = TimeSpan.FromSeconds(5);
			timer.Tick += Timer_Tick;
			timer.Start();

			btnStop.IsEnabled = true;
			btnStart.IsEnabled = false;
			cbGuilds.IsEnabled = false;
			btnStart.Content = "Running";

			tbCurrent.Text = "https://tibiantis.info/stats/guild/" + cbGuilds.SelectedItem.ToString().Replace(" ", "%20");
			tbTextBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " [STARTED] - REFRESHING EVERY 5 SECONDS" + Environment.NewLine;
			tbTextBox.Text += DateTime.Now.ToString("HH:mm:ss tt") +  " [INFO] MAX DELAY 1 MIN (SERVERLIMITATION)" + Environment.NewLine;
			tbTextBox.Text +=  "---------------------------------------------------------------" + Environment.NewLine;
		}
			private void BtnStopConsole_Click(object sender, RoutedEventArgs e)
		{
			timer.Stop();
			btnStop.IsEnabled = false;
			btnStart.IsEnabled = true;
			btnStart.Content = "Start";
			cbGuilds.IsEnabled = true;
			tbTextBox.Clear();
			firstRun = true;
			PlayersOnline.Clear();
			NextPlayersOnline.Clear();
		}

		private void GetGuilds()
		{
			tbTextBox.Clear();
			cbGuilds.Items.Clear();
			var url = "https://tibiantis.online/?page=guilds";
			var web = new HtmlWeb();
			var doc = web.Load(url);
			HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[@class='tabi']");

			if (table != null)
			{
				foreach (var guild in table.SelectNodes(".//tr/td"))
				{
					cbGuilds.Items.Add(guild.InnerText);
				}
			}
			else
			{
				tbTextBox.Text = "null ref";
			}

			cbGuilds.Items.Remove("Guilds");
			if (cbGuilds.SelectedIndex == -1)
			{
				cbGuilds.SelectedIndex = 0;
			}
		}

		void Timer_Tick(object sender, EventArgs e)
		{
			var url = "https://tibiantis.info/stats/guild/" + cbGuilds.SelectedItem.ToString().Replace(" ", "%20");

			//url = url.
			var web = new HtmlWeb();
			var doc = web.Load(url);

			HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[2]");

			if (table != null && table.SelectNodes(".//tr[@class='mu']/td[2]/nick") != null)
			{
				foreach (var item in table.SelectNodes(".//tr[@class='mu']/td[2]/nick"))
				{
					if (firstRun)
					{
						if (!PlayersOnline.Contains(item.InnerText))
						{
							tbTextBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " [ALREADY ONLINE] " + item.InnerText;
							tbTextBox.Text += Environment.NewLine;
							PlayersOnline.Add(item.InnerText);
							NextPlayersOnline.Add(item.InnerText);
						}
					}
					else
					{
						NextPlayersOnline.Add(item.InnerText);

						if (!PlayersOnline.Contains(item.InnerText))
						{
							tbTextBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " [LOGGED IN] " + item.InnerText;
							tbTextBox.Text += Environment.NewLine;
							PlayLogin();
						}
					}
				}
			}

			PlayersOnline.Clear();
			PlayersOnline.AddRange(NextPlayersOnline);
			NextPlayersOnline.Clear();
			firstRun = false;
		}

		public void PlayLogin()
		{
			string location = AppDomain.CurrentDomain.BaseDirectory + "login.wav";
			soundPlayer.SoundLocation = location;
			soundPlayer.Play();
		}
	}
}
