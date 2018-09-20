#Powershell script to delete webapp,functionapp,storageartifacts container file given corresponding webapp,functionapp,storageaccountname.

param(
[string] $ResourceGroupName = 'AFISOW',
[string] $StorageAccountName = 'afisowsa',
[string] $websiteName = 'afisowwa',
[string] $CommonName = 'azurefi',
[string] $StorageContainerName = $ResourceGroupName.ToLowerInvariant() + "-stageartifacts",
[string] $ClientId = '34361fe4-20c7-44f5-b25c-2de77646f9fa',
[string] $ClientSecret = 'XP4esH3vogfpoC+yMalLD0z8Le8/VOpshWQXbqRZf7A=',
[string] $TenantId = 'fa23f4b5-cee9-4c9e-a774-d31b0f10c151',
[string] $SubscriptionId = '0b349e3e-9da1-454f-941b-1f992729a1ff'
)

#To generate the credentials for the azure account. 
function GetCredentials{
    param($ClientId,$ClientSecret) 
    $securePassword = $ClientSecret | ConvertTo-SecureString -AsPlainText -Force
    $psCredential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $ClientId, $securePassword
    return $psCredential
}
#Gets the UniqueHash for obtaining functionapp name.
function GetUniqueHash{
    param($ClientId,$TenantId,$SubscriptionId)
    $hash = [System.Text.StringBuilder]::new()
    $md5provider = [System.Security.Cryptography.MD5CryptoServiceProvider]::new()
    [byte[]] $bytes = $md5provider.ComputeHash([System.Text.UTF8Encoding]::UTF8.GetBytes($ClientId + $TenantId + $SubscriptionId))
    Foreach ($byte in $bytes)
    {
        $hash.Append($byte.ToString("x2"))
    }
    return ($hash.ToString().Substring(0,24))
}
#Main function
function Main{
    param($ClientId,$TenantId,$ClientSecret,$SubscriptionId)
    try
    {
        $psCredential = GetCredentials -ClientId $ClientId -ClientSecret $ClientSecret
        Set-AzureRmContext -SubscriptionId $SubscriptionId
        Connect-AzureRmAccount -ServicePrincipal -Credential $psCredential -TenantId $TenantId

        if(!(Get-AzureRmWebApp -ResourceGroupName $ResourceGroupName -Name $websiteName -ErrorAction Stop).DefaultHostName){}
        else
        {
             $userOption = Read-Host "Do you want to delete the existing webapp and upload the new one? Enter(Y/N)"
             if($userOption.ToLower() -eq 'y')
             {
                 Write-Host "Deleting the website."
                 #Deleting the webapp and run the code
                 Get-AzureRmWebApp -ResourceGroupName $ResourceGroupName -Name $websiteName
                  Write-Host "Deleted the website."
             }
        }
    }
    catch [Microsoft.Rest.Azure.CloudException]
    {
        Write-Host $_.Exception.Message
        Write-Host "There is no WebApp in azure with the given WebAppName."
        #Go to deployment of code
        #create a class with already existed code.
        #Now run the code.   
    }
    catch
    {
       Write-Host $_.Exception.Message
    }
    try
    {
       [string]$uniqueHash = (GetUniqueHash -ClientId $ClientId -TenantId $TenantId -SubscriptionId $SubscriptionId)
       $uniqueHash=$uniqueHash.Substring(0,24)
       [string]$functionAppName = $CommonName + $uniqueHash
       if(!(Get-AzureRmWebApp -ResourceGroupName $ResourceGroupName -Name $functionAppName -ErrorAction Stop).DefaultHostName){}
       else
       {
             $userOption = Read-Host "Do you want to delete the existing functionapp and upload the new one? Enter(Y/N)"  
             if($userOption.ToLower() -eq 'y')
             {
                Write-Host "Deleting the functionapp."
                #Deleting the functionapp and run the code.
                Get-AzureRmWebApp -ResourceGroupName $ResourceGroupName -Name $functionAppName
                Write-Host "Deleted the functionapp."
             }
       }
    }   
    catch [Microsoft.Rest.Azure.CloudException]
    {
       Write-Host $_.Exception.Message
       Write-Host "There is no FunctionApp in azure with the given FunctionAppName."
       #Go to deployment of code
       #create a class with already existed code.
       #Now run the code.   
    }
    catch
    {
       Write-Host $_.Exception.Message
    }
    try
    {
       if(!(Get-AzureRmStorageAccountKey -ResourceGroupName $ResourceGroupName -AccountName $StorageAccountName -ErrorAction Stop)[0].Value){}
       else
       {
           $primaryKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $ResourceGroupName -AccountName $StorageAccountName -ErrorAction Stop)[0].Value 
           $storageContext = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $primaryKey -ErrorAction Stop
           #$storageContext = New-AzureStorageContext -ConnectionString ("DefaultEndpointsProtocol=https;AccountName=" + $storageAccountName + ";AccountKey=" + $primaryKey)
           if(!(Get-AzureStorageContainer -Context $storageContext -Name $StorageContainerName -ErrorAction Stop).Name){}
           else
           {
                $userOption = Read-Host "Do you want to delete the existing container and upload the new one? Enter(Y/N)"
                if($userOption.ToLower() -eq 'y')
                {
                    Write-Host "Deleting the container."
                    #Delete the container and run the code.
                    Get-AzureStorageContainer -Context $storageContext -Name $StorageContainerName -ErrorAction Stop
                    Write-Host "Deleted the container."  
                }
           }
       }
    }
    catch [Microsoft.Rest.Azure.CloudException]
    {
        Write-Host $_.Exception.Message
        Write-Host "Check for valid resourceroupname and storageaccountname."
    }
    catch [System.Management.Automation.RuntimeException]
    {
        Write-Host $_.Exception.Message
        Write-Host "Check for valid resourceroupname,storageaccountname,storageaccountkey,storagecontext and container name."
        #Go to deployment of code
        #create a class with already existed code.
        #Now run the code.   
    }
    catch [Microsoft.WindowsAzure.Commands.Storage.Common.ResourceNotFoundException]
    {
        Write-Host $_.Exception.Message
        Write-Host "The container name is not valid.Please check for a valid container name."
    }
    catch [System.FormatException]
    {
        Write-Host $_.Exception.Message
        Write-Host "Check whether the separation of storage key is done while extracting the storage key.Or else check for valid variable assigned for the parameter StorageAccountKey"
    }
    catch
    {
        Write-Host $_.Exception.Message
    }
}

Main -ClientId $ClientId -TenantId $TenantId -ClientSecret $ClientSecret -SubscriptionId $SubscriptionId