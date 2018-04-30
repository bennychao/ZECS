using System;

namespace ZECS
{
	[DontShowInComponents]
	[System.Serializable]
	public class StartEventSystem : ZSystem
	{
		
		public StartEventSystem ()
		{
		}

		protected override void OnStart(){
			base.entity.pool.RegisterStartEventTrigger(this);
		}



	}

	[DontShowInComponents]
	[System.Serializable]
	public class StopEventSystem : ZSystem
	{

		public StopEventSystem ()
		{
		}

		protected override void OnStart(){
			base.entity.pool.RegisterDestoryEventTrigger(this);
		}
	}
}

