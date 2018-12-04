/// <reference path="angular.js" />
/// <reference path="angular-route.js" />

var app = angular.module("TaskBoardApp", ["ngRoute"])
    .config(function ($routeProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "Templates/TaskManagement.html",
                controller: "getallprojectsController"
            })
            .when("/ProjectPage/:id", {
                templateUrl: "Templates/ProjectPage.html",
                controller: "getprojectDetailsController"
            })
            .otherwise({
                redirectTo: '/'
            });
    })
    .controller("getallprojectsController", function ($scope, $http) {
        $http.get("TaskManagerService.asmx/GetAllProjects")
            .then(function (response) {
                $scope.projects = response.data;
            })
    })
    .controller("getprojectDetailsController", function ($scope, $http, $routeParams) {
        $http({
            url: "TaskManagerService.asmx/GetProject",
            params: { projectId: $routeParams.id },
            method: "get"
        })
            .then(function (response) {
                $scope.ProjectDetails = response.data;
        })

    })