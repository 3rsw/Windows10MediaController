using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Control;
using Windows.Media;
using Windows.UI.ViewManagement;
using System.Threading.Tasks;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BannerMediaController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            prevButton.Click += PrevButton_Click;
            nextButton.Click += NextButton_Click;
            playPauseButton.Click += PlayPauseButton_Click;
            WriteCurrentlyPlayingAsync();
            makeSmall();
            // using Windows.UI.ViewManagement;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            // Set colors
            titleBar.ForegroundColor = Windows.UI.Colors.Black;
            titleBar.BackgroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Black;
            titleBar.InactiveForegroundColor = Windows.UI.Colors.Black;
            titleBar.InactiveBackgroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Black;

            var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            sessionManager.CurrentSessionChanged += SessionManager_CurrentSessionChanged;
            sessionManager.SessionsChanged += SessionManager_SessionsChanged;          


        }

        private async void AddCurrentSessionHandlers()
        {
            var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            if (sessionManager != null && sessionManager.GetCurrentSession() != null)
            {
                sessionManager.GetCurrentSession().PlaybackInfoChanged += MainPage_PlaybackInfoChanged; //Delegate to an instance method cannot have null 'this'
                sessionManager.GetCurrentSession().MediaPropertiesChanged += MainPage_MediaPropertiesChanged;
            }
            
        }

        private void MainPage_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            WriteCurrentlyPlayingAsync();
        }

        private void MainPage_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            WriteCurrentlyPlayingAsync();
        }

        private void SessionManager_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            WriteCurrentlyPlayingAsync();
            AddCurrentSessionHandlers();
        }

        private void SessionManager_CurrentSessionChanged(GlobalSystemMediaTransportControlsSessionManager sender, CurrentSessionChangedEventArgs args)
        { 
            WriteCurrentlyPlayingAsync();
            AddCurrentSessionHandlers();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            var currentSession = sessionManager.GetCurrentSession();
            if (currentSession != null)
            {
                currentSession.TryTogglePlayPauseAsync().GetAwaiter().GetResult();
            }
            
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            var currentSession = sessionManager.GetCurrentSession();
            if (currentSession != null)
            {
                currentSession.TrySkipNextAsync().GetAwaiter().GetResult();
            }
            
        }



        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            var currentSession = sessionManager.GetCurrentSession();
            if (currentSession != null)
            {
                currentSession.TrySkipPreviousAsync().GetAwaiter().GetResult();
            }

        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async Task WriteCurrentlyPlayingAsync()
        {
            var sessionManager = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
            if (sessionManager == null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    songAndArtist.Text = " ";
                    playPauseButton.Content = Symbol.Play;
                });
                return;
            }
            var currentSession = sessionManager.GetCurrentSession();

            

            if (currentSession == null) {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    songAndArtist.Text = " ";
                    playPauseButton.Content = '\uE102';
                });
                return;

            }

            

            var mediaProperties = currentSession.TryGetMediaPropertiesAsync().GetAwaiter().GetResult();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                songAndArtist.Text = mediaProperties.Title + "\n\n" + mediaProperties.Artist;
                if (currentSession.GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                {
                    playPauseButton.Content = '\uE103';

                }
                else
                {
                    playPauseButton.Content = '\uE102';
                }
            });



            //var debug = currentSession.GetPlaybackInfo().PlaybackStatus;

            
                
            

          
        }


        private async void makeSmall()
        {
            //Make picture in picture
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(5, 5));
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Windows.Foundation.Size(5, 5);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
        }


    }
}
