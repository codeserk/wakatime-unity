# wakatime-unity
WakaTime Unity3d (https://unity3d.com/) plugin

Behold, this is not related to Ubuntu's Unity.

# Installation

## Option 1: Quick Install Version
[**DOWNLOAD & Import unitypackage.**](https://github.com/TheLouisHong/wakatime-unity/releases)

*If this option does not work try option 2.*

## Option 2: Manual Installation Alternative

### 1. Clone the repository into your computer
```
  # Using SSH
  git clone git@github.com:josec89/wakatime-unity.git`

  # Or using HTTPS
  git clone https://github.com/josec89/wakatime-unity.git`
```

### 2. Get the latest version of the submodules (WakaTime python client)
```
  git submodule update --init
```
### 3. Copy the Editor folder into the root *Assets* folder of your project

**(The Editor folder MUST be in the root of Assets)**

![Copy the Editor folder](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/Copy.png)

### 4. Insert your API key in the WakaTime Window.

![Insert API Key](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/InsertAPIKey.png)
![Set API Key](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/SetAPIKey.png)

### 5. (Windows) If Python is not installed it will Download and Install it.

![Download Python](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/DownloadPython.png)
![Install Python](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/InstallPython.png)
![Installing Python](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/InstallingPython.png)

### 6.  Work normally and it will track your time.

# [WakaTime](https://wakatime.com/dashboard) Screenshots

![WakaTime Dashboard](https://raw.githubusercontent.com/josec89/wakatime-unity/master/Screenshots/WakaTime1.png)

# Troubleshooting

This project depends on the WakaTime client. If this project stops working, try to update the wakatime client located on `Editor/WakaTime/client` (or reinstall this package following the instructions again).
