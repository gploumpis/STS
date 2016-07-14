angular.module('STS.RP.Static.Angular', ['ui.router'])
.config(['$stateProvider', '$urlRouterProvider', '$httpProvider', function ($stateProvider, $urlRouterProvider, $httpProvider) {

    $urlRouterProvider.when('', '/home');
    $urlRouterProvider.otherwise('/home');

    $stateProvider
        .state('app', {
            abstract: true,
            template: '<ui-view></ui-view>'
        })
        .state('app.home', {
            url: '/home',
            template: 'Hello there!!!  <a ui-sref="^.claim.list">View Claims</a>'
        })
        .state('app.claim', {
            url: '/claims',
            abstract:true,
            controller: 'ClaimsController',
            template: '<ui-view></ui-view>'
        })
        .state('app.claim.list', {
            url:'',
            templateUrl: 'claims.html'
        })
        .state('app.claim.raw', {
            url: '/raw',
            templateUrl: 'claims.raw.html'
        });

    $httpProvider.defaults.withCredentials = true;
  

}])
.controller('ClaimsController', ['$scope', '$http', function ($scope, $http) {
    $scope.refresh = refresh;
    $scope.now = Date.now;
    $scope.url ='../api/users/current/claims'
    refresh($scope.url);

    
    function refresh(url) {
        $scope.claimsError = null;
        getClaims(url)
        .then(function (response) {
            $scope.claims = response.data;
        })
        .catch(function (response) {
            $scope.claimsError = response;
        });
    }

    function getClaims(url) {
        return $http.get(url);
    }
    
}])
;