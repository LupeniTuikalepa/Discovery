// Amplify Impostors
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.IO;
using UnityEditor;
using UnityEngine;

namespace AmplifyImpostors
{
	public class Preferences
	{
	#if UNITY_EDITOR
		public enum ShowOption
		{
			Always = 0,
			OnNewVersion = 1,
			Never = 2
		}

		private static readonly GUIContent StartUp = new GUIContent( "Show start screen on Unity launch", "You can set if you want to see the start screen everytime Unity launches, only just when there's a new version available or never." );
		private static readonly GUIContent AutoSRP = new GUIContent( "Auto import SRP shaders", "By default Amplify Impostors checks for your SRP version and automatically imports compatible shaders.\nTurn this OFF if you prefer to import them manually." );

		public static readonly string PrefGlobalFolder = "IMPOSTORS_GLOBALFOLDER";
		public static readonly string PrefGlobalRelativeFolder = "IMPOSTORS_GLOBALRELATIVEFOLDER";
		public static readonly string PrefGlobalDefault = "IMPOSTORS_GLOBALDEFAULT";
		public static readonly string PrefGlobalTexImport = "IMPOSTORS_GLOBALTEXIMPORT";
		public static readonly string PrefGlobalCreateLodGroup = "IMPOSTORS_GLOBALCREATELODGROUP ";
		public static readonly string PrefGlobalGBuffer0Name = "IMPOSTORS_GLOBALGBUFFER0SUFFIX";
		public static readonly string PrefGlobalGBuffer1Name = "IMPOSTORS_GLOBALGBUFFER1SUFFIX";
		public static readonly string PrefGlobalGBuffer2Name = "IMPOSTORS_GLOBALGBUFFER2SUFFIX";
		public static readonly string PrefGlobalGBuffer3Name = "IMPOSTORS_GLOBALGBUFFER3SUFFIX";
		public static readonly string PrefGlobalGBuffer4Name = "IMPOSTORS_GLOBALGBUFFER4SUFFIX";
		public static readonly string PrefGlobalGBuffer5Name = "IMPOSTORS_GLOBALGBUFFER5SUFFIX";
		public static readonly string PrefGlobalBakingOptions = "IMPOSTORS_GLOBALBakingOptions";
		public static readonly string PrefGlobalStartUp = "IMPOSTORS_GLOBALSTARTUP";
		public static readonly string PrefGlobalAutoSRP = "IMPOSTORS_GLOBALAUTOSRP";

		public static readonly string PrefDataImpType = "IMPOSTORS_DATAIMPTYPE";
		public static readonly string PrefDataTexSizeLocked = "IMPOSTORS_DATATEXSIZEXLOCKED";
		public static readonly string PrefDataTexSizeSelected = "IMPOSTORS_DATATEXSIZEXSELECTED";
		public static readonly string PrefDataTexSizeX = "IMPOSTORS_DATATEXSIZEX";
		public static readonly string PrefDataTexSizeY = "IMPOSTORS_DATATEXSIZEY";
		public static readonly string PrefDataDecoupledFrames = "IMPOSTORS_DATADECOUPLEDFRAMES";
		public static readonly string PrefDataXFrames = "IMPOSTORS_DATAXFRAMES";
		public static readonly string PrefDataYFrames = "IMPOSTORS_DATAYFRAMES";
		public static readonly string PrefDataPixelBleeding = "IMPOSTORS_DATAPIXELBLEEDING";

		public static readonly string PrefDataTolerance = "IMPOSTORS_DATATOLERANCE ";
		public static readonly string PrefDataNormalScale = "IMPOSTORS_DATANORMALSCALE";
		public static readonly string PrefDataMaxVertices = "IMPOSTORS_DATAMAXVERTICES";

		public static readonly string DefaultAlbedoName = "_AlbedoAlpha";
		public static readonly string DefaultSpecularName = "_SpecularSmoothness";
		public static readonly string DefaultNormalName = "_NormalDepth";
		public static readonly string DefaultEmissionName = "_EmissionOcclusion";
		public static readonly string DefaultOcclusionName = "_Occlusion";
		public static readonly string DefaultPositionName = "_Position";

