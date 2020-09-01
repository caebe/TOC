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
		List<string> PlayersOnline = new List<string>();
		bool firstRun = true;
		DispatcherTimer timer = new DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();
			btnStop.IsEnabled = false;
			getGuilds();
		}

		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			timer.Interval = TimeSpan.FromSeconds(5);
			timer.Tick += timer_Tick;
			timer.Start();

			btnStop.IsEnabled = true;
			btnStart.IsEnabled = false;
			btnStart.Content = "Running";
			tbCurrent.Text = "https://tibiantis.info/stats/guild/" + cbGuilds.SelectedItem.ToString().Replace(" ", "%20");
		}

		private void btnStopConsole_Click(object sender, RoutedEventArgs e)
		{
			timer.Stop();
			btnStop.IsEnabled = false;
			btnStart.IsEnabled = true;
			btnStart.Content = "Start";
			tbTextBox.Clear();
			firstRun = true;
		}

		private void getGuilds()
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

		void timer_Tick(object sender, EventArgs e)
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
							tbTextBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " [ONLINE] " + item.InnerText;
							tbTextBox.Text += Environment.NewLine;

						}
						else
						{
							if (!PlayersOnline.Contains(item.InnerText))
							{
								tbTextBox.Text += DateTime.Now.ToString("HH:mm:ss tt") + " [LOGGED IN] " + item.InnerText;
								tbTextBox.Text += Environment.NewLine;

							}
						}
					}
				}

				PlayersOnline.Clear();

				foreach (var item in table.SelectNodes(".//tr[@class='mu']/td[2]/nick"))
				{
					PlayersOnline.Add(item.InnerText);
				}
				firstRun = false;
			}
		}

		public class MyItem
		{
			public string Name { get; set; }
			public string LastOnline { get; set; }
		}


	}

	public class SoundHandler
	{
		SoundPlayer soundPlayer = new SoundPlayer();
		public void PlaySound()
		{
			string location = AppDomain.CurrentDomain.BaseDirectory + "alarm.wav";
			soundPlayer.SoundLocation = location;
			soundPlayer.Play();
		}
	}
}
