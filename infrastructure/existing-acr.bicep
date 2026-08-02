@description('The naming prefix for all resources.')
param prefix string = 'contoso'

var uniqueSubString = uniqueString(resourceGroup().id)
var acrName = '${prefix}acr${uniqueSubString}'

resource acr 'Microsoft.ContainerRegistry/registries@2025-11-01' existing = {
  name: acrName
}

output acrLoginServer string = acr.properties.loginServer
output acrName string = substring(acr.properties.loginServer, 0, indexOf(acr.properties.loginServer, '.'))