param location string = resourceGroup().location
param storageAccountName string
param logAnalyticsId string
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2020-08-01-preview'={
  name: storageAccountName
  location: location
  kind:'StorageV2'
  sku:{
    name:'Standard_LRS'
    tier: 'Standard'
  }
}

resource eventHubNamespaceDiagnosticSettings 'microsoft.insights/diagnosticSettings@2017-05-01-preview' = {
   name: 'Send_To_LogAnalytics'
  scope: storageAccount
  properties: {
    workspaceId: logAnalyticsId
   
    metrics: [
      {
        enabled: true
        category: 'Transaction'
      }
    ]
  }
}

output storageAccountConnectionString string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'