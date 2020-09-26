const loginURI     = 'http://localhost:5000/xarcadeaccount/login';
const redirectPage = '../api-dashboard/dashboard.html';

function loginUser()
{
    let loginUserInput     = document.getElementById('username');
    let loginPasswordInput = document.getElementById('password');

    let item = {
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
<<<<<<< HEAD
<<<<<<< HEAD
                localStorage.setItem('token', JSON.stringify(data['authenticationData']['jwtToken']));
<<<<<<< HEAD
<<<<<<< HEAD
=======
                localStorage.setItem('cookie', JSON.stringify(data['authenticationData']['refreshToken']));
>>>>>>> 579df66... login, token POST integration
=======
>>>>>>> bc94152... create game POST integration
=======
                localStorage.setItem('jwtToken', JSON.stringify(data['authenticationData']['jwtToken']));
>>>>>>> ffe61e4... PUT integration
=======
                localStorage.setItem('jwtToken', JSON.stringify(data['authenticationData']['jwtToken']));
>>>>>>> ffe61e4dfedfba6699b4719d31249e96a2ce95c2
                redirect: window.location.replace(redirectPage);
            }else{
                alert(data['message']);
            }
        });
}