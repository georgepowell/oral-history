var SearchFactory = function ($http) {
    var search = function (q, success, error) {
        $http.get('/api/search/' + q).
          success(success).
          error(error);
    }

    return {
        search: search
    };
}

SearchFactory.$inject = ['$http'];

