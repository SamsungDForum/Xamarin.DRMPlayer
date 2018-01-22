using Tizen.Applications;
using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms;
using ElmSharp;

namespace XamarinSamplePlayer
{
    class Program : FormsApplication
    {
        private UriList uriList;
        private PlayerControl playerControl;

        private ContentPage page;
        private StackLayout layout;
        private LoadingIcon loadingIcon;
        private PositionBar positionBar;

        EcoreEvent<EcoreKeyEventArgs> _keyDown;

        protected override void OnCreate()
        {
            base.OnCreate();
            LoadApplication(new App());
            Initialize();
        }

        private void Initialize()
        {
            page = App.Current.MainPage as ContentPage;
            layout = page.Content as StackLayout;
            MessageBox.SetParent(page);
            loadingIcon = new LoadingIcon(layout.Children);
            InitializeKeyEvent();
        }

        private void InitializePlayer()
        {
            var current = uriList.GetCurrentTrack();
            if (current != null)
            {
                playerControl = new PlayerControl(current.uri, MainWindow, loadingIcon, current.usePlayready);
                positionBar = new PositionBar(layout.Children, playerControl);
            }
        }

        private void InitializeKeyEvent()
        {
            _keyDown = new EcoreEvent<EcoreKeyEventArgs>(EcoreEventType.KeyDown, EcoreKeyEventArgs.Create);
            _keyDown.On += KeyEventHandler;
        }

        private void KeyEventHandler(object sender, EcoreKeyEventArgs e)
        {
            var KeyPressedName = e.KeyName;
            Tizen.Log.Info("KEY", KeyPressedName);
            if (ArrowKeyEvent(KeyPressedName) == true)
            {
                return;
            }
            else if (MediaKeyEvent(KeyPressedName) == true)
            {
                return;
            }
        }

        private bool ArrowKeyEvent(string keyPressedName)
        {
            return ContentsControlKeyEvent(keyPressedName);
        }

        private bool ContentsControlKeyEvent(string keyPressedName)
        {
            if (keyPressedName == "Right")
            {
                if (uriList.MoveToNextTrack())
                {
                    var current = uriList.GetCurrentTrack();
                    positionBar.Deactivate();
                    positionBar = null;
                    playerControl.Terminate();
                    playerControl = new PlayerControl(current.uri, MainWindow, loadingIcon, current.usePlayready);
                    positionBar = new PositionBar(layout.Children, playerControl);
                    playerControl.Resume();
                }
                else
                {
                    MessageBox.New("Last Content");
                }
            }
            else if (keyPressedName == "Down")
            {
                positionBar.Activate((uint)playerControl.GetDuration());
            }
            else if (keyPressedName == "Up")
            {
                positionBar.Deactivate();
            }
            else
            {
                return false;
            }
            return true;
        }

        internal bool MediaKeyEvent(string keyPressedName)
        {
            if (keyPressedName == "XF86AudioPlay")
            {
                playerControl.Resume();
            }
            else if (keyPressedName == "XF86AudioPause")
            {
                playerControl.Pause();
            }
            else if (keyPressedName == "XF86AudioStop")
            {
                playerControl.Unprepare();
            }
            else if (keyPressedName == "XF86AudioNext")
            {
                playerControl.Forward();
            }
            else if (keyPressedName == "XF86AudioRewind")
            {
                playerControl.Backward();
            }
            else
            {
                return false;
            }
            return true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            playerControl.Resume();
        }

        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            //This function is called when application is launched.
            var args = e.ReceivedAppControl.ExtraData;
            string value;
            string uri = null;
            Tizen.Log.Info("KEY", "AppControl");

            if (args.TryGet("MEDIA_URI", out value) == true)
            {
                Tizen.Log.Info("KEY", value);
                uri = value;
            }

            if (uri != null)
            {
                if (uriList == null)
                {
                    uriList = new UriList(new string[0]);
                }
                uriList.Add(uri);
            }
            if (uriList == null)
            {
                //use default contents list
                uriList = new UriList();
            }

            if (playerControl == null)
            {
                InitializePlayer();
            }

            base.OnAppControlReceived(e);
        }

        protected override void OnTerminate()
        {
            playerControl.Terminate();
            if (playerControl != null)
            {
                playerControl.Terminate();
            }
            base.OnTerminate();
        }

        static void Main(string[] args)
        {
            var app = new Program();
            Forms.Init(app);
            app.Run(args);
        }
    }
}
