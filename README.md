figs.net
========

Scalable .Net Configuration Management

Figs.Net sets out to solve the problem of managing .Net configurations when the code base starts to get large. .Net configuration files, as usefull as they are, do not appear to scale well when you are required to manage configuration across multiple projects and solutions. This is largely because a vast majority of the configuration stays the same between developers and environments, with only key values having to change.

We required a system which would allow us to define different configuration's simply to be applied across the entire project and fit in easily with automated builds.

The ablity to easily change configuraton settings without breaking other peoples/systems environments is critical when there are multiple players working on the project.

Figs.Net works by introducing a build task which is triggered on the "AfterBuild" hook. The task parses the congfg file and replaces the key areas of the configuration with values from a much simpler json configuration file which applies the settings project/solution wide. This allows you to switch between environments quickly, only having to build your code for the changes to be applied.

Since it leverages off MSBuild it will fit well into automated build environments. In fact nothing actually needs to changed since all the work is executed by the compiler.
