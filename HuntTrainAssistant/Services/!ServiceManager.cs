using HuntTrainAssistant.PluginUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public static class ServiceManager
{
		public static SonarMonitor SonarMonitor { get; private set; }
		public static SettingsWindow SettingsWindow { get; private set; }
		public static ContextMenuManager ContextMenuManager { get ; private set; }
		public static LifestreamIPC LifestreamIPC { get; private set; }
		public static TeleporterIPC TeleporterIPC { get; private set; }
		public static TabAetheryteBlacklist TabAetheryteBlacklist { get; private set; }
		public static TabIntegrations TabIntegrations { get; private set; }
		public static Chat2IPC Chat2IPC { get; private set; }
		public static Notificator Notificator { get; private set; }
		public static LFGService LFGService { get; private set; }
}
