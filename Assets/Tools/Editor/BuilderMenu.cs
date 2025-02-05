using UnityEditor;
using Builder.Tools;
using System.Diagnostics;
using System.IO;

public class BuilderMenu : EditorWindow
{
    static string BUILDS_FOLDER_PATH = Path.GetFullPath("./Builds/");

    [MenuItem("Builder/Default")]
    private static void ShowCommonBuildConfirmation()
    {
        if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to generate a build?", "Yes", "Cancel"))
        {
            string buildPath;
            if (Builder.Tools.Builder.ExecuteDefaultBuild(out buildPath))
            {
                if (EditorUtility.DisplayDialog("Success", $"The build was successfully created at {buildPath}!", "OK"))
                    Process.Start(System.IO.Path.GetDirectoryName(BUILDS_FOLDER_PATH));
            }
            else
            {
                EditorUtility.DisplayDialog("Failure", "The build failed due to an error! Check the console for more details.", "OK");
            }
        }
    }

    [MenuItem("Builder/Single.exe (SFX)")]
    private static void ShowSingleBuildConfirmation()
    {
        if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to generate a SFX build?", "Yes", "Cancel"))
        {
            string buildPath;
            if (Builder.Tools.Builder.ExecuteSingleExecutableBuild(out buildPath))
            {
                if (EditorUtility.DisplayDialog("Success", $"The SFX build was successfully created!", "OK"))
                    Process.Start(Path.GetDirectoryName(BUILDS_FOLDER_PATH));
            }
            else
            {
                EditorUtility.DisplayDialog("Failure", "The build failed due to an error! Check the console for more details.", "OK");
            }
        }
    }
    
    [MenuItem("Builder/Installer (InnoSetup)")]
    private static void ShowInstallerConfirmation()
    {
        if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to generate a Installer?", "Yes", "Cancel"))
        {
            string buildPath;
            if (Builder.Tools.Builder.ExecuteInstallerBuild(out buildPath))
            {
                if (EditorUtility.DisplayDialog("Success", $"The Installer was successfully created!", "OK"))
                    Process.Start(Path.GetDirectoryName(BUILDS_FOLDER_PATH));
            }
            else
            {
                EditorUtility.DisplayDialog("Failure", "The build failed due to an error! Check the console for more details.", "OK");
            }
        }
    }
}