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

    var allInterviews = function (success, error) {
        $http.get('/api/interviews').
          success(success).
          error(error);
    }

    var searchInterview = function (id, query, success, error) {
        $http.get('/api/interviews?id=' + id + '&query=' + encodeURIComponent(query)).
          success(success).
          error(error);
    }

    return {
        search: search,
        searchInterview: searchInterview,
        allInterviews: allInterviews,
        lookup: lookup
    };
}

InterviewsApiFactory.$inject = ['$http'];

