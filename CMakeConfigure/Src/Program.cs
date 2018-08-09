using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CMakeConfigure
{
	class MainClass
	{
		static string[] srcfileExtensions = 
		{
			".cpp",
		};

		static string[] includeFileExtensions = 
		{
			".h",
			".inl",
		};

		//static readonly string utilitiesFolder = "../";
		//static readonly string projectFolder = "../../project";

		//static readonly string codeFolder = "project";
		static readonly string srcFolder = "src";
		static readonly string includeFolder = "include";

		static readonly string SHARED_LIB = "SHARED";
		static readonly string STATIC_LIB = "STATIC";
		static readonly string HEADER_LIB = "HEADER";

		static readonly string STATICLIB_TEMPLATE_FILENAME = "CMakeLists_StaticLib_Template.txt";
		static readonly string SHAREDLIB_TEMPLATE_FILENAME = "CMakeLists_SharedLib_Template.txt";
		static readonly string EXECUTABLE_TEMPLATE_FILENAME = "CMakeLists_Executable_Template.txt";
		static readonly string HEADER_TEMPLATE_FILENAME = "CMakeLists_HeaderLib_Template.txt";

		static readonly string CMAKELIST_OUTPUT_FILENAME = "CMakeLists.txt";

		static readonly string PRJ_NAME_KEY = "#[PROJECT_NAME]";
		static readonly string BUILD_TYPE_KEY = "#[BUILD_TYPE]";

		static readonly string PRJ_INCLUDE_KEY = "#[PRJ_INCLUDE]";
		static readonly string PRJ_SOURCES_KEY = "#[PRJ_SOURCE_FILES]";
		static readonly string PRJ_HEADERS_KEY = "#[PRJ_HEADER_FILES]";

		static readonly string EXTERNAL_KEY = "#[EXTERNAL_PROJECTS]";

		public static void Main (string[] args)
		{
			if(args.Length < 5)
			{
				Console.WriteLine("CORRECT USE: mono CMakeConfigure.exe " +
					"<templates_path> <output_path> " +
					"<template_type> <project_name> <build_type> <subdirectory1> <subdirectory2>...");
				return;
			}

			string templatesPath = args[0];
			string outputPath = args[1];
			string templateType = args[2];
			string projectName = args[3];
			string buildType = args[4];

			string CMAKE_TEMPLATE_FILENAME = EXECUTABLE_TEMPLATE_FILENAME;

			if (templateType == SHARED_LIB)
			{
				CMAKE_TEMPLATE_FILENAME = SHAREDLIB_TEMPLATE_FILENAME;
			}
			else if (templateType == STATIC_LIB)
			{
				CMAKE_TEMPLATE_FILENAME = STATICLIB_TEMPLATE_FILENAME;
			}
			else if (templateType == HEADER_LIB)
			{
				CMAKE_TEMPLATE_FILENAME = HEADER_TEMPLATE_FILENAME;
			}

			string[] cmakeTemplateLines = File.ReadAllLines(Path.Combine(templatesPath, CMAKE_TEMPLATE_FILENAME));

			// This is going to be the final CMakeLists.txt lines
			List<string> cmakeLineList = new List<string>(cmakeTemplateLines);

			// Table with all the lines to add for each key in the template
			Dictionary<string, List<string>> newLinesTable = new Dictionary<string, List<string>> ();

			newLinesTable.Add(PRJ_NAME_KEY, new List<string>() { CMakeHelper.CreateProject(projectName) });
			newLinesTable.Add(BUILD_TYPE_KEY, new List<string>() { CMakeHelper.CreateSet("CMAKE_BUILD_TYPE", buildType) });


			// This list will contain the paths to sources and includes, it will be reused for both
			List<string> pathList = new List<string> ();

			// Paths
			string rootIncludePath = Path.GetFullPath(outputPath);

			// Project includes
			string completeIncludePath = Path.Combine(rootIncludePath, includeFolder);
			pathList.Add(includeFolder);
			PathsHelp.GetAllDirectoryPaths(completeIncludePath, completeIncludePath, pathList);
			newLinesTable.Add(PRJ_INCLUDE_KEY, CMakeHelper.CreateMultipleIncludeDirectories(pathList));

			pathList.Clear();

			// Header files
			PathsHelp.GetAllFilesPaths(completeIncludePath, completeIncludePath, includeFileExtensions, pathList);
			newLinesTable.Add(PRJ_HEADERS_KEY, CMakeHelper.CreateSetMultiline("HEADERS", pathList));

			pathList.Clear();

			// Source files
			string completSrcPath = Path.Combine(rootIncludePath, srcFolder);
			PathsHelp.GetAllFilesPaths(completSrcPath, completSrcPath, srcfileExtensions, pathList);
			newLinesTable.Add(PRJ_SOURCES_KEY, CMakeHelper.CreateSetMultiline("SOURCES", pathList));

			pathList.Clear();

			// Subdirectories
			List<string> nameList = new List<string>();
			int subdirectoriesNum = args.Length - 5;
			for(int i=5; i<args.Length; ++i)
			{
				nameList.Add(args[i]);
			}
			newLinesTable.Add (EXTERNAL_KEY, CMakeHelper.AddMultipleSubdirectory(nameList));

			// Add the new lines where the keys are in the template
			CMakeHelper.AddWhereKeys(cmakeLineList, newLinesTable);

			string outputFilePath = Path.Combine(outputPath, CMAKELIST_OUTPUT_FILENAME);
			File.WriteAllLines(outputFilePath, cmakeLineList.ToArray());

			string fullOutputPath = Path.GetFullPath(outputPath);
			Console.WriteLine("CMakeLists.txt generated succesfully "+fullOutputPath);
			
		}
	}
}