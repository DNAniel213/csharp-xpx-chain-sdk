const getToken          = 'http://localhost:5000/token/token';
const postToken         = 'http://localhost:5000/token/generate/token';
const listToken         = 'http://localhost:5000/token';
const modifyTokenSupply = 'http://localhost:5000/token/modify/supply';

const redirectPage = '../api-dashboard/login.html';

let userData = JSON.parse(localStorage.getItem('userData'));
let jwtToken = JSON.parse(localStorage.getItem('jwtToken'));

/**
 * Gets a list of tokens of the logged user
 * 
 */
function getTokens()
{
    let params = new URLSearchParams({
        userId: userData.userId
    });

    fetch(listToken + '?' + params.toString(), {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.json())
        .then(data => displayTokens(data))
        .catch(error => alert('Unable to get tokens', error));
}

/**
 * Creates a token for the logged user
 * 
 * 
 */
function addToken()
{
    let tokenNameTextbox     = document.getElementById('token-name');
    let tokenSupplyTextbox   = document.getElementById('token-supply');
    let tokenNamespaceSelect = document.getElementById('token-namespace');
    
<<<<<<< HEAD
    let userData = JSON.parse(localStorage.getItem('userData'));
    let jwtToken = JSON.parse(localStorage.getItem('token'));
<<<<<<< HEAD
<<<<<<< HEAD
    
=======
>>>>>>> ffe61e4... PUT integration
    let params = new URLSearchParams({
        name:  tokenNameTextbox.value,
=======
    let cookie   = JSON.parse(localStorage.getItem('cookie'));
    
    let params = new URLSearchParams({
        name: tokenNameTextbox.value,
>>>>>>> 579df66... login, token POST integration
=======
    
    let params = new URLSearchParams({
        name:  tokenNameTextbox.value,
>>>>>>> bc94152... create game POST integration
        owner: userData.userId
        //namespaceName: tokenNamespaceSelect.options[tokenNamespaceSelect.value].text
    });

<<<<<<< HEAD
<<<<<<< HEAD
=======
    let item = {
        name: tokenNameTextbox.value,
        owner: userData.userId
    }

>>>>>>> 579df66... login, token POST integration
=======
>>>>>>> bc94152... create game POST integration
    fetch(postToken + '?' + params.toString(), {
        method: 'POST',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.json())
        .then(data => {
<<<<<<< HEAD
<<<<<<< HEAD
=======
            console.log(data['message']);z
>>>>>>> 579df66... login, token POST integration
=======
>>>>>>> bc94152... create game POST integration
            if (data['message'] === 'Success!'){
                //getTokens();
                tokenNameTextbox.value     = '';
                tokenSupplyTextbox.value   = '';
                tokenNamespaceSelect.value = '';
            }
        })
<<<<<<< HEAD
<<<<<<< HEAD
        .catch(error => alert('Unable to add token', error));
=======
        .catch(error => console.error('Unable to add token', error));
>>>>>>> 579df66... login, token POST integration
=======
        .catch(error => alert('Unable to add token', error));
>>>>>>> bc94152... create game POST integration

}

function modifySupply(tokenId)
{
    let supplyInput = document.getElementById('supplyInput');

    let params = new URLSearchParams({
        userId: userData.userId,
        tokenId: tokenId,
        supply: supplyInput
    });

    fetch(modifyTokenSupply + '?' + params.toString(), {
        method: 'PUT',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json' 
        },
    })
        .then(response => response.json())
        .then(() => getTokens());
}

/**
 * Displays tokens of the logged user to the webpage
 * 
 * @param {TokenViewModel} tokenData 
 */
function displayTokens(tokenData)
{
    let tokenRow = document.getElementById('tokens');

    tokenData.forEach(item => {
        let addDiv   = '<div class="col-lg-6 col-md-6 col-sm-6">'
                            +'<div class="card card-stats">'
                            +'<div class="card-body ">'
                                +'<div class="row">'
                            +'<div class="col-5 col-md-12" >'
                                +'<div class="text-center" style="height:100px">'
                                    +'<img src="../assets/img/dane-icon.png" style="height:100px" class="rounded" alt="aaa">'
                                    +'</div>'
                                +'</div>'
                                +'</div>'
                                +'<div class="row">'
                                +'<div class="col-12 col-md-12">'
                                    +'<div class="numbers text-center">'
                                    +'<p class="card-category">'+ item.name +'</p>'
                                    +'<p class="card-title">26000DANE<p>'
                                    +'</div>'
                                +'</div>'
                                +'</div>'
                            +'</div>'
                            +'<div class="card-footer">'
                                +'<hr>'
                                +'<div class="row">'
                                +'<div class="col-lg-4">'
                                    +'<div class="btn btn-outline-info btn-sm btn-block"  data-toggle="modal" data-target="#addSupplyModal">'
                                    +'<i class="nc-icon nc-simple-add"></i>'
                                    +'Add supply'
                                    +'</div>'
                                +'</div>'
                                +'<div class="col-lg-4">'
                                    +'<div class="btn btn-outline-info btn-sm btn-block" data-toggle="modal" data-target="#sendSupplyModal">'
                                    +'<i class="nc-icon nc-send"></i>'
                                    +'Send'
                                    +'</div>'
                                +'</div>'
                                +'<div class="col-lg-4">'
                                    +'<div class="btn btn-outline-info btn-sm btn-block" data-toggle="modal" data-target="#modifyTokenModal">'
                                    +'<i class="nc-icon nc-settings-gear-65"></i>'
                                    +'Modify'
                                    +'</div>'
                                +'</div>'
                                +'</div>'
                            +'</div>'
                            +'</div>'
                        +'</div>';

        //let addSupply = document.getElementById('supply-button');
        //addSupply.setAttribute('onclick', `modifySupply(${item.tokenId})`);
        //addSupply.setAttribute('onsubmit', item.tokenId);
        //console.log(addSupply);
        $(tokenRow).append(addDiv);
    });
    
}