using System;
using System.IO;
using System.Collections.Generic;

namespace CMakeConfigure
{
	public static class PathsHelp
	{
		public static bool HasExtension(string [] fileExtensionArray, string fileName)
		{
			foreach(var extension in fileExtensionArray)
			{
				if(fileName.EndsWith(extension))
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

		public static void GetAllFilesPaths(string rootPath, string searchPath, string [] fileExtensionArray, List<string> outPathList)
		{
         if(!Directory.Exists(searchPath))
         {
            return;
         }

			var dirInfo = new DirectoryInfo(searchPath);
			foreach(var fileInfo in dirInfo.EnumerateFiles())
			{
				if (HasExtension(fileExtensionArray, fileInfo.Name))
				{
					var relativePath = MakeRelative(rootPath, fileInfo.FullName);
					outPathList.Add(relativePath);
				}
			}

			foreach(var directoryInfo in dirInfo.EnumerateDirectories())
			{
				GetAllFilesPaths(rootPath, directoryInfo.FullName, fileExtensionArray, outPathList);
			}
		}

		public static void GetAllDirectoryPaths(string rootPath, string searchPath, List<string> outPathList)
		{
			var rootDirInfo = new DirectoryInfo(rootPath);
			var searchDirInfo = new DirectoryInfo(searchPath);

			foreach(var directoryInfo in searchDirInfo.EnumerateDirectories())
			{
				var relativePath = MakeRelative(rootDirInfo.FullName, directoryInfo.FullName);
				outPathList.Add(relativePath);
			}

			foreach(var directoryInfo in searchDirInfo.EnumerateDirectories())
			{
				GetAllDirectoryPaths(rootPath, directoryInfo.FullName, outPathList);
			}
		}
	}
}

