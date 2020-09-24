const uri = 'http://localhost:5000/account/owner/?ownerId=29';

function getUser()
{
    fetch(uri)
        .then(response => response.json())
        .then(data => displayUserDetails(data))
        .catch(error => console.error('Unable to get user', error));
}

function displayUserDetails(data)
{
    document.getElementById('email').value = data.email;
    document.getElementById('first_name').value = data.name;
}