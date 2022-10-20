@description('application environment (dev/prod/etc)')
param appEnv string = 'dev'
param location string = resourceGroup().location

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  name: 'household-configstore-${appEnv}'
  location: location
  sku: {
    name: 'free'
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'household-loganalytics-${appEnv}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'household-ai-${appEnv}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: 'household-dbaccount-${appEnv}'
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
  name: '${cosmosAccount.name}/household-db-${appEnv}'
  properties: {
    resource: {
      id: 'household-db-${appEnv}'
    }
    options: {
      throughput: 400
    }
  }
}

resource staticWebApp 'Microsoft.Web/staticSites@2022-03-01' = {
  name: 'household-staticsite-${appEnv}'
  location: location
  sku:{
    name: 'Free'
    tier: 'Free'
  }
}

// resource staticWebAppLinkedBackend 'Microsoft.Web/staticSites/linkedBackends@2022-03-01' = {
//   name: 'household-staticsite-backend-${appEnv}'
//   parent: staticWebApp
//   properties: {
//     backendResourceId: appService.id
//   }
// }

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'household-appserviceplan-${appEnv}'
  location: location
  sku: {
    name: 'B1'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: 'household-appservice-${appEnv}'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|6.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'cosmosDb_endpoint'
          value: cosmosAccount.properties.documentEndpoint
        }
        {
          name: 'cosmosDb_key'
          value: cosmosAccount.listKeys().primaryMasterKey
        }
        {
          name: 'cosmosDb_databaseName'
          value: cosmosDB.properties.resource.id
        }
        {
          name: 'cosmosDb_containers_users'
          value: 'Users'
        }
        {
          name: 'cosmosDb_containers_recipes'
          value: 'Recipes'
        }
      ]
      connectionStrings: [
        {
          name: 'appCfg'
          connectionString: appConfig.listKeys().value[2].connectionString
        }
      ]
      alwaysOn: true
    }
    httpsOnly:true
    
  }
}
