using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using ToastNotificationDemo.Maui.Platforms.Windows;
using Windows.UI.Notifications;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ToastNotificationDemo.Maui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    public Action<string> ShowPopup { get; set; }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();

        // need to add this because otherwise setting background activation does nothing.
        ToastNotificationManagerCompat.OnActivated += (notificationArgs) =>
        {
            // this will run everytime ToastNotification.Activated is called,
            // regardless of what toast is clicked and what element is clicked on.
            // Works for all types of ToastActivationType so long as the Windows app manifest
            // has been updated to support ToastNotifications. 

            // you can check your args here, however I'll be doing mine below to keep it cleaner.
            // With so many ToastNotifications it would be messy to check all of them here.

            Debug.WriteLine($"A ToastNotification was just activated! Arguments: {notificationArgs.Argument}");

            // using the code below to show a popup from MainPage, saying that the toast itself was clicked.
            if (notificationArgs.Argument.Contains("action=toastClicked"))
                ShowPopup?.Invoke("The Toast was clicked!");
        };
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    /// <summary>
    /// Create's a ToastContentBuilder with all of the main elements we want.
    /// </summary>
    /// <returns></returns>
	public ToastContentBuilder CreateBasicToast()
    {
        var builder = new ToastContentBuilder()
            .SetBackgroundActivation()
            .AddArgument("action", "toastClicked")
            .AddText("My Toast Notification")
            .AddText("Check this out, this is a cool toast description!");

        return builder;
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and an Ok button.
    /// </summary>
    /// <returns></returns>
	public void ShowToastOkBtn()
    {
        var builder = CreateBasicToast()
            .AddButton(new ToastButton()
                .SetContent("Ok")
                .AddArgument("action", "ok")
                .SetBackgroundActivation());

        builder.Show(toast => toast.Activated += (sender, args) =>
        {
            if (args is ToastActivatedEventArgs toastArgs && toastArgs.Arguments.Contains("action=ok"))
                ShowPopup?.Invoke("Ok Button Clicked!");
        });
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and a Yes and No button.
    /// </summary>
    /// <returns></returns>
    public void ShowToastYesNoBtn()
    {
        var builder = CreateBasicToast()
            .AddButton(new ToastButton()
                .SetContent("Yes")
                .AddArgument("action", "yes")
                .SetBackgroundActivation())
            .AddButton(new ToastButton()
                .SetContent("No")
                .AddArgument("action", "no")
                .SetBackgroundActivation());

        builder.Show(toast => toast.Activated += (sender, args) =>
        {
            var toastArgs = args as ToastActivatedEventArgs;
            if (toastArgs.Arguments.Contains("action=yes"))
                ShowPopup?.Invoke("Yes Button Clicked!");
            else if (toastArgs.Arguments.Contains("action=no"))
                ShowPopup?.Invoke("No Button Clicked!");
        });
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and an Image
    /// </summary>
    /// <returns></returns>
    public ToastContentBuilder CreateToastWithImage(bool useHeroImage)
    {
        var builder = CreateBasicToast();

        var uri = new Uri("https://picsum.photos/360/202?image=1043");
        if (!useHeroImage)
            builder.AddInlineImage(uri);
        else
            builder.AddHeroImage(uri);

        return builder;
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want, an Image, and thumbs up/down buttons
    /// </summary>
    /// <returns></returns>
    public void ShowToastWithImageAndThumbs()
    {
        var builder = CreateToastWithImage(true)
            .AddButton(new ToastButton()
                .SetContent("Thumbs up")
                .AddArgument("action", "thumbsUp")
                .SetBackgroundActivation())
            .AddButton(new ToastButton()
                .SetContent("Thumbs Down")
                .AddArgument("action", "thumbsDown")
                .SetBackgroundActivation());

        builder.Show(toast => toast.Activated += (sender, args) =>
        {
            var toastArgs = args as ToastActivatedEventArgs;
            if (toastArgs.Arguments.Contains("action=thumbsUp"))
                ShowPopup?.Invoke("Thumbs Up Button Clicked!");
            else if (toastArgs.Arguments.Contains("action=thumbsDown"))
                ShowPopup?.Invoke("Thumbs Down Button Clicked!");
        });
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and a dropdown.
    /// </summary>
    /// <returns></returns>
	public void ShowToastWithDropdown()
    {
        var builder = CreateBasicToast()
            .AddButton(new ToastButton()
                .SetContent("Select")
                .AddArgument("action", "select")
                .SetBackgroundActivation())

            // the ID "options" becomes the key that we use to access the selected value.
            .AddToastInput(new ToastSelectionBox("options")
            {
                // default item is based off of the ID, not the value. Value is just for display.
                DefaultSelectionBoxItemId = "lunch",
                Items =
                {
                    // the ID is what gets passed as an argument
                    // the value is the text that gets displayed in the app.
                    new ToastSelectionBoxItem("breakfast", "Breakfast"),
                    new ToastSelectionBoxItem("lunch", "Lunch"),
                    new ToastSelectionBoxItem("dinner", "Dinner"),
                }
            });

        builder.Show(toast => toast.Activated += (sender, args) =>
        {
            if (args is ToastActivatedEventArgs toastArgs && toastArgs.Arguments.Contains("action=select"))
            {
                // "options" is the ID of our ToastSelectionBox.
                // If we had more than one type of user input, you'd use the ID of the UserInput you wanted.
                string selectionId = "options";
                var selectedItemId = toastArgs.UserInput[selectionId];
                ShowPopup?.Invoke($"Select Button Clicked!\nDropdown Item Selected: {selectedItemId}");
            }
        });
    }

    public void ShowToastWithProgressBar()
    {
        new ProgressBarToast().Show();
    }
}

