# Toast Notifications with .Net Maui
This project demonstrates and explains how to use Windows Toast Notifications with .Net Maui

TLDR: Use this to learn how to make any kind of toast notification like the ones from the [Microsoft Documentation](https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/adaptive-interactive-toasts?tabs=builder-syntax)

## Why does this project exist
There is very little documentation on getting Windows Toast Notifications to work with .Net Maui apps. If trying to use them out of the box, it won't work because some important configurations are missing from the project. This will show you what you need to do in order to get it to work. The project also comes with several examples of different Toast Notifications, giving you reference for how you could structure your own.

## Steps to get started
The reason Windows Toast Notifications don't normally work for .Net Maui projects is because 3 important things are missing:
1. Add the nuget package [Microsoft.Toolkit.Uwp.Notifications](https://www.nuget.org/packages/Microsoft.Toolkit.Uwp.Notifications) to your project.
2. Edit the app manifest for the windows project and add support for the ToastActivator. 
3. Subscribe to the ToastActivator somewhere in your code. This is basically `ToastNotificationManagerCompat.OnActivated`

## Enable Toast Activator
The toast activator allows us to subscribe to OnActivated events. This is necessary because it allows us to see when a button is clicked, a dropdown item is selected, text entered, etc. Without this we cannot get any activation info from our toast notifications. This is especially true if your toast element has background activation (it runs in the background and won't open a new window). As mentioned above, this is normally not enabled in a .Net Maui project so things will just flat out not work. To enable this in your project, go to your `Package.appxmanifest` file for the Windows project, located at `Platforms/Windows/Package.appxmanifest`. Right click this file and click "View Code" to edit it manually. If this doesn't work you can navigate to the folder with this file, then edit it with a text editor like Notepad++ or Visual Studio Code. Add the following code to enable it:

At the top where all of the namespaces are located, add these 2 lines:

```xml
xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10" 
xmlns:com="http://schemas.microsoft.com/appx/manifest/com/windows10"
```

This will let your project recognize the code that we're going to type next. Inside of the `<Application>` tag, add the following code:

```xml
      <Extensions>

        <!-- Specify which CLSID to activate when toast clicked -->
        <desktop:Extension Category="windows.toastNotificationActivation">

          <!-- Change Id to your ApplicationIdGuid, which can be found in your .Net Maui csproj -->
          <desktop:ToastNotificationActivation ToastActivatorCLSID="yourApplicationIdGuidHere" /> 
        </desktop:Extension>

        <!--Register COM CLSID LocalServer32 registry key-->
        <com:Extension Category="windows.comServer">
          <com:ComServer>

            <!-- Change Executable to the EXE of your project -->
            <com:ExeServer Executable="yourProject.exe" Arguments="-ToastActivated" DisplayName="Toast activator">
              
              <!-- Change Id to your ApplicationIdGuid, which can be found in your .Net Maui csproj -->
              <com:Class Id="yourApplicationIdGuidHere" DisplayName="Toast activator"/>
            </com:ExeServer>
          </com:ComServer>
        </com:Extension>
      </Extensions>
```

If you need more help you can see how I structured mine here [ToastNotificationDemo.Maui/Platforms/Windows/Package.appxmanifest](https://github.com/gurrenm3/Toast-Notifications-with-.Net-Maui/blob/master/ToastNotificationDemo.Maui/Platforms/Windows/Package.appxmanifest).

After pasting this second part in you'll have to change a 3 values.
1. Under `com:ExeServer` change `Executable="yourProject.exe"` to your actual project exe. See mine at the link above if you are confused.
2. Under `com:Class` change `Id="yourApplicationIdGuidHere"` to your actual applications GUID. The documentation says it can be any string from 2 to ~30 characters long, however I think it's best to use your GUID to prevent any conflicts.
3. Under `desktop:ToastNotificationActivation` change `ToastActivatorCLSID="yourApplicationIdGuidHere"` to your actual applications GUID. Same as above in step 2. See mine at the link above if you're confused.

Now that the ToastActivator is setup you need to subscribe to it once in your app. If you don't do this then anything with Background Activation won't work. I recommend putting the code below in the Constructor for your Windows App, like so:
```
ToastNotificationManagerCompat.OnActivated += (notificationArgs) =>
{
      // you don't need to put anything here if you don't want to. 
      // You just need to subscribe to it in your app
};
```

If you need more help, check out how I did mine [here](https://github.com/gurrenm3/Toast-Notifications-with-.Net-Maui/blob/fe2f7c526c510c00f78a82225ac324a0e2d2eb7d/ToastNotificationDemo.Maui/Platforms/Windows/App.xaml.cs#L28)

That's it. Your .Net Maui app is now able to use Windows Toast Notifications fully without any issues!

## Things to note
- Check this [Microsoft Documentation](https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/adaptive-interactive-toasts?tabs=builder-syntax) to see all the cool things you can do with your Toast Notification.
- This is for the Windows Toast Notifications specifically. There are some cross platform libraries that provide toast notifications like [.Net Maui Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/alerts/toast) which can be downloaded at [this nuget link](https://www.nuget.org/packages/CommunityToolkit.Maui/). While it is cross platform, it has very limited functionality. If you want more complex Toast Notifications you'll have to make one for each platform, and that is why knowing how to use the Windows Toast Notification is so important. 
- For any windows specific code, make sure you put your code in an `#if` directive like so:
```
#if WINDOWS
    // my windows specific code here
#endif
```

## Credits
I had an extremely hard time figuring out how to get this to work, until I found [this repository](https://github.com/emorell96/MauiWithWindowsToasts) by Emore1196. Huge thanks to them for pioneering the steps needed to take in order to get this working
