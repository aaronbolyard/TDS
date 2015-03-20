solution "TDS"
	configurations { "Debug", "Release" }
	platforms { "x86" }
	architecture "x32"
	location "Solution"

	filter "configurations:Debug"
		targetdir "Bin/Debug"

	filter "configurations:Release"
		targetdir "Bin/Release"

project "TormentedDemonSimulator"
	language "C#"
	kind "ConsoleApp"
	files { "Source/**.cs" }
	links { "System.dll" }