# wakatime-unity
WakaTime Unity3d (https://unity3d.com/) plugin [UNDER DEV]

Behold, this is not related to Ubuntu's Unity.

# Instructions

## Installation

- Clone the repository into your computer
```
  # Using SSH
  git clone git@github.com:josec89/wakatime-unity.git`

  # Or using HTTPS
  git clone https://github.com/josec89/wakatime-unity.git`
```

- Get the latest version of the submodules (WakaTime python client)
```
  git submodule update --init
```
- Copy the Editor folder into the *Assets* folder of your project

![Copy the Editor folder](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/Copy.png)

- Insert your API key in the WakaTime Window.

![Insert API Key](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/InsertAPIKey.png)
![Set API Key](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/SetAPIKey.png)

- (Windows) If Python is not installed it will Download and Install it.

![Download Python](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/DownloadPython.png)
![Install Python](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/InstallPython.png)
![Installing Python](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/InstallingPython.png)

- Work normally and it will track your time.

# [WakaTime](https://wakatime.com/dashboard) Screenshots

![WakaTime Dashboard](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/WakaTime1.png)

# Troubleshooting

This project depends on the WakaTime client. If this project stops working, try to update the wakatime client located on `Editor/WakaTime/client` (or reinstall this package following the instructions again).