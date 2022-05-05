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

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'cookbook-app-${appEnv}'
  location: location
  properties:{
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      ingress: {
        targetPort:80
        external: false
        allowInsecure: false
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
  // resource auth0 'authConfigs@2022-03-01' = {
  //   name: 'current'
  //   properties: {
  //     globalValidation: {
  //       redirectToProvider: 'authzero'
  //       unauthenticatedClientAction: 'RedirectToLoginPage'
  //     }
  //     identityProviders: {
  //       customOpenIdConnectProviders: {
  //         authzero : {
  //           registration: {
  //             clientCredential: {
  //               clientSecretSettingName: 'auth0-client-secret'
  //             }
  //             clientId: auth0ClientId
  //             openIdConnectConfiguration:{
  //               wellKnownOpenIdConfiguration: 'https://landy-cookbook.eu.auth0.com/.well-known/openid-configuration'
  //             }
  //           }
  //           login: {
  //             nameClaimType: 'name'
  //             scopes: [
  //               'openid'
  //               'profile'
  //               'email'
  //             ]
  //           }
  //         }
  //       }
  //       login: {
  //         preserveUrlFragmentsForLogins: false
  //       }
  //     }
  //     platform: {
  //       enabled: true
  //     }
  //   }
  // }
}



resource recipesContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
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

resource spaApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'cookbook-app-spa-${appEnv}'
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
        appId: 'spa'
        appPort: 80
      }
    }
    template: {
      containers: [
        {
          image: 'landys/cookbook-spa:${imageTag}'
          name: 'recipes-spa'
        }
      ]
    }
  }
}

resource gateway 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'cookbook-app-gateway-${appEnv}'
  location: location
  properties:{
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      ingress: {
        targetPort:80
        external: true
      }
      activeRevisionsMode: 'single'
      dapr: {
        enabled: true
        appId: 'gateway'
        appPort: 80
      }
    }
    template: {
      containers: [
        {
          image: 'landys/cookbook-gateway:${imageTag}'
          name: 'recipes-gateway'
        }
      ]
    }
  }
}
