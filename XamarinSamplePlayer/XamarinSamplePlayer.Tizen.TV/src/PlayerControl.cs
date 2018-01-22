using Tizen.Multimedia;
using Tizen.TV.Multimedia;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ElmSharp;

namespace XamarinSamplePlayer
{
    internal class PlayerControl
    {
        private const string licenseServer = "http://playready.directtaps.net/pr/svc/rightsmanager.asmx";

        private Tizen.TV.Multimedia.Player player;
        private DRMManager drm;
        private Window window;
        private string uriSource;
        private IUIUpdate loadingUpdater;
        private int interruptedPosition;
        
        internal PlayerControl(string source, Window window, IUIUpdate loadingUpdater = null, bool usePlayready = false)
        {
            uriSource = source;
            this.window = window;
            player = new Tizen.TV.Multimedia.Player();
            player.SetSource(new MediaUriSource(uriSource));
            
            player.Display = new Display(window);
            player.PlaybackCompleted += (s, _ ) =>
            {
                player.Stop();
                Unprepare();
            };
            player.PlaybackInterrupted += (s, _) =>
            {
                Tizen.Log.Info("KEY", "Interrupted");
                interruptedPosition = player.GetPlayPosition();
                Unprepare();
            };
            player.ErrorOccurred += (s, e) =>
            {
                Tizen.Log.Info("KEY", e.ToString());
            };
            
            if (loadingUpdater != null)
            {
                this.loadingUpdater = loadingUpdater;
                player.BufferingProgressChanged += BufferingHandler;
            }
            if (usePlayready)
            {
                try
                {
                    drm = DRMManager.CreateDRMManager(DRMType.Playready);
                    drm.AddProperty("LicenseServer", licenseServer);
                    drm.AddProperty("DeleteLicenseAfterUse", true);
                    drm.AddProperty("GetChallenge", true);
                }
                catch (ArgumentException e)
                {
                    if (drm != null)
                    {
                        drm.Dispose();
                    }
                    MessageBox.New(e.ToString());
                }
                catch (PlatformNotSupportedException e)
                {
                    MessageBox.New(e.ToString());
                }
            }
        }

        private void BufferingHandler(object s, BufferingProgressChangedEventArgs e)
        {
            var percent = e.Percent;
            Tizen.Log.Info("KEY", "Buffering " + percent);
            if (percent > 1 && percent < 100)
            {
                loadingUpdater?.Update(true);
            }
            else if (percent >= 100)
            {
                loadingUpdater?.Update(false);
            }
        }

        internal void Resume()
        {
            var state = player.State;
            if (state == PlayerState.Idle)
            {
                player.SetSource(new MediaUriSource(uriSource));
                player.Display = new Display(window);
                if (drm != null)
                {
                    drm.Init("XamarinSamplePlayer");
                    drm.Url = uriSource;
                    drm.LicenseRequested += (s, e) =>
                    {
                        Func<string, string> httpPost = param =>
                        {
                            WebRequest req = WebRequest.Create(licenseServer);

                            req.ContentType = "text/xml";
                            req.Method = "POST";

                            byte[] bytes = Encoding.ASCII.GetBytes(param);
                            req.ContentLength = bytes.Length;

                            using (Stream os = req.GetRequestStream())
                                os.Write(bytes, 0, bytes.Length);

                            return new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd().Trim();
                        };

                        string base64DecodedChallengeData = Encoding.ASCII.GetString(Convert.FromBase64String(e.ChallengeData));
                        string licenseData = httpPost(base64DecodedChallengeData);
                        drm.InstallLicense(licenseData);
                    };
                    drm.Open();

                    player.SetDrm(drm);
                }
                loadingUpdater?.Update(true);
                player.PrepareAsync().ContinueWith((t) =>
                {
                    PrepareContinueWithStart(t);
                    if (interruptedPosition > 0)
                    {
                        player.SetPlayPositionAsync(interruptedPosition, true);
                        interruptedPosition = 0;
                    }
                });
            }
            else if (state == PlayerState.Ready || state == PlayerState.Paused)
            {
                player.Start();
            }
        }

        internal void Pause()
        {
            var state = player.State;
            if (state == PlayerState.Playing)
            {
                player.Pause();
            }
        }

        internal void Stop()
        {
            var state = player.State;
            if (state == PlayerState.Playing || state == PlayerState.Paused)
            {
                player.Stop();
            }
        }

        internal void Forward()
        {
            try
            {
                var duration = player.StreamInfo.GetDuration();
                var position = Math.Min(duration, player.GetPlayPosition() + 10000);

                loadingUpdater?.Update(true);
                player.SetPlayPositionAsync(position, true).ContinueWith((_) =>
                loadingUpdater?.Update(false));
            }
            catch (InvalidOperationException)
            { }
        }

        internal void SetPosition(int position)
        {
            try
            {
                loadingUpdater?.Update(true);
                player.SetPlayPositionAsync(position, true).ContinueWith((_) =>
                loadingUpdater?.Update(false));
            }
            catch (InvalidOperationException)
            { }
        }

        internal void Backward()
        {
            try
            {
                var position = Math.Max(0, player.GetPlayPosition() - 10000);

                loadingUpdater?.Update(true);
                player.SetPlayPositionAsync(position, true).ContinueWith((_) =>
                loadingUpdater?.Update(false));
            }
            catch (InvalidOperationException)
            { }
        }

        internal Task PrepareStart()
        {
            loadingUpdater?.Update(true);
            return player.PrepareAsync().ContinueWith((t) => 
            {
                PrepareContinueWithStart(t);
            });
        }

        private void PrepareContinueWithStart(Task task)
        {
            loadingUpdater?.Update(false);
            if (task.Status == TaskStatus.Faulted)
            {
                var e = task.Exception.InnerException;
                if (e is InvalidOperationException)
                {
                    MessageBox.New(e.Message);
                }
                return;
            }
            player.Start();
        }

        internal void Terminate()
        {
            try
            {
                player.Unprepare();
                if (drm != null)
                {
                    drm.Close();
                }
            }
            catch (InvalidOperationException)
            { }
            finally
            {
                if (drm != null)
                {
                    drm.Dispose();
                }
                player.Dispose();
            }
        }

        internal void Unprepare()
        {
            try
            {
                player.BufferingProgressChanged -= BufferingHandler;
                player.Unprepare();
                if (drm != null)
                {
                    drm.Close();
                }
            }
            catch (InvalidOperationException)
            { }
        }

        internal int GetCurrentPosition()
        {
            if (CheckStateReady(player.State) == false)
            {
                return 0;
            }
            return player.GetPlayPosition();
        }

        internal int GetDuration()
        {
            if (CheckStateReady(player.State) == false)
            {
                return 0;
            }
            return player.StreamInfo.GetDuration();
        }

        private bool CheckStateReady(PlayerState state)
        {
            if (state == PlayerState.Idle || state == PlayerState.Preparing)
            {
                return false;
            }
            return true;
        }
    }
}
