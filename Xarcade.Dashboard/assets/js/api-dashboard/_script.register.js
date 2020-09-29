const registerURI = 'http://localhost:5000/xarcadeaccount/register';

const redirectPage = '../api-dashboard/login.html';

function registerUser()
{
    let firstNameInput = document.getElementById('first-name');
    let lastNameInput  = document.getElementById('last-name');
    let emailInput     = document.getElementById('email');
    let userNameInput  = document.getElementById('user-name')
    let passwordInput  = document.getElementById('password');
    let confirmInput   = document.getElementById('password-confirm');
    let termsInput     = document.getElementById('terms-conditions');
    
    let item = {
        firstName:       firstNameInput.value,
        lastName:        lastNameInput.value,
        userName:        userNameInput.value,
        email:           emailInput.value,
        password:        passwordInput.value,
        confirmPassword: confirmInput.value,
        acceptTerms:     termsInput.checked
    };

    if (item.password === item.confirmPassword && item.acceptTerms === true){
        fetch(registerURI, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(item)
        })
        .then(response => response.json())
        .then(data => {
            if(data['message'] === 'Account Registered!'){
                alert('Account registered! Please check your email to verify');
                redirect: window.location.replace(redirectPage);
            }else{
                alert(data['message']);
            }
        });
    }else if(item.password !== item.confirmPassword){
        alert('Passwords do not match!');
    }else if(item.acceptTerms === false){
        alert('Please check the terms and conditions!');
    }

}