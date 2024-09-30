document.getElementById('fetch-message').addEventListener('click', function () {
    fetch('/api/helloworld')  // Call the REST endpoint
        .then(response => response.text())
        .then(data => {
            document.getElementById('api-message').textContent = data;
        })
        .catch(error => {
            document.getElementById('api-message').textContent = 'Error: Unable to fetch message from API';
        });
});
