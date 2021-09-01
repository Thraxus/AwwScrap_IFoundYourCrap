using AwwScrap_IFoundYourCrap.Thraxus.Common.Enums;
using AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.Tools.Logging;
using VRage.Game.Components;
using VRage.ObjectBuilders;

namespace AwwScrap_IFoundYourCrap.Thraxus.Common.BaseClasses
{
	internal abstract class BaseGameLogicComp : MyGameLogicComponent
	{
		protected string EntityName = "PlaceholderName";
		protected long EntityId = 0L;
		protected long Ticks;

		public void WriteToLog(string caller, string message, LogType logType)
		{
			switch (logType)
			{
				case LogType.General:
					GeneralLog(caller, message);
					return;
				case LogType.Exception:
					ExceptionLog(caller, message);
					return;
				default:
					return;
			}
		}

		public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
		{
			// Always return base.GetObjectBuilder(); after your code! 
			// Do all saving here, make sure to return the OB when done;
			return base.GetObjectBuilder(copy);
		}

		public override void UpdateBeforeSimulation()
		{
			Ticks++;
			TickTimer();
			base.UpdateBeforeSimulation();
		}

		protected abstract void TickTimer();

		private void GeneralLog(string caller, string message)
		{
			StaticLog.WriteToLog($"{EntityName} ({EntityId}): {caller}", message, LogType.General);
		}

		private void ExceptionLog(string caller, string message)
		{
			StaticLog.WriteToLog($"{EntityName} ({EntityId}): {caller}", $"Exception! {message}", LogType.Exception);
		}
	}
}
