using HuntTrainAssistant.PluginUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public static class ServiceManager
{
		public static SonarMonitor SonarMonitor;
		public static SettingsWindow SettingsWindow;
		public static ContextMenuManager ContextMenuManager;
		public static LifestreamIPC LifestreamIPC;
		public static TeleporterIPC TeleporterIPC;
		public static TabAetheryteBlacklist TabAetheryteBlacklist;
		public static TabIntegrations TabIntegrations;
		public static Chat2IPC Chat2IPC;
		public static Notificator Notificator;
		public static LFGService LFGService;
		public static AutoRetainerIPC AutoRetainerIPC;
}
