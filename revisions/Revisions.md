# Revisions

## Installation

### Windows

A dev-signed package is [available on AppCenter](https://install.appcenter.ms/orgs/uno-platform/apps/Ch9-2); follow these steps to install it.

1. Download and extract the content of the `Release_UWP.zip` file.
1. Run **Windows PowerShell** as adminstrator (right-click + run as adminstrator).
1. In the console, navigate to the extracted directory (`cd <path-to-your-extracted-folder>`).
1. You should see a file named `Install.ps1`.
1. Make sure you can run scripts using the following command `set-executionpolicy remotesigned`.
1. Make sure developer mode is enabled on your machine. (Settings > Update & Security > For Developers and select **Developer mode**)
1. Execute the following command `.\Install.ps1`.
1. Proceed through the installation.

### iOS
A release is [available on AppCenter](https://install.appcenter.ms/orgs/uno-platform/apps/Ch9).

### Android
A release is [available on AppCenter](https://install.appcenter.ms/orgs/uno-platform/apps/Ch9-1).

---

## Version 1.0.5

- Replaces in-app styles to use Material styles.
- Updates the navigation architecture to use the `NavigationView`.
- Adds API to get the list of shows.
- Adds support of WebAssembly.
- Adds support of MacOS.
- Switches to fullscreen automatically when the device goes in landscape.

## Version 1.0.4

- Added *Shows* section to select from a set of shows.

## Version 1.0.3

- Added *Visual Studio Toolbox*

## Version 1.0.0

- Initial version.