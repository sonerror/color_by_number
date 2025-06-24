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
	public class NGMediaReceiveCallbackAndroid : AndroidJavaProxy
	{
		private object threadLock;

		public string Path { get; private set; }
		public string[] Paths { get; private set; }

		public NGMediaReceiveCallbackAndroid( object threadLock ) : base( "com.yasirkula.unity.NativeGalleryMediaReceiver" )
		{
			Path = string.Empty;
			this.threadLock = threadLock;
		}

		public void OnMediaReceived( string path )
		{
			Path = path;

			lock( threadLock )
			{
				Monitor.Pulse( threadLock );
			}
		}

		public void OnMultipleMediaReceived( string paths )
		{
			if( string.IsNullOrEmpty( paths ) )
				Paths = new string[0];
			else
			{
				string[] pathsSplit = paths.Split( '>' );

				int validPathCount = 0;
				for( int i = 0; i < pathsSplit.Length; i++ )
				{
					if( !string.IsNullOrEmpty( pathsSplit[i] ) )
						validPathCount++;
				}

				if( validPathCount == 0 )
					pathsSplit = new string[0];
				else if( validPathCount != pathsSplit.Length )
				{
					string[] validPaths = new string[validPathCount];
					for( int i = 0, j = 0; i < pathsSplit.Length; i++ )
					{
						if( !string.IsNullOrEmpty( pathsSplit[i] ) )
							validPaths[j++] = pathsSplit[i];
					}

					pathsSplit = validPaths;
				}

				Paths = pathsSplit;
			}
			
			lock( threadLock )
			{
				Monitor.Pulse( threadLock );
			}
		}
	}
}
#endif
