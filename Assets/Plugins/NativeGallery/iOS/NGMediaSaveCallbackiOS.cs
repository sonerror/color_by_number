/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

#if !UNITY_EDITOR && UNITY_IOS
using UnityEngine;

namespace NativeGalleryNamespace
{
	public class NGMediaSaveCallbackiOS : MonoBehaviour
	{
		private static NGMediaSaveCallbackiOS instance;
		private NativeGallery.MediaSaveCallback callback;

		public static void Initialize( NativeGallery.MediaSaveCallback callback )
		{
			if( instance == null )
			{
				instance = new GameObject( "NGMediaSaveCallbackiOS" ).AddComponent<NGMediaSaveCallbackiOS>();
				DontDestroyOnLoad( instance.gameObject );
			}
			else if( instance.callback != null )
				instance.callback( null );

			instance.callback = callback;
		}
		
		public void OnMediaSaveCompleted( string message )
		{
			if( callback != null )
			{
				callback( null );
				callback = null;
			}
		}

		public void OnMediaSaveFailed( string error )
		{
			if( string.IsNullOrEmpty( error ) )
				error = "Unknown error";

			if( callback != null )
			{
				callback( error );
				callback = null;
			}
		}
	}
}
#endif
