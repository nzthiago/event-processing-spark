param location string = resourceGroup().location
param resourcenamePrefix string = uniqueString(resourceGroup().id)
param environmedntNamePrefix string = resourceGroup().location
param sensorType string = 'yellow'

// Based on recommended naming conventions at:
// https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging
// Changes these to parameters for better felexibility
var functionAppName = 'func-${resourcenamePrefix}-${environmedntNamePrefix}'

var eventHubNamespaceName = take('evhns-${resourcenamePrefix}-${environmedntNamePrefix}', 50)
var inputEventHubName = 'evh-${resourcenamePrefix}-in-${environmedntNamePrefix}'
var outputEventHubName = 'evh-${resourcenamePrefix}-out-${environmedntNamePrefix}'

var outputStorageAccountName = take(toLower('st${resourcenamePrefix}${environmedntNamePrefix}${uniqueString(resourceGroup().name)}'), 23)

var functionsHostingPlanName = take('plan-${resourcenamePrefix}-${environmedntNamePrefix}', 40)
var functionsAppName = take('func-${resourcenamePrefix}-${environmedntNamePrefix}', 60)
var functionsAppStorageName = take('stfunc${resourcenamePrefix}${environmedntNamePrefix}', 23)

var logAnalyticsName = take('log-${resourcenamePrefix}-${environmedntNamePrefix}', 63)
var appInsightsName = take('appi-${resourcenamePrefix}-${environmedntNamePrefix}', 255)


var defaultTags = {
  Environment: environmedntNamePrefix
}


module logAnalytics './logAnalytics.bicep' = {
  name: 'logAnalytics'
  params: {
    location: location
    logAnalyticsName: logAnalyticsName
    tags: defaultTags
  }
}

module applicationInsights './appInsights.bicep' = {
  name: 'appInsights'
  params: {
    location: location
    appInsightsName: appInsightsName
    logAnalyticsId: logAnalytics.outputs.logAnalyticsId
    tags: defaultTags
  }
}


module eventHubs './eventHub.bicep'= {
  name: 'eventHubs'
  params:{
    location: location
    eventHubNamespaceName: eventHubNamespaceName
    inputHubName: inputEventHubName
    outputHubName: outputEventHubName
    functionName: functionAppName
    logAnalyticsId: logAnalytics.outputs.logAnalyticsId
    tags: defaultTags
  }
}

module  storageAccount './storage.bicep'={
  name: 'storage'
  params:{
    location: location
    storageAccountName: outputStorageAccountName
    logAnalyticsId: logAnalytics.outputs.logAnalyticsId
    tags: defaultTags
  }
}


module functionApp './functionApp.bicep'={
  name: 'functionApp'
  params:{
    functionsAppName: functionAppName
    functionsHostingPlanName: functionsHostingPlanName
    functionsAppStorageName: functionsAppStorageName
    appInsightsInstrumentationKey: applicationInsights.outputs.appInsightsInstrumentationKey
    logAnalyticsId: logAnalytics.outputs.logAnalyticsId
    location: location
    tags: defaultTags
    //Send in additional settings that connect the function to other components
    additionalSettings:[
      {
        name: 'Input_EH_Name'
        value: eventHubs.outputs.inputHubName
      }
      {
        name: 'InputEventHubConnectionString'
        value: eventHubs.outputs.eventHubConnectionStringListen
      }
      {
        name: 'Input_EH_ConsumerGroup'
        value: eventHubs.outputs.inputConsumerGroup
      }
      {
        name: 'Output_EH_Name'
        value: eventHubs.outputs.outputHubName
      }
      {
        name: 'OutputEventHubConnectionString'
        value: eventHubs.outputs.eventHubConnectionStringSend
      }
      {
        name: 'SENSOR_TYPE'
        value: sensorType
      }
    ]
  }
}

output Input_EH_Name string = eventHubs.outputs.inputHubName 
output Input_EH_ConsumerGroup string = eventHubs.outputs.inputConsumerGroup
output Output_EH_Name string = eventHubs.outputs.outputHubName
output SENSOR_TYPE string = sensorType
output AzureWebJobsStorage string = functionApp.outputs.storageAccountConnectionString
output LocalConsumerStorage string = storageAccount.outputs.storageAccountConnectionString
output EventHubConnectionStringListen string = eventHubs.outputs.eventHubConnectionStringListen
output EventHubConnectionStringSend string = eventHubs.outputs.eventHubConnectionStringSend
