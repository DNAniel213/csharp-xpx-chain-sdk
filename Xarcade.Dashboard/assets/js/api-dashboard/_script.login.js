const loginURI     = 'http://localhost:5000/xarcadeaccount/login';
const redirectPage = '../api-dashboard/dashboard.html';

function loginUser()
{
    const loginUserInput     = document.getElementById('username');
    const loginPasswordInput = document.getElementById('password');

    const item = {
        username: loginUserInput.value,
        password: loginPasswordInput.value,
    }

    fetch(loginURI, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(item),
    })
        .then(response => response.json())
        .then(data => {
            if (data['message'] === 'Ok'){
                localStorage.setItem('userData', JSON.stringify(data['authenticationData']['account']));
                localStorage.setItem('token', JSON.stringify(data['authenticationData']['jwtToken']));
<<<<<<< HEAD
=======
                localStorage.setItem('cookie', JSON.stringify(data['authenticationData']['refreshToken']));
>>>>>>> 579df66... login, token POST integration
                redirect: window.location.replace(redirectPage);
            }else{
                alert(data['message']);
            }
        });
}