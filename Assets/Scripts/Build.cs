#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using Nordeus.Build.Reporters;
using UnityEditor;
using UnityEngine;
using System.Collections;

//-batchMode -quit -nographics -output F:\build\build.exe -executeMethod JAW.BuildPipeline.Build -target StandaloneWindows  -logfile f:\out.txt

namespace JAW
{
	public static class BuildPipeline
    {
        static string OutputPath = ".\\build\\build.exe";
        static BuildTarget BuildTarget = BuildTarget.NoTarget;
        static bool DevelopmentPlayer = false;
        static bool ScriptsOnly = false;
        static bool AllowDebugging = false;
        static int BuildNumber = 0;
        static int BuildVersion = 0;

        private static void BuildPlayer(out UnityEditor.Build.Reporting.BuildReport report)
        {
            int options = (DevelopmentPlayer ? (int)BuildOptions.Development : 0) |
                          (ScriptsOnly ? (int)BuildOptions.BuildScriptsOnly : 0) |
                          (AllowDebugging ? (int)BuildOptions.AllowDebugging : 0);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetEnabledScenePaths().ToArray();
            buildPlayerOptions.locationPathName = OutputPath;
            buildPlayerOptions.target = BuildTarget;
            buildPlayerOptions.options = (BuildOptions)options;
            buildPlayerOptions.targetGroup = GetBuildTargetGroup( BuildTarget );
            report = UnityEditor.BuildPipeline.BuildPlayer( buildPlayerOptions );
        }

        private static List<string> GetEnabledScenePaths()
		{
			List<string> scenePaths = new List<string>();
			foreach (var scene in EditorBuildSettings.scenes)
				scenePaths.Add(scene.path);
			return scenePaths;
		}

        private static BuildTargetGroup GetBuildTargetGroup( BuildTarget target )
        {
            if (target == BuildTarget.PS4)
                return BuildTargetGroup.PS4;
            if (target == BuildTarget.XboxOne)
                return BuildTargetGroup.XboxOne;
            if (target == BuildTarget.StandaloneWindows)
                return BuildTargetGroup.Standalone;
            return BuildTargetGroup.Unknown;
        }

        private static void ProcessCommandLineArguments()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                string cmd = args[i].ToLower();

                if (cmd == "-target")
                {
                    i++;
                    BuildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), args[i]);
                }
                else if (cmd == "-output")
                {
                    i++;
                    OutputPath = args[i];
                }
                else if (cmd == "-devplayer")
                {
                    DevelopmentPlayer = true;
                }
                else if (cmd == "-scriptonly")
                {
                    ScriptsOnly = true;
                }
                else if (cmd == "-debugging")
                {
                    AllowDebugging = true;
                }
            }
        }

        [MenuItem("JAW/BuildPipeline/Build")]
        public static void Build()
        {
            if (BuildTarget == BuildTarget.NoTarget)
                BuildTarget = EditorUserBuildSettings.activeBuildTarget;

            UnityEditor.Build.Reporting.BuildReport report;

            ProcessCommandLineArguments();

            BuildPlayer( out report );
        }

        [MenuItem("JAW/BuildPipeline/Light Mapper")]
        public static void LightMapper()
        {

        }
    }
}
#endif