angular.module('STS.RP.Static', ['ui.router'])
.value('clientId', 'd8f47af9-2259-4971-9331-fd1cc41c1d8b')
.config(['$stateProvider', '$urlRouterProvider', '$httpProvider', function ($stateProvider, $urlRouterProvider, $httpProvider) {

    $urlRouterProvider.when('', '/claims');

    $stateProvider
        .state('app', {
            abstract: true,
            controller: 'RootController',
            template: '<ui-view></ui-view>'
        })
        .state('app.claims', {
            url: '/claims?code',
            controller: 'ClaimsController',
            templateUrl: 'claims.html'
        });

    //$httpProvider.interceptors.push('authInterceptor');

}])
//.factory('token', ['clientId', '$q', '$http', function (clientId, $q, $http) {

//    // redirect to https://login.microsoftonline.com/075af58f-3435-456a-95c4-5a4fd57e1a9d/oauth2/authorize........
//    // intercept token 


   

//    //return $http({
//    //    method: 'POST',
//    //    url: 'https://login.microsoftonline.com/075af58f-3435-456a-95c4-5a4fd57e1a9d/oauth2/authorize',
//    //    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
//    //    transformRequest: function (obj) {
//    //        var str = [];
//    //        for (var p in obj)
//    //            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
//    //        return str.join("&");
//    //    },
//    //    data: {
//    //        //client_id: clientId,
//    //        //grant_type: 'client_credentials',
//    //        //client_secret: '123456',
//    //        //scope: 'https://localhost:44306/'

//    //        client_id: clientId,
//    //        grant_type: 'authorization_code',
//    //        response_type: 'code'
//    //    }
//    //}).then(function (response) {
//    //    return response.data.access_token;
//    //});

//}])
//.factory('authInterceptor', ['$q', '$injector', function ($q, $injector) {
//    return {
//        'request': function(config) {
//            // do something on success
//            return config;
//        },
//        'responseError': function (rejection) {
//            // do something on error
//            if (rejection.status === 401) { /*unauthorized*/
//                var $http = $injector.get('$http');
//                var token = $injector.get('token');


//                return token.then(function (tokenValue) {
//                    if (!rejection.config.retryCount) {
//                        rejection.config.retryCount = rejection.config.retryCount || 0;
//                        rejection.config.retryCount++;
//                        rejection.config.headers.Authorization = 'Bearer ' + tokenValue;
//                        return $http(rejection.config);
//                    }
//                    return rejection;
//                });

//            }
//            return $q.reject(rejection);
//        }
//    };
//}])
.controller('ClaimsController', ['$http', '$scope', function ($http,$scope) {
    $http.get('https://localhost:44307/api/claims', { withCredentials: true }).then(function (response) {
        console.log(response);
        $scope.claims = response.data;
    });
}])
.controller('RootController', ['$scope', '$window', '$http', '$stateParams', '$rootScope', '$q','$location', 'clientId', function ($scope, $window, $http, $stateParams, $rootScope, $q, $location, clientId) {

    var code = $window.location.search.split('?').filter(Boolean)[0].split('&').map(function (i) {
        return i.split('=');
    }).find(function (i) {
        return i[0] === 'code';
    })[1];

    if (!code) {
        //login
        $window.location.href = getLoginUrl('https://login.microsoftonline.com/075af58f-3435-456a-95c4-5a4fd57e1a9d/oauth2/authorize', {
            client_id: clientId,
            grant_type: 'authorization_code',
            response_type: 'code',
            response_mode:'query', 
            redirect_uri: 'https://localhost:44306/#/claims/'
        });
    } else {

        $window.location.href = getLoginUrl('https://login.microsoftonline.com/075af58f-3435-456a-95c4-5a4fd57e1a9d/oauth2/authorize', {
            client_id: clientId,
            client_secret: '7IylhPKSYXjKDAf1UTUOhcIAtpc8AQSU8WiGJ5HurbU=',
            code: code,
            grant_type: 'authorization_code',
            response_type: 'code',
            scope: 'https://gploumhotmail.onmicrosoft.com/rp.static',
            response_mode: 'query',
            redirect_uri:'https://localhost:44306/#/claims/'
        });

    }


    function getLoginUrl(url,params) {
        var str = [];
        for (var p in params)
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(params[p]));
        return url+'?'+ str.join("&");
    }
}])
;