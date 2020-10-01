const verifyEmail = 'http://localhost:5000/xarcadeaccount/verifyemail';
const createOwner = 'http://localhost:5000/account/generate/user';

const redirectPage = '../api-dashboard/login.html';

function verifyUser()
{
    let verifyCode = document.getElementById('verification-code');

    let item = {
        token: verifyCode.value
    };

    fetch(verifyEmail, {
        method: 'POST',
        headers: { 'Content-type': 'application/json' },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(data => {
            if (data['message'] === 'Ok'){
                alert('Account verified!');
                //redirect: window.location.replace(redirectPage);
            }
            console.log(data);
        });
    
    fetch(createOwner)
}

