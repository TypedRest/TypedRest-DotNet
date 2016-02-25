Param (
    [Parameter(Mandatory=$True)] [string]$TargetDir,
    [Parameter(Mandatory=$True)] [string]$ProjectName,
    [Parameter(Mandatory=$True)] [string]$ProjectNamespace,
    [Parameter(Mandatory=$False)] [string]$TemplateName = "WebAPI")
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

mkdir $TargetDir | Out-Null

$templateDir = "$ScriptDir\$TemplateName"
$sourceFiles = ls -Recurse $templateDir -Exclude ("build","packges",".vs","ReSharper.Caches") | Where-Object {$_.Extension -ne ".suo"}
foreach ($x in $sourceFiles)
{
    $sourcePath = $x.FullName
    $targetPath = $sourcePath.Replace($templateDir, $TargetDir).Replace("XProjectNamespaceX", $ProjectNamespace)

    if ($x.PSIsContainer) {
        # Directory
        mkdir $targetPath | Out-Null
    } else {
        # File
        (Get-Content $sourcePath -Encoding UTF8).Replace("XProjectNameX", $ProjectName).Replace("XProjectNamespaceX", $ProjectNamespace) | Out-File $targetPath -Encoding UTF8
    }
}