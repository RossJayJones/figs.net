Framework "4.5.2x64"

properties {
	$base_dir = $psake.build_script_dir
    $xunit_runner = "$base_dir\packages\xunit.runner.console.2.1.0\tools\xunit.console.exe"
    $destination_dir = $output    
    $nuget = "$base_dir\.nuget\NuGet.exe"
}

Task default -depends Clean, Build, Test

Task Publish {
    $apikey = Read-Host -Prompt 'Enter Api Key'
    publish-package "Figs" $apikey  
}

Task Build {

    exec { msbuild "$base_dir/Figs.sln" }
}

Task Clean {
    Get-ChildItem ./ -include bin,obj -Recurse -Force | % { 
        write-host "Cleaning $_"
        Remove-Item $_ -Recurse -Force  
    }
}

Task Test {
    # NSpec Tests
    exec { & $xunit_runner "$base_dir\Plot.Tests\bin\Debug\Plot.Tests.dll" }
}

function publish-package($nuspec, $apikey) {
    $file = "$base_dir\$nuspec\$nuspec.nuspec"
    write-host $file
    $spec = [xml](get-content $file)
    $version = $spec.package.metadata.version
    $package = "$base_dir\$nuspec.$version.nupkg"
    $package_dir = "$base_dir\$nuspec\bin\package"
    
    # Prepare package folder
    remove-item $package_dir -R -ErrorAction SilentlyContinue
    new-item -itemtype directory "$package_dir"
    new-item -itemtype directory "$package_dir/tools"
    
    # Copy nuspec file to package folder
    copy-item "$base_dir\$nuspec\$nuspec.nuspec" "$package_dir\$nuspec.nuspec"
    copy-item "$base_dir\$nuspec\bin\debug\Figs.dll" "$package_dir\tools"
    copy-item "$base_dir\$nuspec\bin\debug\Newtonsoft.Json.dll" "$package_dir\tools"
    copy-item "$base_dir\$nuspec\tools\Figs.Targets" "$package_dir\tools"
    copy-item "$base_dir\$nuspec\tools\install.ps1" "$package_dir\tools"
    copy-item "$base_dir\$nuspec\tools\uninstall.ps1" "$package_dir\tools"

    # Create nuget package and upload to nuget
    exec { & $nuget pack "$package_dir/$nuspec.nuspec" }
    #exec { & $nuget setApiKey $apikey }
    #exec { & $nuget push "$package" }

    # Perform some cleanup on the folder
    #remove-item $package
    #remove-item $package_dir -R -ErrorAction SilentlyContinue
}