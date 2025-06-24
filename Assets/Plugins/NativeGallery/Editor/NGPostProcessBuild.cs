/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;
#endif

public class NGPostProcessBuild 
{
	private const bool ENABLED = true;

	private const string PHOTO_LIBRARY_USAGE_DESCRIPTION = "Save media to Photos";
	private const bool MINIMUM_TARGET_8_OR_ABOVE = false;

#if UNITY_IOS
#pragma warning disable 0162
	[PostProcessBuild]
	public static void OnPostprocessBuild( BuildTarget target, string buildPath )
	{
		if( !ENABLED )
			return;

		if( target == BuildTarget.iOS )
		{
			string pbxProjectPath = PBXProject.GetPBXProjectPath( buildPath );
			string plistPath = Path.Combine( buildPath, "Info.plist" );

			PBXProject pbxProject = new PBXProject();
			pbxProject.ReadFromFile( pbxProjectPath );

			string targetGUID = pbxProject.TargetGuidByName( PBXProject.GetUnityTargetName() );
	
			if( MINIMUM_TARGET_8_OR_ABOVE )
			{
				pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework Photos" );
				pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework MobileCoreServices" );
				pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework ImageIO" );
			}
			else
			{
				pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-weak_framework Photos" );
				pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework AssetsLibrary" );
				pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework MobileCoreServices" );
				pbxProject.AddBuildProperty( targetGUID, "OTHER_LDFLAGS", "-framework ImageIO" );
			}
	
			pbxProject.RemoveFrameworkFromProject( targetGUID, "Photos.framework" );

			File.WriteAllText( pbxProjectPath, pbxProject.WriteToString() );
			
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString( File.ReadAllText( plistPath ) );

			PlistElementDict rootDict = plist.root;
			rootDict.SetString( "NSPhotoLibraryUsageDescription", PHOTO_LIBRARY_USAGE_DESCRIPTION );
			rootDict.SetString( "NSPhotoLibraryAddUsageDescription", PHOTO_LIBRARY_USAGE_DESCRIPTION );

			File.WriteAllText( plistPath, plist.WriteToString() );
		}
	}
#pragma warning restore 0162
#endif
}
