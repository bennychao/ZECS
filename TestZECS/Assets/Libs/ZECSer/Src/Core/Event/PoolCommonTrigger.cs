using System;

namespace ZECS
{
//	public class PoolStartedTrgger : IZTrigger
//	{
//		public PoolStartedTrgger(PoolStartEndEvent onStarted, IZSystem system){
//			onStarted += ()=> system.Execute();
//		}
//		void IZTrigger.Trigger(){
//			
//		}
//	}

	public static class PoolCommonTriggerTools{
		public static void RegisterStartEventTrigger(this EntityPool pool, IZSystem system){
			pool.OnStartEnd += _=> system.Execute();
		}

		public static void RegisterDestoryEventTrigger(this EntityPool pool, IZSystem system){
			pool.OnPreDestory += _=> system.Execute();
		}
	}

//	public static partial class EntityPoolBuilder
//	{
//
//		public static void RegisterAllStartEventTrigger(){
//			
//		}
//	}
}

