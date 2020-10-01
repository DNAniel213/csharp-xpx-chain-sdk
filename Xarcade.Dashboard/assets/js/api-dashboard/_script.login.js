const loginURI = 'http://localhost:5000/xarcadeaccount/login';

const getOwnerURI    = 'http://localhost:5000/account/owner';
const createOwnerURI = 'http://localhost:5000/account/generate/owner';

const redirectPage = '../api-dashboard/dashboard.html';

function loginUser()
{
    let loginUserInput     = document.getElementById('username');
    let loginPasswordInput = document.getElementById('password');

    let item = {
        username: loginUserInput.value,
        password: loginPasswordInput.value
    };

    fetch(loginURI, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(item),
    })
        .then(response => response.json())
        .then(data => {
            if (data['message'] === 'Ok'){
                localStorage.setItem('userData', JSON.stringify(data['authenticationData']['account']));
                localStorage.setItem('jwtToken', JSON.stringify(data['authenticationData']['jwtToken']));

                getOwner();
            }else{
                alert(data['message']);
            }
        });
}

function getOwner()
{
    let userData = JSON.parse(localStorage.getItem('userData'));
    let jwtToken = JSON.parse(localStorage.getItem('jwtToken'));

    let params = new URLSearchParams({
        userId: userData.userId,
        searchId: userData.userId
    });

    fetch(getOwnerURI + '?' + params.toString(), {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            if(response['status'] === 204){
                alert('Creating Owner!');
                createOwner();
            }else{
                redirect: window.location.replace(redirectPage);
            }
        });
}

function createOwner()
{
    let userData = JSON.parse(localStorage.getItem('userData'));
    let jwtToken = JSON.parse(localStorage.getItem('jwtToken'));

    let params = new URLSearchParams({
        userId: userData.userId
    });

    fetch(createOwnerURI + '?' + params.toString(),{
        method: 'POST',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {
            if(data['message'] === 'Success!'){
                redirect: window.location.replace(redirectPage);
            }
        })
}