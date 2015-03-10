var InterviewsApiFactory = function ($http) {
    var search = function (q, success, error) {
        $http.get('/api/search?q=' + q).
          success(success).
          error(error);
    }

    var lookup = function (id, success, error) {
        $http.get('/api/interviews?id=' + id).
          success(success).
          error(error);
    }

    return {
        search: search,
        lookup: lookup
    };
}

InterviewsApiFactory.$inject = ['$http'];