		public static bool GlobalDefaultMode = EditorPrefs.GetBool( PrefGlobalDefault, false );
		public static string GlobalFolder = EditorPrefs.GetString( PrefGlobalFolder, "" );
		public static string GlobalRelativeFolder = EditorPrefs.GetString( PrefGlobalRelativeFolder, "" );
		public static int GlobalTexImport = EditorPrefs.GetInt( PrefGlobalTexImport, 0 );
		public static bool GlobalCreateLodGroup = EditorPrefs.GetBool( PrefGlobalCreateLodGroup, false );
		public static string GlobalAlbedo = EditorPrefs.GetString( PrefGlobalGBuffer0Name, DefaultAlbedoName );
		public static string GlobalNormals = EditorPrefs.GetString( PrefGlobalGBuffer1Name, DefaultSpecularName );
		public static string GlobalSpecular = EditorPrefs.GetString( PrefGlobalGBuffer2Name, DefaultNormalName );
		public static string GlobalOcclusion = EditorPrefs.GetString( PrefGlobalGBuffer3Name, DefaultOcclusionName );
		public static string GlobalEmission = EditorPrefs.GetString( PrefGlobalGBuffer4Name, DefaultEmissionName );
		public static string GlobalPosition = EditorPrefs.GetString( PrefGlobalGBuffer5Name, DefaultPositionName );
		public static bool GlobalBakingOptions = EditorPrefs.GetBool( PrefGlobalBakingOptions, true );
		public static ShowOption GlobalStartUp = ( ShowOption )EditorPrefs.GetInt( PrefGlobalStartUp, 0 );
		public static bool GlobalAutoSRP = EditorPrefs.GetBool( PrefGlobalAutoSRP, true );
		private static readonly GUIContent DefaultSuffixesLabel = new GUIContent( "Default Suffixes", "Default Suffixes for new Bake Presets" );

		private static bool PrefsLoaded = false;
		private static GUIContent PathButtonContent = new GUIContent();

		[SettingsProvider]
		public static SettingsProvider ImpostorsSettings()
		{
			var provider = new SettingsProvider( "Preferences/Amplify Impostors", SettingsScope.User )
			{
				guiHandler = ( string searchContext ) => {
					PreferencesGUI();
				}
			};
			return provider;
		}

