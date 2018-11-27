using System;
using System.IO;
using System.Collections.Generic;

namespace CMakeConfigure
{
	public static class PathsHelp
	{
		public static readonly char PATH_SEPARATOR = '/';

		public static string Combine(params string[] folders)
		{
			string combined = string.Empty;
			foreach (var folder in folders)
			{
				combined += PATH_SEPARATOR + folder;
			}
			return combined;
		}

		public static bool HasExtension(string[] fileExtensionArray, string fileName)
		{
			foreach (var extension in fileExtensionArray)
			{
				if (fileName.EndsWith(extension))
					return true;
			}
			return false;
		}

		public static string MakeRelative(string referencePath, string filePath)
		{
			var fileUri = new Uri(filePath);
			var referenceUri = new Uri(referencePath);
			return referenceUri.MakeRelativeUri(fileUri).ToString();
		}

		public static void GetAllFilePaths(string rootPath, string searchPath, string[] fileExtensionArray, List<string> outPathList)
		{
			if (!Directory.Exists(searchPath))
			{
				return;
			}

			var dirInfo = new DirectoryInfo(searchPath);
			foreach (var fileInfo in dirInfo.EnumerateFiles())
			{
				if (HasExtension(fileExtensionArray, fileInfo.Name))
				{
					var relativePath = MakeRelative(rootPath, fileInfo.FullName);
					outPathList.Add(relativePath);
				}
			}

			foreach (var directoryInfo in dirInfo.EnumerateDirectories())
			{
				GetAllFilePaths(rootPath, directoryInfo.FullName, fileExtensionArray, outPathList);
			}
		}

		public static void GetAllDirectoryPaths(string rootPath, string searchPath, List<string> outPathList)
		{
			var rootDirInfo = new DirectoryInfo(rootPath);
			var searchDirInfo = new DirectoryInfo(searchPath);

			foreach (var directoryInfo in searchDirInfo.EnumerateDirectories())
			{
				var relativePath = MakeRelative(rootDirInfo.FullName, directoryInfo.FullName);
				outPathList.Add(relativePath);
			}

			foreach (var directoryInfo in searchDirInfo.EnumerateDirectories())
			{
				GetAllDirectoryPaths(rootPath, directoryInfo.FullName, outPathList);
			}
		}

		public static void GetAllExternalProjects(string searchPath, ref List<List<string>> outProjectList)
		{
			if (!Directory.Exists(searchPath))
			{
				Console.WriteLine("ERROR the path " + searchPath + " doesn't exist!");
				return;
			}

			// The externals folder is at the same level as the project one
			var searchDirInfo = new DirectoryInfo(searchPath);
			var parentSearchDirInfo = Directory.GetParent(searchDirInfo.FullName);
			var searchExternalsDir = Path.Combine(parentSearchDirInfo.FullName, "externals");

			if (!Directory.Exists(searchExternalsDir))
			{
				return;
			}

			var externalsDirInfo = new DirectoryInfo(searchExternalsDir);
			foreach (var directoryInfo in externalsDirInfo.EnumerateDirectories())
			{
				var projectPath = Path.Combine(directoryInfo.FullName, "project");
				if (Directory.Exists(projectPath))
				{
					var projectList = new List<string>();
					projectList.Add(directoryInfo.Name);
					Console.WriteLine("Folder: " + directoryInfo.Name);
					GetExternalProjects(projectPath, ref projectList);
					outProjectList.Add(projectList);
				}
			}
		}

		public static void GetExternalProjects(string searchPath, ref List<string> outProjectList)
		{
			if (!Directory.Exists(searchPath))
			{
				Console.WriteLine("[ERROR] the path " + searchPath + " doesn't exist!");
				return;
			}

			var searchDirInfo = new DirectoryInfo(searchPath);
			var parentSearchDirInfo = Directory.GetParent(searchDirInfo.FullName);
			var searchExternalsDir = Path.Combine(parentSearchDirInfo.FullName, "externals");

			// Exit condition, no externals on this project
			if (!Directory.Exists(searchExternalsDir))
			{
				return;
			}

			var externalsDirInfo = new DirectoryInfo(searchExternalsDir);
			foreach (var directoryInfo in externalsDirInfo.EnumerateDirectories())
			{
				var projectPath = Path.Combine(directoryInfo.FullName, "project");
				if (Directory.Exists(projectPath))
				{
					outProjectList.Add(directoryInfo.Name);
					Console.WriteLine("External: " + directoryInfo.Name);
					GetExternalProjects(projectPath, ref outProjectList);
				}
			}
		}
	}
}

