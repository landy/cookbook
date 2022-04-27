param appEnv string
param location string
param containerAppEnvironmentId string
param cosmosDocumentEndpoint string
param cosmosDbName string
param imageTag string
@secure()
param cosmosPrimaryKey string
@secure()
param appConfigConnectionString string
@secure()
param auth0Secret string
@secure()
param auth0ClientId string

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: 'cookbook-app-${appEnv}'
  location: location
  properties:{
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      ingress: {
        targetPort:80
        external: true
      }
      dapr: {
        enabled: true
        appId: 'cookbook-app'
        appPort: 80
      }
      activeRevisionsMode: 'single'
      secrets: [
        {
          name: 'cosmos-endpoint'
          value: cosmosDocumentEndpoint
        }
        {
          name: 'cosmos-key'
          value:  cosmosPrimaryKey
        }
        {
          name: 'app-config'
          value: appConfigConnectionString
        }
        {
          name: 'auth0-client-secret'
          value: auth0Secret
        }
      ]
    }
    template: {
      containers: [
        {
          image: 'landys/cookbook-api:${imageTag}'
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
              value: cosmosDbName
            }
            {
              name: 'ConnectionStrings__appCfg'
              secretRef: 'app-config'
            }
          ]
        }
      ]
    }
  }
}

resource auth0 'Microsoft.App/containerApps/authConfigs@2022-01-01-preview' = {
  name: 'current'
  parent: containerApp
  properties: {
    globalValidation: {
      unauthenticatedClientAction: 'AllowAnonymous'
    }
    identityProviders: {
      customOpenIdConnectProviders: {
        auth0 : {
          registration: {
            clientCredential: {
              clientSecretRefName: 'auth0-client-secret'
            }
            clientId: auth0ClientId
            openIdConnectConfiguration:{
              wellKnownOpenIdConfiguration: 'https://landy-cookbook.eu.auth0.com/.well-known/openid-configuration'
            }
          }
        }
      }
    }
    login: {
      preserveUrlFragmentsForLogins: 'False'
    }
    state: 'Enabled'
  }
}

resource recipesContainerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: 'cookbook-app-recipes-${appEnv}'
  location: location
  properties:{
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      ingress: {
        targetPort:80
        external: false
      }
      activeRevisionsMode: 'single'
      dapr: {
        enabled: true
        appId: 'recipes-api'
        appPort: 80
      }
      secrets: [
        {
          name: 'cosmos-endpoint'
          value: cosmosDocumentEndpoint
        }
        {
          name: 'cosmos-key'
          value:  cosmosPrimaryKey
        }
        {
          name: 'app-config'
          value: appConfigConnectionString
        }
      ]
    }
    template: {
      containers: [
        {
          image: 'landys/cookbook-recipes:${imageTag}'
          name: 'recipes-api'
          probes: [
            {
              type: 'liveness'
              httpGet: {
                path: '/liveness'
                port: 80
              }
            }
          ]
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
              value: cosmosDbName
            }
            {
              name: 'ConnectionStrings__appCfg'
              secretRef: 'app-config'
            }
          ]
        }
      ]
    }
  }
}
