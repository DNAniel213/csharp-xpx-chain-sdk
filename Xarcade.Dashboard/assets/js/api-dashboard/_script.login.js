const loginURI = 'http://localhost:5000/xarcadeaccount/login';

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
                redirect: window.location.replace(redirectPage);
            }else{
                alert(data['message']);
            }
        });
}