param functionsHostingPlanName string
param functionsAppName string
param functionsAppStorageName string
param appInsightsInstrumentationKey string
param logAnalyticsId string
param location string = resourceGroup().location
param tags object = {}
param additionalSettings array = []

module storageAccount './storage.bicep' = {
  name: 'Function-Storage'
  params: {
    storageAccountName: functionsAppStorageName
    location: location
    logAnalyticsId: logAnalyticsId
    tags: tags
  }
}
output storageAccountConnectionString string = storageAccount.outputs.storageAccountConnectionString

resource appService 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: functionsHostingPlanName
  location: location
  kind: 'functionapp'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  tags: tags
}

var basicSettings = [
        {
          name: 'AzureWebJobsStorage'
          value: storageAccount.outputs.storageAccountConnectionString
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: storageAccount.outputs.storageAccountConnectionString
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: 'InstrumentationKey=${appInsightsInstrumentationKey}'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
]

var finalSettings = concat(basicSettings, additionalSettings)

resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: functionsAppName
  location: location
  kind: 'functionapp'
  dependsOn:[
    storageAccount
  ]
  
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    serverFarmId: appService.id
    siteConfig: {
      appSettings: finalSettings
    }
  }
  tags: tags
}
