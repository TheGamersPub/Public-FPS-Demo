using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace Builder.Tools
{
    public class Builder
    {
        public static bool ExecuteDefaultBuild(out string buildPath)
        {
            string projectName = Application.productName;
            string projectVersion = PlayerSettings.bundleVersion;

            string buildDir = $"Builds/{projectName}_v{projectVersion}";
            string exeName = $"{projectName}_v{projectVersion}.exe";

            if (Directory.Exists(buildDir))
            {
                    string originalBuildDir = buildDir;
                    int i = 0;

                    while (Directory.Exists(buildDir))
                    {
                        i++;
                        buildDir = $"Builds\\{projectName}_v{projectVersion}({i})";
                    }
            }

            buildPath = Path.Combine(buildDir, exeName);

            BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
            BuildOptions buildOptions = BuildOptions.None;

            // Try to build the project and return whether it succeeded or not.
            var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, buildTarget, buildOptions);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                UnityEngine.Debug.Log("<color=green><b>Build succeeded! Everything went smoothly.</b></color>");
                return true;
            }
            else
            {
                UnityEngine.Debug.LogError("Build failed! Check the console for more information.");
                return false;
            }
        }

        public static bool ExecuteSingleExecutableBuild(out string buildDir)
        {
            //Verify if WinRAR Evironment Variable is correctly configured
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "winrar -version")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(startInfo);
            if (process == null)
            {
                UnityEngine.Debug.LogError("Failed to start process.");
                buildDir = null;
                return false;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            UnityEngine.Debug.Log(output);
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError(error);
            }

            //Select Build Icon
            string iconPath = EditorUtility.OpenFilePanel("Select Icon", "", "ico");
            if (string.IsNullOrEmpty(iconPath))
            {
                UnityEngine.Debug.LogError("No icon selected! SFX creation failed.");
                buildDir = null;
                return false;
            }

            //Build Project Normally

            string projectName = Application.productName;
            string projectVersion = PlayerSettings.bundleVersion;

            buildDir = $"Builds\\temp";

            if (Directory.Exists(buildDir))
            {
                UnityEngine.Debug.Log("Deleting previous build to avoid conflicts.");
                Directory.Delete(buildDir, true);
            }

            string buildPath = Path.Combine(buildDir, "game.exe");

            BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
            BuildOptions buildOptions = BuildOptions.None;

            var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, buildTarget, buildOptions);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                UnityEngine.Debug.LogError("Build failed! Check the console for more information.");
                return false;
            }

            if (!File.Exists(buildPath))
            {
                UnityEngine.Debug.LogError("Executable not found! SFX creation failed.");
                return false;
            }

            //Create SFX File and Delete Temp Folder

            string[] sfxContent = new string[]
            {
            "Setup=game.exe",
            "Silent=1",
            "Overwrite=1",
            "TempMode"
            };

            string configSFXPath = Path.GetFullPath(Path.Combine(buildDir, "config.sfx"));

            File.WriteAllLines(configSFXPath, sfxContent);
            UnityEngine.Debug.Log($"<color=green><b>SFX config file created at: {configSFXPath}</b></color>");

            string executableOutput = $"Builds/{projectName}_v{projectVersion}.exe";

            if (File.Exists(executableOutput))
            {
                string originalOutputName = executableOutput;
                int i = 0;

                while (File.Exists(executableOutput))
                {
                    i++;
                    executableOutput = $"Builds\\{projectName}_v{projectVersion}({i}).exe";
                }
            }

            string winrarSFXCommand = $"winrar a -sfx -z\"{configSFXPath}\" -iicon\"{iconPath}\" \"../../{executableOutput}\" * -r";

            startInfo = new ProcessStartInfo("cmd.exe", $"/c cd /d \"{buildDir}\" && {winrarSFXCommand}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process = Process.Start(startInfo);
            if (process == null)
            {
                UnityEngine.Debug.LogError("Failed to start process.");
                return false;
            }

            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrEmpty(output))
                UnityEngine.Debug.Log(output);

            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError(error);

            if (Directory.Exists(buildDir))
            {
                Directory.Delete(buildDir, true);
            }

            if (File.Exists(executableOutput))
            {
                UnityEngine.Debug.Log($"<color=green><b>SFX build successfully created at {executableOutput}</b></color>");
                return true;
            }
            else
            {
                UnityEngine.Debug.LogError("SFX creation failed.");
                return false;
            }
        }

        public static bool ExecuteInstallerBuild(out string buildDir)
        {
            //Verify if ISCC Evironment Variable is correctly configured
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "ISCC")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(startInfo);
            if (process == null)
            {
                UnityEngine.Debug.LogError("Failed to start process.");
                buildDir = null;
                return false;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            UnityEngine.Debug.Log(output);
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError(error);
            }

            //Select Build Icon
            string iconPath = EditorUtility.OpenFilePanel("Select Icon", "", "ico");
            if (string.IsNullOrEmpty(iconPath))
            {
                UnityEngine.Debug.LogError("No icon selected! SFX creation failed.");
                buildDir = null;
                return false;
            }

            //Build Project Normally

            string projectName = Application.productName;
            string projectVersion = PlayerSettings.bundleVersion;
            string exeName = $"{projectName}_v{projectVersion}.exe";

            string tempDir = Path.GetFullPath($"Builds\\temp");
            buildDir = Path.Combine(tempDir, $"{projectName}_v{projectVersion}");

            if (Directory.Exists(tempDir))
            {
                UnityEngine.Debug.Log("Deleting previous build to avoid conflicts.");
                Directory.Delete(tempDir, true);
            }

            string buildPath = Path.Combine(buildDir, exeName);

            BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
            BuildOptions buildOptions = BuildOptions.None;

            var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildPath, buildTarget, buildOptions);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                UnityEngine.Debug.LogError("Build failed! Check the console for more information.");
                return false;
            }

            if (!File.Exists(buildPath))
            {
                UnityEngine.Debug.LogError("Executable not found! Installer creation failed.");
                return false;
            }

            // Create Inno Setup Script File

            string myAppId = Guid.NewGuid().ToString();
            string myAppPublisher = "The Gamer's Pub";
            string myAppURL = "";
            
            string issContent = $@"            
            [Setup]
            AppId={myAppId}
            AppName={projectName}
            AppVersion={projectVersion}
            AppPublisher={myAppPublisher}
            AppPublisherURL={myAppURL}
            AppSupportURL={myAppURL}
            AppUpdatesURL={myAppURL}
            DefaultDirName={buildDir}\{projectName}
            ArchitecturesAllowed=x64compatible
            ArchitecturesInstallIn64BitMode=x64compatible
            DisableProgramGroupPage=yes
            OutputBaseFilename=Instalador {projectName}
            SetupIconFile={iconPath}
            Compression=lzma
            SolidCompression=yes
            WizardStyle=modern
            
            [Languages]
            Name: ""english""; MessagesFile: ""compiler:Default.isl""
            Name: ""brazilianportuguese""; MessagesFile: ""compiler:Languages\BrazilianPortuguese.isl""
            Name: ""spanish""; MessagesFile: ""compiler:Languages\Spanish.isl""
            
            [Tasks]
            Name: ""desktopicon""; Description: ""{{cm:CreateDesktopIcon}}""; GroupDescription: ""{{cm:AdditionalIcons}}""; Flags: unchecked
            
            [Files]
            Source: ""{buildDir}\{exeName}""; DestDir: ""{{app}}""; Flags: ignoreversion
            Source: ""{buildDir}\*""; DestDir: ""{{app}}""; Flags: ignoreversion recursesubdirs createallsubdirs
            
            [Icons]
            Name: ""{{autoprograms}}\{projectName}""; Filename: ""{{app}}\{exeName}""
            Name: ""{{autodesktop}}\{projectName}""; Filename: ""{{app}}\{exeName}""; Tasks: desktopicon
            
            [Run]
            Filename: ""{{app}}\{exeName}""; Description: ""{{cm:LaunchProgram,{projectName}}}""; Flags: nowait postinstall skipifsilent
            ";

            string issPath = Path.Combine(tempDir, "setup_script.iss");

            File.WriteAllText(issPath, issContent);

            string executableOutput = $"Builds\\Instalador {projectName}.exe";
            string InnoCommand = $"ISCC \"{issPath}\"";

            startInfo = new ProcessStartInfo("cmd.exe", $"/c cd /d \"{tempDir}\" && {InnoCommand}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process = Process.Start(startInfo);
            if (process == null)
            {
                UnityEngine.Debug.LogError("Failed to start process.");
                return false;
            }

            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrEmpty(output))
                UnityEngine.Debug.Log(output);

            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError(error);

            if (File.Exists(executableOutput))
            {
                string originalOutputName = executableOutput;
                int i = 0;
                
                while (File.Exists(executableOutput)) 
                {
                    i++;
                    executableOutput = $"Builds\\Instalador {projectName}({i}).exe";
                }
            }

            if (File.Exists($"{tempDir}\\Output\\Instalador {projectName}.exe"))
            {
                File.Move($"{tempDir}\\Output\\Instalador {projectName}.exe", executableOutput);
            }

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            if (File.Exists(executableOutput))
            {
                UnityEngine.Debug.Log($"<color=green><b>Installer successfully created at {executableOutput} </b></color>");
                return true;
            }
            else
            {
                UnityEngine.Debug.LogError("Installer creation failed.");
                return false;
            }
        }
    }
}