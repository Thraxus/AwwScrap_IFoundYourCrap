using AwwScrap_IFoundYourCrap.Thraxus.Common.Enums;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.Tools.Logging
{
	[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate, priority: int.MinValue)]
	internal class StaticLog : MySessionComponentBase
	{
		private const string GeneralLogName = CommonSettings.StaticGeneralLogName;
		private const string ExceptionLogName = CommonSettings.ExceptionLogName;

		private static Log _generalLog;
		private static Log _exceptionLog;

		private static readonly object GeneralWriteLocker = new object();
		private static readonly object ExceptionWriteLocker = new object();

		/// <inheritdoc />
		public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
		{
			base.Init(sessionComponent);
			_exceptionLog = new Log(ExceptionLogName);
			_generalLog = new Log(GeneralLogName);
			WriteToLog("StaticLogger", "Static log loaded.", LogType.General);
			WriteToLog("StaticLogger", "Exception log loaded.", LogType.Exception);
		}

		/// <inheritdoc />
		protected override void UnloadData()
		{
			WriteToLog("StaticLogger", "Static log closed.", LogType.General);
			WriteToLog("StaticLogger", "Exception log closed.", LogType.Exception);
			lock (ExceptionWriteLocker)
			{
				_exceptionLog?.Close();
			}
			lock (GeneralWriteLocker)
			{
				_generalLog?.Close();
			}
			base.UnloadData();
		}

		public static void WriteToLog(string caller, string message, LogType type, int duration = CommonSettings.DefaultLocalMessageDisplayTime, string color = MyFontEnum.Green)
		{
			switch (type)
			{
				case LogType.Exception:
					WriteException(caller, message, duration, color);
					return;
				case LogType.General:
					WriteGeneral(caller, message, duration, color);
					return;
				default:
					return;
			}
		}

		private static void WriteException(string caller, string message, int duration, string color)
		{
			lock (ExceptionWriteLocker)
			{
				_exceptionLog?.WriteToLog(caller, message, duration, color);
				MyLog.Default.WriteLine($"{caller}: {message}");
			}
		}

		private static void WriteGeneral(string caller, string message, int duration, string color)
		{
			lock (GeneralWriteLocker)
			{
				_generalLog?.WriteToLog(caller, message);
			}
		}
	}
}
