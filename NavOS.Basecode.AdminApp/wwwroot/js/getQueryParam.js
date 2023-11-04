function getQueryParam(parameterName) {
    const queryString = window.location.search
    const urlParams = new URLSearchParams(queryString)

    return urlParams.get(parameterName)
}

let searchQuery = getQueryParam("searchQuery")
if (searchQuery != null) {
    document.getElementById("searchQuery").value = searchQuery
}