		public static void PreferencesGUI()
		{
			if ( !PrefsLoaded )
			{
				LoadDefaults();
				PrefsLoaded = true;
			}

			PathButtonContent.text = string.IsNullOrEmpty( GlobalFolder ) ? "Click to select folder" : GlobalFolder;

			EditorGUIUtility.labelWidth = 250;

			GlobalStartUp = ( ShowOption )EditorGUILayout.EnumPopup( StartUp, GlobalStartUp );
			GlobalAutoSRP = EditorGUILayout.Toggle( AutoSRP, GlobalAutoSRP );

			GlobalDefaultMode = ( FolderMode )EditorGUILayout.EnumPopup( "New Impostor Default Path", GlobalDefaultMode ? FolderMode.Global : FolderMode.RelativeToPrefab ) == FolderMode.Global;
			EditorGUILayout.BeginHorizontal();
			if ( GlobalDefaultMode )
			{
				EditorGUI.BeginChangeCheck();
				GlobalFolder = EditorGUILayout.TextField( "Global Folder", GlobalFolder );
				if ( EditorGUI.EndChangeCheck() )
				{
					GlobalFolder = GlobalFolder.TrimStart( new char[] { '/', '*', '.', ' ' } );
					GlobalFolder = "/" + GlobalFolder;
					GlobalFolder = GlobalFolder.TrimEnd( new char[] { '/', '*', '.', ' ' } );
					EditorPrefs.SetString( PrefGlobalFolder, GlobalFolder );
				}
				if ( GUILayout.Button( "...", "minibutton", GUILayout.Width( 20 )/*GUILayout.MaxWidth( Screen.width * 0.5f )*/ ) )
				{
					string oneLevelUp = Application.dataPath + "/../";
					string directory = Path.GetFullPath( oneLevelUp ).Replace( "\\", "/" );
					string fullpath = directory + GlobalFolder;
					string folderpath = EditorUtility.SaveFolderPanel( "Save Impostor to folder", FileUtil.GetProjectRelativePath( fullpath ), null );

					folderpath = FileUtil.GetProjectRelativePath( folderpath );
					if ( !string.IsNullOrEmpty( folderpath ) )
					{
						GlobalFolder = folderpath;
						GlobalFolder = GlobalFolder.TrimStart( new char[] { '/', '*', '.', ' ' } );
						GlobalFolder = "/" + GlobalFolder;
						GlobalFolder = GlobalFolder.TrimEnd( new char[] { '/', '*', '.', ' ' } );
						EditorPrefs.SetString( PrefGlobalFolder, GlobalFolder );
					}
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				GlobalRelativeFolder = EditorGUILayout.TextField( "Relative to Prefab Folder", GlobalRelativeFolder );
				if ( EditorGUI.EndChangeCheck() )
				{
					GlobalRelativeFolder = GlobalRelativeFolder.TrimStart( new char[] { '/', '*', '.', ' ' } );
					GlobalRelativeFolder = "/" + GlobalRelativeFolder;
					GlobalRelativeFolder = GlobalRelativeFolder.TrimEnd( new char[] { '/', '*', '.', ' ' } );
					EditorPrefs.SetString( PrefGlobalRelativeFolder, GlobalRelativeFolder );
				}
				EditorGUI.BeginDisabledGroup( true );
				GUILayout.Button( "...", "minibutton", GUILayout.Width( 20 ) );
				EditorGUI.EndDisabledGroup();
			}

			EditorGUILayout.EndHorizontal();

			GlobalTexImport = EditorGUILayout.Popup( "Texture Importer Settings", GlobalTexImport, new string[] { "Ask if resolution is different", "Don't ask, always change", "Don't ask, never change" } );
			GlobalCreateLodGroup = EditorGUILayout.Toggle( "Create LODGroup if not present", GlobalCreateLodGroup );
			GUILayout.Space( 5 );
			GUILayout.Label( DefaultSuffixesLabel, "boldlabel" );
			GlobalAlbedo = EditorGUILayout.TextField( "Albedo (RGB) Alpha (A)", GlobalAlbedo );
			GlobalNormals = EditorGUILayout.TextField( "Normal (RGB) Depth (A)", GlobalNormals );
			GlobalSpecular = EditorGUILayout.TextField( "Specular (RGB) Smoothness (A)", GlobalSpecular );
			GlobalOcclusion = EditorGUILayout.TextField( "Occlusion (RGB)", GlobalOcclusion );
			GlobalEmission = EditorGUILayout.TextField( "Emission (RGB)", GlobalEmission );
			GlobalPosition = EditorGUILayout.TextField( "Position (RGB)", GlobalPosition );
			if ( GUI.changed )
			{
				EditorPrefs.SetInt( PrefGlobalStartUp, ( int )GlobalStartUp );
				EditorPrefs.SetBool( PrefGlobalAutoSRP, GlobalAutoSRP );

				EditorPrefs.SetBool( PrefGlobalDefault, GlobalDefaultMode );
				EditorPrefs.SetInt( PrefGlobalTexImport, GlobalTexImport );
				EditorPrefs.SetBool( PrefGlobalCreateLodGroup, GlobalCreateLodGroup );

				EditorPrefs.SetString( PrefGlobalGBuffer0Name, GlobalAlbedo );
				EditorPrefs.SetString( PrefGlobalGBuffer1Name, GlobalSpecular );
				EditorPrefs.SetString( PrefGlobalGBuffer2Name, GlobalNormals );
				EditorPrefs.SetString( PrefGlobalGBuffer3Name, GlobalEmission );
				EditorPrefs.SetString( PrefGlobalGBuffer4Name, GlobalOcclusion );
				EditorPrefs.SetString( PrefGlobalGBuffer5Name, GlobalPosition );
			}
		}

		public static void LoadDefaults()
		{
			GlobalStartUp = ( ShowOption )EditorPrefs.GetInt( PrefGlobalStartUp, 0 );
			GlobalAutoSRP =  EditorPrefs.GetBool( PrefGlobalAutoSRP, true );

			GlobalFolder = EditorPrefs.GetString( PrefGlobalFolder, "" );
			GlobalRelativeFolder = EditorPrefs.GetString( PrefGlobalRelativeFolder, "" );
			GlobalDefaultMode = EditorPrefs.GetBool( PrefGlobalDefault, false );
			GlobalTexImport = EditorPrefs.GetInt( PrefGlobalTexImport, 0 );
			GlobalCreateLodGroup = EditorPrefs.GetBool( PrefGlobalCreateLodGroup, false );
			GlobalBakingOptions = EditorPrefs.GetBool( PrefGlobalBakingOptions, true );

			GlobalAlbedo = EditorPrefs.GetString( PrefGlobalGBuffer0Name, DefaultAlbedoName );
			GlobalSpecular = EditorPrefs.GetString( PrefGlobalGBuffer1Name, DefaultSpecularName );
			GlobalNormals = EditorPrefs.GetString( PrefGlobalGBuffer2Name, DefaultNormalName );
			GlobalEmission = EditorPrefs.GetString( PrefGlobalGBuffer3Name, DefaultEmissionName );
			GlobalOcclusion = EditorPrefs.GetString( PrefGlobalGBuffer4Name, DefaultOcclusionName );
			GlobalPosition = EditorPrefs.GetString( PrefGlobalGBuffer5Name, DefaultPositionName );
		}
	#endif
	}
}
