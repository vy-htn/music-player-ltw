﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Threading;
using System.Windows.Forms.VisualStyles;
using System.Security;

namespace WpfApp3
{
    class Playlist
    {
        public string name { get; set; }
        public List<string> songsList { get; set; } = new List<string>();
    }


    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       

        private DispatcherTimer timer = new DispatcherTimer();
        private List<string> songList= new List<string>();
        private List<Playlist> playlistList = new List<Playlist>();
        bool isLoop = false;
        bool isRand = false;



        private Random rand = new Random();
        public   MainWindow()
        {
            InitializeComponent();
            slider.Minimum = 0;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick1;
           

            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
            saveButton.IsEnabled = false;


        }

       
        private void Timer_Tick1(object sender, EventArgs e)
        {
            
            slider.Value++;
            if (slider.Value == slider.Maximum)
            {
                timer.Stop();

            }
        }

      

        bool ended = false;
        bool changed = false;
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            slider.Value = 0;
            ended = false;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (ended)
            {
                return;
            }
            if (isLoop)
            {

                isRand = false;
                slider.Value = 0;
                timer.Start();
                ended = false;

            }
            if (isRand)
            {
                

                if (listview.Items.Count > 0)
                {
                    int randomIndex = rand.Next(0, listview.Items.Count);
                    listview.SelectedIndex = randomIndex;
                    timer.Start();


                }
                ended = false;

            }
            if (!isRand && !isLoop)
            {

                timer.Stop();
                if (listview.Items.Count > 0 && listview.SelectedIndex < listview.Items.Count - 1)
                {
                    listview.SelectedIndex++;
                    timer.Start();
                }
                ended = true;


            }


        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            openFileDialog.Multiselect = true;
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               changed = true;
               foreach (string filename in openFileDialog.FileNames )
               {
                    
                    

                    songList.Add(filename);


                    string safeFileName = System.IO.Path.GetFileNameWithoutExtension(filename);
                    listview.Items.Add(safeFileName);


                }







            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedIndex >= 0)
            {
                mediaElement.Play();
                timer.Start();
                pauseButton.IsEnabled = true;
                stopButton.IsEnabled = true;
                nextButton.IsEnabled = true;
                prvButton.IsEnabled = true;
            }
            
            
            

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (listview.SelectedIndex >= 0)
            {
                mediaElement.Pause();
                slider.Value++;
                timer.Stop();
                pauseButton.IsEnabled = false;
            }
           
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (listview.SelectedIndex >= 0)
            {
                mediaElement.Stop();
                timer.Stop();
                slider.Value = 0;
            }
            
           
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Position = TimeSpan.FromSeconds(slider.Value);
            
        }

        
        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            for (int i = 0; i < listview.Items.Count; i++)
            {
                if (listview.SelectedIndex == i)
                {
                    mediaElement.Source = new Uri(songList[i], UriKind.Absolute);
                    mediaElement.LoadedBehavior = MediaState.Manual;
                    slider.Value = 0;
                    txtBlock_songName.Text = System.IO.Path.GetFileNameWithoutExtension(songList[i]);
                    
                }    
                   
            }
           


        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (prvButton.IsEnabled)
            {
                if (listview.SelectedIndex > 0)
                {
                    listview.SelectedIndex--;
                    mediaElement.Play();
                    timer.Start();
                }
            }
            
            
            

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (nextButton.IsEnabled)
            {
                if (listview.Items.Count > 0 && listview.SelectedIndex < listview.Items.Count - 1)
                {
                    listview.SelectedIndex++;
                    mediaElement.Play();
                    timer.Start();
                }
            }
            
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            changed = true;
            var selectedItems = listview.SelectedItems;
            while (selectedItems.Count > 0)
            {

                var selectedItem = selectedItems[0];
                for (int i = 0; i < listview.Items.Count; i++)
                {
                    if (listview.SelectedIndex == i)
                    {


                        for (int j = i; j < songList.Count - 1; j++)
                        {
                            songList[j] = songList[j + 1];

                        }

                        songList[songList.Count - 1] = null;
                        songList.Remove(songList[songList.Count - 1]);



                    }
                }
                listview.Items.Remove(selectedItem);
                slider.Value = 0;
                timer.Stop();
                mediaElement.Pause();
            }

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (infoStackPanel.Visibility == Visibility.Collapsed)
                infoStackPanel.Visibility = Visibility.Visible;
            else
                infoStackPanel.Visibility = Visibility.Collapsed;
        }

        private void loopButton_Click(object sender, RoutedEventArgs e)
        {
            isLoop = !isLoop;
            if (isLoop == false)
            {
                loopButton.Content = "Off";
                
                loopButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 250, 235, 215));

            }
                
            else
            {
                loopButton.Content =" ";
                loopButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 255, 255));
                loopButton.Background = new ImageBrush(new BitmapImage(new Uri(@"E:\VY\LTTQ\WpfApp3\WpfApp3\loop.png")));
                
                
            }
            
            
        }

        private void randButton_Click(object sender, RoutedEventArgs e)
        {
            isRand = !isRand;
            if (isRand)
            {

                randButton.Content = " ";
                randButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 255, 255));
                randButton.Background = new ImageBrush(new BitmapImage(new Uri(@"E:\VY\LTTQ\WpfApp3\WpfApp3\shuffle.png")));


            }

            else
            {
                randButton.Content = "Off";
                randButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 250, 235, 215));
                
            }
                
        }

        private void playlistButton_Click(object sender, RoutedEventArgs e)
        {
            if (DropDownPlaylist.Visibility == Visibility.Collapsed)
                DropDownPlaylist.Visibility = Visibility.Visible;
            else
                DropDownPlaylist.Visibility = Visibility.Collapsed;
        }

        private void addPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = Interaction.InputBox("Enter playlist name");
            if (playlistName == string.Empty )
            {
                System.Windows.Forms.MessageBox.Show("Enter valid playlist name");

            }
            else
            {
                txt_Playlist.Text = playlistName;
                txt_PlaylistHeading.Text = playlistName;
                DropDownListbox.Items.Add(playlistName);



                if (listview.Items.Count > 0)
                {
                    Playlist playlist = new Playlist();
                    playlist.name = playlistName;

                    foreach (var item in songList)
                    {
                        playlist.songsList.Add(item);
                    }
                    playlistList.Add(playlist);


                }
                else
                {
                    Playlist playlist = new Playlist();
                    playlist.name = playlistName;
                    playlistList.Add(playlist);
                }
               
            }
        }

      

        private void DropDownListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



            mediaElement.Pause();
            txtBlock_songName.Text = "";
            txt_Playlist.Text = playlistList[DropDownListbox.SelectedIndex].name;
            txt_PlaylistHeading.Text = playlistList[DropDownListbox.SelectedIndex].name;
            if (playlistList[DropDownListbox.SelectedIndex].songsList != null)
            {
                listview.Items.Clear();
                songList.Clear();
                foreach (var item in playlistList[DropDownListbox.SelectedIndex].songsList)
                {
                    songList.Add(item);
                    listview.Items.Add(System.IO.Path.GetFileNameWithoutExtension(item));

                }
            }
            saveButton.IsEnabled = true;





        }

       
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

                playlistList[DropDownListbox.SelectedIndex].songsList.Clear();
                foreach (var item in songList)
                {
                    playlistList[DropDownListbox.SelectedIndex].songsList.Add(item);

                }
            
            System.Windows.Forms.MessageBox.Show("Saved successfully");



        }

    }
}
