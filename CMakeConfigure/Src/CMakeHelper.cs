using System;
using System.Collections.Generic;
using System.IO;

namespace CMakeConfigure
{
	public static class CMakeHelper
	{
		public static bool FillTemplateList(string projectType, string templatesPath, ref List<string> outCMakeTemplateLinesList)
		{
			string CMAKE_TEMPLATE_FILENAME = ProjectConfig.GetTemplateFilename(projectType);
			var templateFilePath = Path.Combine(templatesPath, CMAKE_TEMPLATE_FILENAME);
			if (!File.Exists(templateFilePath))
			{
				Console.WriteLine("[ERROR] No templates on path: " + templateFilePath);
				return false;
			}

			string[] cmakeTemplateLinesArray = File.ReadAllLines(templateFilePath);
			if (cmakeTemplateLinesArray.Length == 0)
			{
				Console.WriteLine("[ERROR] Template file is empty: " + templateFilePath);
				return false;
			}

			outCMakeTemplateLinesList.Clear();
			outCMakeTemplateLinesList.AddRange(cmakeTemplateLinesArray);
			return true;
		}

		public static void AddWhereKeys(List<string> cmakeLineList, Dictionary<string, List<string>> newLinesTable)
		{
			for (int i = 0; i < cmakeLineList.Count; ++i) 
			{
				var line = cmakeLineList [i];

				if(newLinesTable.ContainsKey(line))
				{
					List<string> newLineList = newLinesTable[line];
					int addedPathIndex = i;
					foreach (var newLine in newLineList)
					{
						cmakeLineList.Insert(++addedPathIndex, newLine);
					}
				}
			}
		}

		public static List<string> CreateSetMultiline(string keyName, List<string> paths)
		{
			List<string> setLines = new List<string>();

			setLines.Add("SET("+keyName);
			foreach(string path in paths)
			{
				setLines.Add("\t"+path);
			}
			setLines.Add(")");

			return setLines;
		}

		public static List<string> CreateMultipleIncludeDirectories(List<string> paths)
		{
			List<string> includeLines = new List<string>();

			foreach(string path in paths)
			{
				includeLines.Add(CreateInclude(path));
			}
			return includeLines;
		}

		public static string CreateProjectFromProjPath(string projectPath)
		{
			var projectDirInfo = new DirectoryInfo(projectPath);
			var parentSearchDirInfo = Directory.GetParent(projectDirInfo.FullName);
			return CreateProject(parentSearchDirInfo.Name);
		}

		public static string CreateProject(string projectName)
		{
			return "PROJECT(" + projectName + ")";
		}

		public static string CreateSet(string keyName, string value)
		{
			return "SET("+keyName+" "+value+")";
		}

		public static string CreateInclude(string path)
		{
			return "INCLUDE_DIRECTORIES("+path+")";
		}

		public static string CreateIncludeExternal(string projectName)
		{
			string externalsProjectPath = Path.Combine("${EXTERNALS_PATH}", projectName, "project", "include").Replace("\\", "/");
			return "INCLUDE_DIRECTORIES("+ externalsProjectPath + ")";
		}

		public static List<string> AddMultipleSubdirectory(List<string> nameList)
		{
			List<string> subdirectoryLines = new List<string>();

			foreach(string name in nameList)
			{
				subdirectoryLines.Add(AddSubdirectory(name));
			}
			return subdirectoryLines;
		}

		public static string AddSubdirectory(string projectName)
		{
			string externalsProjectPath = Path.Combine("${EXTERNALS_PATH}", projectName, "project").Replace("\\", "/");
			string binaryProjectPath = Path.Combine("${PROJECT_BINARY_DIR}", projectName, "project").Replace("\\", "/");
			return "ADD_SUBDIRECTORY("+ externalsProjectPath + " " + binaryProjectPath + ")\n" +
				CreateIncludeExternal(projectName) + "\n";
		}

		public static void AddExternalProjects(List<List<string>> projectList, ref List<string> outExternalProjects)
		{
			// External projects
			foreach (var project in projectList)
			{
				if (0 < project.Count)
				{
					Console.WriteLine("External Project: " + AddSubdirectory(project[0]));
					outExternalProjects.Add(AddSubdirectory(project[0]));
				}
			}

			// Externals of external
			foreach (var project in projectList)
			{
				if (0 < project.Count)
				{
					string externalRootPath = Path.Combine(project[0], "externals");
					for (int i=1; i < project.Count; ++i)
					{
						externalRootPath = Path.Combine(externalRootPath, project[i]);
						Console.WriteLine("Include External of External Project: " + CreateIncludeExternal(externalRootPath));
						outExternalProjects.Add(CreateIncludeExternal(externalRootPath));
						externalRootPath = Path.Combine(externalRootPath, "externals");
					}
				}
			}
		}

		public static void AddExternalTargets(List<List<string>> externalList, ref List<string> outExternalProjects)
		{
			outExternalProjects.Add("TARGET_LINK_LIBRARIES(${PROJECT_NAME}");
			foreach (var projectList in externalList)
			{
				foreach (var project in projectList)
				{
					outExternalProjects.Add("\t" + project);
				}
			}
			outExternalProjects.Add(")");
		}
	}
}

