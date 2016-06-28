angular.module('STS.Angular', [])
.controller('ClaimsController', ['$scope', '$http', function ($scope, $http) {

    $http.get('https://localhost:44302/api/claims', {withCredentials :true}).then(function (response) {

        console.log(response);
        $scope.claims = response.data;

    });

}])
;