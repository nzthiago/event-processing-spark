param location string = resourceGroup().location
param appInsightsName string
param logAnalyticsId string
param tags object = {}



resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties:{
    Application_Type:'web'
    WorkspaceResourceId: logAnalyticsId
  }
}


resource appInsightsDiagnosticSettings 'microsoft.insights/diagnosticSettings@2017-05-01-preview' = {
  name: 'Send_To_LogAnalytics'
  scope: appInsights
  properties: {
    workspaceId: logAnalyticsId
    logs: [
      {
        category: 'AppAvailabilityResults'
        enabled: true
      }
      {
        category: 'AppEvents'
        enabled: true
      }
      {
        category: 'AppMetrics'
        enabled: true
      }
      {
        category: 'AppDependencies'
        enabled: true
      }
      {
        category: 'AppExceptions'
        enabled: true
      }
      {
        category: 'AppPageViews'
        enabled: true
      }
      {
        category: 'AppPerformanceCounters'
        enabled: true
      }
      {
        category: 'AppRequests'
        enabled: true
      }
      {
        category: 'AppSystemEvents'
        enabled: true
      }
      {
        category: 'AppTraces'
        enabled: true
      }
      {
        category: 'AppBrowserTimings'
        enabled: true
      }
    ]
    metrics: [
      {
        enabled: true
        category: 'AllMetrics'
      }
    ]
  }
}


output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey
output appInsightsIngestionEndpoiont string = appInsights.properties.publicNetworkAccessForIngestion
output appInsightsConnectionString string = appInsights.properties.ConnectionString