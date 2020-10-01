const getToken          = 'http://localhost:5000/token/token';
const postToken         = 'http://localhost:5000/token/generate/token';
const listToken         = 'http://localhost:5000/token';
const modifyTokenSupply = 'http://localhost:5000/token/modify/supply';
const sendTokenURL      = 'http://localhost:5000/transaction/send/token';
const listGame          = 'http://localhost:5000/game/';

const redirectPage = '../api-dashboard/tokens.html';

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
    let namespaceSelectbox   = document.getElementById('namespace');
    
    let params = new URLSearchParams({
        name:          tokenNameTextbox.value,
        owner:         userData.userId,
        namespaceName: namespaceSelectbox.value,
        quantity:      tokenSupplyTextbox.value
    });

    fetch(postToken + '?' + params.toString(), {
        method: 'POST',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.json())
        .then(data => {
            if (data['message'] === 'Success!'){
                $('#create-token-button').submit(function(e){
                    e.preventDefault();
                    $('#create-token-modal').modal('hide');
                    return false;
                });

                getTokens();
            }
        })
        .catch(error => alert('Unable to add token', error));
}

function modifySupply(tokenId)
{
    let supplyInput = document.getElementById('supply-input');

    let params = new URLSearchParams({
        userId: userData.userId,
        tokenId: tokenId,
        supply: supplyInput.value
    });

    fetch(modifyTokenSupply + '?' + params.toString(), {
        method: 'PUT',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json' 
        },
    })
        .then(response => response.json())
        .then(() => {
            alert('Transaction successful!');
            redirect: window.location.replace(redirectPage);
        });
}

function sendToken(tokenId)
{
    let receiverInput = document.getElementById('send-receiver');
    let amountInput   = document.getElementById('send-amount');
    let messageInput  = document.getElementById('send-message');

    let params = new URLSearchParams({
        senderId:   userData.userId,
        receiverId: receiverInput.value,
        tokenId:    tokenId,
        amount:     amountInput.value,
        message:    messageInput.value
    });

    fetch(sendTokenURL + '?' + params.toString(), {
        method: 'POST',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.json())
        .then(() => {
            alert('Transaction successful!');
            redirect: window.location.replace(redirectPage);
        })
}

function getSpecificToken(tokenId)
{
    let params = new URLSearchParams({
        userId: userData.userId,
        tokenId: tokenId
    });

    fetch(getToken + '?' + params.toString(),{
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => console.log(data));
}

function getUnlinkedNamespace()
{
    let namespaceSelect = document.getElementById('namespace');

    let params = new URLSearchParams({
        userId: userData.userId
    });

    fetch(listGame + '?' + params.toString(), {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {

            data.forEach(item => {
                let options = document.createElement('option');

                if (item.tokens === null){
                    options.appendChild(document.createTextNode(item.name));
                    options.value = item.name;

                    namespaceSelect.appendChild(options);
                }
            });

        });
}

/**
 * Displays tokens of the logged user to the webpage
 * 
 * @param {TokenViewModel} tokenData 
 */
function displayTokens(tokenData)
{
    let tokenRow = document.getElementById('tokens');
    let count    = 1;

    tokenData.forEach(item => {
        //let tokenLink = 
        let addDiv    = '<div class="col-lg-6 col-md-6 col-sm-6" id="token-div-'+ count +'">'
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
                                    +'<p class="card-category">'+ item.namespaceName +'</p>'
                                    +'<p class="card-title">'+ item.quantity +" "+item.name  +'<p>'
                                    +'</div>'
                                +'</div>'
                                +'</div>'
                            +'</div>'
                            +'<div class="card-footer">'
                                +'<hr>'
                                +'<div class="row">'
                                +'<div class="col-lg-4">'
                                    +'<div type="button" class="btn btn-outline-info btn-sm btn-block"  data-toggle="modal" data-target="#addSupplyModal" id="add-supply-'+ count +'">'
                                    +'<i class="nc-icon nc-simple-add"></i>'
                                    +'Add supply'
                                    +'</div>'
                                +'</div>'
                                +'<div class="col-lg-4">'
                                    +'<div class="btn btn-outline-info btn-sm btn-block" data-toggle="modal" data-target="#sendSupplyModal" id="send-token-'+ count +'">'
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

        if(!document.getElementById('token-div-'+count)){
            let addSupplyConfirm = document.getElementById('add-supply-form');
            let sendTokenConfirm = document.getElementById('send-token-form');

            $(tokenRow).append(addDiv);

            document.getElementById('add-supply-'+count).addEventListener('click', function(){
               addSupplyConfirm.onsubmit = function(){
                   modifySupply(item.tokenId);
               };
            });

            document.getElementById('send-token-'+count).addEventListener('click', function(){
                sendTokenConfirm.onsubmit = function (){
                    sendToken(item.tokenId);
                };
            });
        }
        count++;
    });
    
}