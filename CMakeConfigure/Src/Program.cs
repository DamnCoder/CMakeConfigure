using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CMakeConfigure
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string[] finalArgs = new string[(int)EArgType.MAX_ARGS];
			if(!ProjectConfig.FillArgs(args, ref finalArgs))
			{
				Console.WriteLine("CORRECT USE: mono CMakeConfigure.exe " +
					"<templates_path> <output_path> " +
					"<template_type> <build_type>");
				return;
			}
			
			string templatesPath = finalArgs[(int)EArgType.TEMPLATES_PATH];
			string outputPath = finalArgs[(int)EArgType.OUTPUT_PATH];
			string projectType = finalArgs[(int)EArgType.PROJECT_TYPE];
			string buildType = finalArgs[(int)EArgType.PROJECT_ENVIRONMENT];

			// 
			// Retrieve the template file lines and copy them on this list that is going to be the final CMakeLists.txt lines
			List<string> cmakeLineList = new List<string>();
			if (!CMakeHelper.FillTemplateList(projectType, templatesPath, ref cmakeLineList))
			{
				return;
			}

			// Paths
			string projectFolderPath = Path.GetFullPath(outputPath);

			// Table with all the lines to add for each key in the template
			Dictionary<string, List<string>> newLinesTable = new Dictionary<string, List<string>> ();

			newLinesTable.Add(ProjectConfig.PRJ_NAME_KEY, new List<string>() { CMakeHelper.CreateProjectFromProjPath(projectFolderPath) });
			newLinesTable.Add(ProjectConfig.BUILD_TYPE_KEY, new List<string>() { CMakeHelper.CreateSet("CMAKE_BUILD_TYPE", buildType) });
         
			// This list will contain the paths to sources and includes, it will be reused for both
			List<string> pathList = new List<string> ();

			// Project includes
			string completeIncludePath = Path.Combine(projectFolderPath, ProjectConfig.INCLUDE_FOLDER);
			pathList.Add(ProjectConfig.INCLUDE_FOLDER);
			PathsHelp.GetAllDirectoryPaths(completeIncludePath, completeIncludePath, pathList);
			newLinesTable.Add(ProjectConfig.PRJ_INCLUDE_KEY, CMakeHelper.CreateMultipleIncludeDirectories(pathList));

			pathList.Clear();

			// Header files
			PathsHelp.GetAllFilePaths(completeIncludePath, completeIncludePath, ProjectConfig.INCLUDE_FILE_EXT, pathList);
			newLinesTable.Add(ProjectConfig.PRJ_HEADERS_KEY, CMakeHelper.CreateSetMultiline("HEADERS", pathList));

			pathList.Clear();

			// Source files
			string completSrcPath = Path.Combine(projectFolderPath, ProjectConfig.SRC_FOLDER);
			PathsHelp.GetAllFilePaths(completSrcPath, completSrcPath, ProjectConfig.SRC_FILE_EXT, pathList);
			newLinesTable.Add(ProjectConfig.PRJ_SOURCES_KEY, CMakeHelper.CreateSetMultiline("SOURCES", pathList));

			pathList.Clear();

			// Subdirectories
			var projectList = new List<List<string>>();
			PathsHelp.GetAllExternalProjects(projectFolderPath, ref projectList);

			var externalLibs = new List<string>();
			CMakeHelper.AddExternalProjects(projectList, ref externalLibs);
			newLinesTable.Add(ProjectConfig.EXTERNAL_KEY, externalLibs);

			var externalTargets = new List<string>();
			CMakeHelper.AddExternalTargets(projectList, ref externalTargets);
			newLinesTable.Add(ProjectConfig.EXTERNAL_TARGET, externalTargets);

			// Add the new lines where the keys are in the template
			CMakeHelper.AddWhereKeys(cmakeLineList, newLinesTable);

			string outputFilePath = Path.Combine(outputPath, ProjectConfig.CMAKELIST_OUTPUT_FILENAME);
			File.WriteAllLines(outputFilePath, cmakeLineList.ToArray());

			string fullOutputPath = Path.GetFullPath(outputPath);
			Console.WriteLine("CMakeLists.txt generated succesfully "+fullOutputPath);
			
		}
	}
}