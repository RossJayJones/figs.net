param($projectName, $path)    
    $project = Get-Project $projectName 
    # Need to load MSBuild assembly if it's not loaded yet.
    Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
    # Grab the loaded MSBuild project for the project
    $msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1 
    # Add the import and save the project
    $msbuild.Xml.AddImport('$(MSBuildExtensionsPath)\\Figs\\Trackmatic.Figs.Targets') | out-null
    $target = $msbuild.Xml.AddTarget('AfterBuild')
    $task = $target.AddTask('Figs')
    $task.SetParameter('ConigurationFilePath', $path)
    $task.SetParameter('ProjectName', '$(MSBuildProjectName)')
    $task.SetParameter('BinPath', '$(OutputPath)')
    $task.SetParameter('OutputType', '$(OutputType)')
    $project.Save()