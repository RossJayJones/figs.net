param($installPath, $toolsPath, $package, $project)
	
	# Clean up older version of figs
	$proj = [xml](get-content $project.FullName)
	$target = $proj.GetElementsByTagName("Target") |? { $_.Name -eq "AfterBuild" -and ($_.GetElementsByTagName("Figs").Count -gt 0 -or $_.GetElementsByTagName("FigsAlongSide").Count -gt 0) }
	if ($target) { $proj.ChildNodes[1].RemoveChild($target) }
	$import = $proj.GetElementsByTagName("Import") |? { $_.Project.Contains("Figs") }
	if($import) { $proj.ChildNodes[1].RemoveChild($import) }
	
	# Add new import
	$element = $proj.CreateElement("Import", "http://schemas.microsoft.com/developer/msbuild/2003")
	$attribute = $proj.CreateAttribute("Project")
	$attribute.Value = "..\packages\" + $package.Id + "." + $package.Version + "\tools\Figs.Targets"
	$element.Attributes.Append($attribute) | out-null
	$proj.ChildNodes[1].AppendChild($element) | out-null
	$proj.save($project.FullName)