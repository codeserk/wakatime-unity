using UnityEngine;
using System.Collections;

namespace WakaTime {
	internal static class WakaTimeConstants {
		internal const string CURRENT_CLIENT_VERSION = "4.0.14"; //https://github.com/wakatime/wakatime/blob/master/HISTORY.rst
		internal const string CLIENT_URL = "https://github.com/wakatime/wakatime/archive/master.zip";
		internal const string PLUGIN_NAME = "unity-wakatime";
		internal const string EDITOR_NAME = "unity";
		internal const int TIME_TO_HEARTBEAT = 120;
	}
}