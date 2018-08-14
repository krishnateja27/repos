Param(
   <# [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,#>
    [string] $ResourceGroupName = 'ARMTemplate-WebApp',
    [switch] $UploadArtifacts,
    [string] $StorageAccountName,
    [string] $StorageContainerName = $ResourceGroupName.ToLowerInvariant() + '-stageartifacts',
    [string] $TemplateFile = 'WebSite.json',
    [string] $TemplateParametersFile = 'WebSite.parameters.json',
    [string] $ArtifactStagingDirectory = '.',
    [string] $DSCSourceFolder = 'DSC',
    [switch] $ValidateOnly
)
$JsonParameters = Get-Content $TemplateParametersFile -Raw | ConvertFrom-Json
Write-Host $JsonParameters.parameters.webSiteName.value
$ClientId = $JsonParameters.parameters.clientID.value
Write-Host $ClientId
$ClientSecret = $JsonParameters.parameters.clientSecretKey.value
Write-Host $ClientSecret
$TenantId = $JsonParameters.parameters.tenantID.value
Write-Host $TenantId
$SubscriptionId = (Get-AzureRmContext).Subscription.SubscriptionId
Write-Host $SubscriptionId