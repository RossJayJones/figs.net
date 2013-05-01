figs.net
========

Scalable .Net Configuration Management

Figs.Net sets out to solve the problem of managing .Net configurations when the code base starts to get large. .Net configuration files, as usefull as they are, do not appear to scale well when you are required to manage configuration across multiple projects and solutions. This is largely because a vast majority of the configuration stays the same between developers and environments, with only key values having to change.

We required a system which would allow us to define different configuration's simply to be applied across the entire project and fit in easily with automated builds.

The ablity to easily change configuraton settings without breaking other peoples/systems environments is critical when there are multiple players working on the project.

Figs.Net works by introducing a build task which is triggered on the "AfterBuild" hook. The task parses the congfg file and replaces the key areas of the configuration with values from a much simpler json configuration file which applies the settings project/solution wide. This allows you to switch between environments quickly, only having to build your code for the changes to be applied.

Since it leverages off MSBuild it will fit well into automated build environments. In fact nothing actually needs to changed since all the work is executed by the compiler.

Installation
========

To install Figs.Net you need to copy the output of Trackmatic.Figs under a "Figs" folder in the MSBuildExtensions directory. Normally this is C:\Program Files (x86)\MSBuild

A script is available, "figs-install.ps1", which will add the relevant tags to your project file. The script requires NuGet to be installed since it leverages of some of the new get commands. The easiest way to run the script is to drop it in the same folder as your .sln file, and run the script from the "Package Manager Console" in Visual Studion found under Tools ->  Library Package Manager -> Package Manager Console

The script takes 2 parameters, -ProjectName which identifies the project which you want to apply Figs.Net to and -Path which is the relative path of your settings.json file. If you have maintained a nice flat structure for your solution it will normall be ../settings.json. The structure described here is represented below:

Main Folder
---Solution.sln
---settings.json
---ProjectOne
------ProjectOne.csproj
---ProjectTwo
------ProjectTwo.csproj

Usage
========

Simply repce the values in your .net config file with a token pre-fixed with "{{" and appended with "}}". Figs.Net parses the file and replaces these tokens with a value from the settings.json file. The json file also indicates which configuratiom must be used

Sample Config

<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="sample1" value="{{sample1}}" />
    <add key="sample2" value="{{sample2}}" />
  </appSettings>
</configuration>

Sample Json

{
	"development": {
		"sample1" : "dev value 1",
		"sample2" : "dev value 2"
	},
	"production": {
		"sample1" : "prod value 1",
		"sample2" : "prod value 2"
	},
	"configuration": {
		"active" : "production"
	}
}

When the solution with the above project is built Figs.Net will replace {{smpale1}} with value "prod value 1" and so on.