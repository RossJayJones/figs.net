param($installPath, $toolsPath, $package, $project)

	# Need to load MSBuild assembly if it's not loaded yet.
	add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
	
	# Grab the loaded MSBuild project for the project
	$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1 
	
	# Remove the import and save the project
	$ExistingImports = $msbuild.Xml.Imports | Where-Object { $_.Project -like "*\Figs.Targets" }
	if ($ExistingImports) {
	    $ExistingImports | ForEach-Object { $msbuild.Xml.RemoveChild($_) | Out-Null }
	    $project.Save()
	}