/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

#if !UNITY_EDITOR && UNITY_ANDROID
using System.Threading;
using UnityEngine;

namespace NativeGalleryNamespace
{
	public class NGPermissionCallbackAndroid : AndroidJavaProxy
	{
		private object threadLock;
		public int Result { get; private set; }

		public NGPermissionCallbackAndroid( object threadLock ) : base( "com.yasirkula.unity.NativeGalleryPermissionReceiver" )
		{
			Result = -1;
			this.threadLock = threadLock;
		}

		public void OnPermissionResult( int result )
		{
			Result = result;

			lock( threadLock )
			{
				Monitor.Pulse( threadLock );
			}
		}
	}
}
#endif
