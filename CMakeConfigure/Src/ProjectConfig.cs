using System;
using System.Collections.Generic;

namespace CMakeConfigure
{
	// ARGS
	public enum EArgType
	{
		TEMPLATES_PATH = 0,
		OUTPUT_PATH = 1,
		PROJECT_TYPE = 2,
		PROJECT_NAME = 3,
		PROJECT_ENVIRONMENT = 4,
		MAX_ARGS = 5
	}

	public static class ProjectConfig
	{
		// EXTENSIONS
		public static readonly string[] SRC_FILE_EXT =
		{
			".cpp",
		};

		public static readonly string[] INCLUDE_FILE_EXT =
		{
			".h",
			".inl",
		};

		public static readonly string SRC_FOLDER = "src";
		public static readonly string INCLUDE_FOLDER = "include";

		public static readonly string SHARED_LIB = "SHARED";
		public static readonly string STATIC_LIB = "STATIC";
		public static readonly string HEADER_LIB = "HEADER";

		public static readonly string STATICLIB_TEMPLATE_FILENAME = "CMakeLists_StaticLib_Template.txt";
		public static readonly string SHAREDLIB_TEMPLATE_FILENAME = "CMakeLists_SharedLib_Template.txt";
		public static readonly string EXECUTABLE_TEMPLATE_FILENAME = "CMakeLists_Executable_Template.txt";
		public static readonly string HEADER_TEMPLATE_FILENAME = "CMakeLists_HeaderLib_Template.txt";

		public static readonly string CMAKELIST_OUTPUT_FILENAME = "CMakeLists.txt";

		// Template keys
		public static readonly string PRJ_NAME_KEY = "#[PROJECT_NAME]";
		public static readonly string BUILD_TYPE_KEY = "#[BUILD_TYPE]";

		public static readonly string PRJ_INCLUDE_KEY = "#[PRJ_INCLUDE]";
		public static readonly string PRJ_SOURCES_KEY = "#[PRJ_SOURCE_FILES]";
		public static readonly string PRJ_HEADERS_KEY = "#[PRJ_HEADER_FILES]";

		public static readonly string EXTERNAL_KEY = "#[EXTERNAL_PROJECTS]";
		public static readonly string EXTERNAL_TARGET = "#[EXTERNAL_TARGET]";

		// CONSTANTS
		public static readonly uint LIBRARIES_INDEX = (int) EArgType.MAX_ARGS;
		
		// TEST VALUES
		public static readonly string TEMPLATES_PATH = @"..\Templates";
		public static readonly string FILE_OUTPUT_PATH = @"..\Test\project";

		public static readonly string PROJECT_TYPE = "EXECUTABLE";
		public static readonly string PROJECT_NAME = "Test";
		public static readonly string PROJECT_ENVIRONMENT = "Debug";
		public static readonly string PROJECT_EXTERNALS = "project_a project_b";

		// METHODS
		public static string GetTemplateFilename(string templateType)
		{
			if (templateType == SHARED_LIB)
			{
				return SHAREDLIB_TEMPLATE_FILENAME;
			}
			else if (templateType == STATIC_LIB)
			{
				return STATICLIB_TEMPLATE_FILENAME;
			}
			else if (templateType == HEADER_LIB)
			{
				return HEADER_TEMPLATE_FILENAME;
			}
			return EXECUTABLE_TEMPLATE_FILENAME;
		}

		public static void FillLibraryNameList(string[] args, ref List<string> outLibraryNameList)
		{
			outLibraryNameList.Clear();
			for (uint i = 5; i < args.Length; ++i)
			{
				outLibraryNameList.Add(args[i]);
			}
		}

		public static bool FillArgs(string[] args, ref string[] outArgs)
		{
			if (args.Length < LIBRARIES_INDEX)
			{
#if DEBUG

				outArgs[(int)EArgType.TEMPLATES_PATH] = TEMPLATES_PATH;
				outArgs[(int)EArgType.OUTPUT_PATH] = FILE_OUTPUT_PATH;
				outArgs[(int)EArgType.PROJECT_TYPE] = PROJECT_TYPE;
				outArgs[(int)EArgType.PROJECT_NAME] = PROJECT_NAME;
				outArgs[(int)EArgType.PROJECT_ENVIRONMENT] = PROJECT_ENVIRONMENT;

				var externalArray = PROJECT_EXTERNALS.Split(' ');
				for (uint i = 0; i < externalArray.Length; ++i)
				{
					outArgs[LIBRARIES_INDEX + i] = externalArray[i];
				}
				return true;
#else
				return false;
#endif
			}

			outArgs[(int)EArgType.TEMPLATES_PATH] = args[(int)EArgType.TEMPLATES_PATH];
			outArgs[(int)EArgType.OUTPUT_PATH] = args[(int)EArgType.OUTPUT_PATH];
			outArgs[(int)EArgType.PROJECT_TYPE] = args[(int)EArgType.PROJECT_TYPE];
			outArgs[(int)EArgType.PROJECT_NAME] = args[(int)EArgType.PROJECT_NAME];
			outArgs[(int)EArgType.PROJECT_ENVIRONMENT] = args[(int)EArgType.PROJECT_ENVIRONMENT];

			for (uint i = LIBRARIES_INDEX; i < args.Length; ++i)
			{
				outArgs[i] = args[i];
			}

			return true;
		}
	}
}

