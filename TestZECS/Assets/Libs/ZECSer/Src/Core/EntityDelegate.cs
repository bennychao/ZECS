
using System;

namespace ZECS
{
	public delegate void PoolStartEndEvent(EntityPool pool);
	public delegate void PoolDestoryPreEvent(EntityPool pool);

	public delegate void PoolAllStartEndEvent();
	public delegate void PoolAllDestoryPreEvent();
}

