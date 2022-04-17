@description('application environment (dev/prod/etc)')
param appEnv string = 'dev'
@description('docker image tag')
param imageTag string = 'latest'
param location string = resourceGroup().location


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

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: 'cookbook-app-${appEnv}'
  location: location
  properties:{
    managedEnvironmentId: appEnvironment.id
    configuration: {
      ingress: {
        targetPort:80
        external: true
      }
      activeRevisionsMode: 'single'
      secrets: [
        {
          name: 'cosmos-endpoint'
          value: cosmosAccount.properties.documentEndpoint
        }
        {
          name: 'cosmos-key'
          value:  cosmosAccount.listKeys().primaryMasterKey
        }
        {
          name:'ai-intrumentation-key'
          value: appInsights.properties.InstrumentationKey
        }
      ]
    }
    template: {
      containers: [
        {
          image: 'landys/cookbook:${imageTag}'
          name: 'cookbook-web'
          env:[
            {
              name: 'cosmosDb__connectionString'
              secretRef: 'cosmos-endpoint'
            }
            {
              name: 'cosmosDb__key'
              secretRef: 'cosmos-key'
            }
            {
              name: 'cosmosDb__databaseName'
              value: cosmosDB.properties.resource.id
            }
            {
              name: 'cosmosDb__containers__users'
              value: 'Users'
            }
            {
              name: 'cosmosDb__containers__recipes'
              value: 'Recipes'
            }
            {
              name: 'cosmosDb__containers__refreshTokens'
              value: 'RefreshTokens'
            }
            {
              name: 'appinsightsinstrumentationkey'
              secretRef: 'ai-intrumentation-key'
            }
          ]
        }
      ]
    }
  }
}