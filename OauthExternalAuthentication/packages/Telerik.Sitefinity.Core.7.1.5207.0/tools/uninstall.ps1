param($installPath, $toolsPath, $package, $project)

	. (Join-Path $toolsPath "CommonPropertyValues.ps1") 

	$project.Object.References |  Where-Object { $_.Name -eq $referenceAssemblyName } | foreach { $_.Remove() }

	$project.Save()