param($installPath, $toolsPath, $package, $project)
	
	# Clean up older version of figs
	$proj = [xml](get-content $project.FullName)
	$target = $proj.GetElementsByTagName("Target") |? { $_.Name -eq "AfterBuild" -and $_.GetElementsByTagName("Figs").Count -gt 0 }
	if ($target) { $proj.ChildNodes[1].RemoveChild($target) }
	$import = $proj.GetElementsByTagName("Import") |? { $_.Project.Contains("Figs") }
	if($import) { $proj.ChildNodes[1].RemoveChild($import) }
	$proj.save($project.FullName)
	
	# Need to load MSBuild assembly if it's not loaded yet.
	add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
	
	# Grab the loaded MSBuild project for the project
	$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1 
	
	# Add the import and save the project
	$targets = '..\packages\' + $package.Id + '.' + $package.Version + '\tools\Figs.Targets'
	$msbuild.Xml.AddImport($targets) | out-null
	$project.Save()