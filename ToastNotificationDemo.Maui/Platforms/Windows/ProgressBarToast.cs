using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace ToastNotificationDemo.Maui.Platforms.Windows
{
    /// <summary>
    /// A wrapper for the Toast with a Progress Bar. 
    /// <br/>Makes it easier to manage all related data without cluttering other classes.
    /// </summary>
    internal class ProgressBarToast
    {
        public string tag = "weekly-playlist";
        public string group = "downloads";

        private int currentSong = 16;
        private int maxSong = 26;
        private uint sequenceNumber = 0;

        public ProgressBarToast()
        {

        }

        public double GetCurrentProgress()
        {
            return Math.Round((double)currentSong / maxSong, 2);
        }

        public void Show()
        {
            var toast = CreateToast();

            // show it
            ToastNotificationManager.CreateToastNotifier().Show(toast);


            // simulate progress events...
            Task.Run(async () =>
            {
                while (currentSong < maxSong)
                {
                    await Task.Delay(1000);

                    currentSong += 2;
                    sequenceNumber++;

                    // create updated notification data
                    var data = new NotificationData()
                    {
                        SequenceNumber = sequenceNumber,
                        Values =
                        {
                            { "progressValue", GetCurrentProgress().ToString() },
                            { "progressValueString", $"{currentSong}/{maxSong} songs" }
                        }   
                    };

                    // Update the existing notification's data by using tag/group
                    ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
                }



                // create done event

                sequenceNumber++;
                var doneEventData = new NotificationData()
                {
                    SequenceNumber = sequenceNumber,
                    Values =
                    {
                        { "progressStatus", "Done!" }
                    }
                };

                // Update the existing notification's data by using tag/group
                ToastNotificationManager.CreateToastNotifier().Update(doneEventData, tag, group);
            });
        }


        /// <summary>
        /// Create's a ToastContentBuilder with all of the elements we want and a progress bar.
        /// <br/>Taken from: https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/toast-progress-bar?tabs=builder-syntax#using-data-binding-to-update-a-toast
        /// </summary>
        private ToastNotification CreateToast()
        {
            // create the builder
            var builder = new ToastContentBuilder()
                .SetBackgroundActivation()
                .AddArgument("action", "toastClicked")
                .AddText("My Toast Notification")
                .AddText("Check this out, this is a cool toast description!")
                .AddVisualChild(new AdaptiveProgressBar()
                {
                    Title = "Weekly playlist",
                    Value = new BindableProgressBarValue("progressValue"),
                    ValueStringOverride = new BindableString("progressValueString"),
                    Status = new BindableString("progressStatus")
                });

            // get the toast content so we can bind the properties
            var content = builder.GetToastContent();

            // create the toast notification from our toast content
            var toast = new ToastNotification(content.GetXml());

            // set the identifier info so we can find this toast later
            toast.Tag = tag;
            toast.Group = group;

            sequenceNumber = 1;

            // create the notification data, binding it to our Bindable Properties above.
            toast.Data = new NotificationData()
            {
                SequenceNumber = sequenceNumber,
                Values =
                {
                    { "progressValue", GetCurrentProgress().ToString() },
                    { "progressValueString", $"{currentSong}/{maxSong} songs" },
                    { "progressStatus", "Downloading..." },
                }
            };

            return toast;
        }
    }
}
