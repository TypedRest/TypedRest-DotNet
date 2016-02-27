Param (
    [Parameter(Mandatory=$True)] [string]$TargetDir,
    [Parameter(Mandatory=$True)] [string]$ProjectName,
    [Parameter(Mandatory=$True)] [string]$ProjectNamespace,
    [Parameter(Mandatory=$False)] [string]$TemplateName = "WebAPI")
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

mkdir $TargetDir | Out-Null

$templateDir = "$ScriptDir\$TemplateName"
foreach ($x in (ls -Recurse $templateDir))
{
    if (($x.Name -eq "ConnectionStrings.config") -or ((".suo",".user") -contains $x.Extension) -or `
        $x.FullName.Contains("\build\") -or $x.FullName.Contains("\packages\") -or $x.FullName.Contains("\.vs\") -or $x.FullName.Contains("\_ReSharper.Caches\") -or $x.FullName.Contains("\obj\") -or $x.FullName.Contains("\bin\") )
    { continue }

    $sourcePath = $x.FullName
    $targetPath = $sourcePath.Replace($templateDir, $TargetDir).Replace("XProjectNamespaceX", $ProjectNamespace).Replace(".nuspec.template", ".nuspec")

    if ($x.PSIsContainer) {
        # Directory
        mkdir $targetPath | Out-Null
    } else {
        # File
        $encoding = @{$true="Ascii";$false="UTF8"}[$x.Extension -eq '.cmd']  
        (Get-Content $sourcePath -Encoding $encoding).Replace("XProjectNameX", $ProjectName).Replace("XProjectNamespaceX", $ProjectNamespace) | Out-File $targetPath -Encoding $encoding
    }
}