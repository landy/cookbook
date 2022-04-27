@description('application environment (dev/prod/etc)')
param appEnv string = 'dev'
@description('docker image tag')
param imageTag string = 'latest'
param location string = resourceGroup().location


resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'cookbook-keyvault-dev'
  scope: resourceGroup()
}


resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: 'cookbook-log-analytics-${appEnv}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'cookbook-ai-${appEnv}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: 'cookbook-db-account-${appEnv}'
  location: location
  properties: {
    enableFreeTier: true
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
      }
    ]
  }
}

resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' = {
  name: '${cosmosAccount.name}/cookbook-db-${appEnv}'
  properties: {
    resource: {
      id: 'cookbook-db-${appEnv}'
    }
    options: {
      throughput: 400
    }
  }
}

resource appEnvironment 'Microsoft.App/managedEnvironments@2022-01-01-preview' = {
  name: 'cookbook-app-env-${appEnv}'
  location: location
  properties: {
    daprAIInstrumentationKey: appInsights.properties.InstrumentationKey
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2021-10-01-preview' = {
  name: 'cookbook-app-config-${appEnv}'
  location: location
  sku: {
    name: 'standard'
  }
}

var settings = [
  {
    key: 'cosmosDb:containers:users'
    value: 'Users'
  }
  {
    key: 'cosmosDb:containers:recipes'
    value: 'Recipes'
  }
  {
    key: 'cosmosDb:containers:refreshTokens'
    value: 'RefreshTokens'
  }
]

resource appConfigValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2021-10-01-preview' = [for (item,i) in settings: {
  parent: appConfig
  name: item.key
  properties: {
    value: item.value
  }
}]

module containerApp 'container-apps.bicep' = {
  name: '${deployment().name}-container-app'
  params: {
    appEnv: appEnv
    location: location
    containerAppEnvironmentId: appEnvironment.id
    cosmosDocumentEndpoint: cosmosAccount.properties.documentEndpoint
    cosmosPrimaryKey: cosmosAccount.listKeys().primaryMasterKey
    cosmosDbName: cosmosDB.properties.resource.id
    appConfigConnectionString: appConfig.listKeys().value[2].connectionString
    auth0Secret: keyVault.getSecret('auth0secret')
    auth0ClientId: keyVault.getSecret('auh0clientid')
    imageTag: imageTag
  }
}


