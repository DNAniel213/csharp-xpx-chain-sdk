const getGame  = 'http://localhost:5000/token/game';
const postGame = 'http://localhost:5000/token/generate/game';
const listGame = 'http://localhost:5000/game';

let userData = JSON.parse(localStorage.getItem('userData'));
let jwtToken = JSON.parse(localStorage.getItem('jwtToken'));

function getGames()
{
    let params = new URLSearchParams({
        userId: userData.userId
    });

    fetch(listGame + '?' + params.toString(), {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.json())
        .then(data => displayGames(data))
        .catch(error => alert('Unable to get tokens', error));
}

function addGame()
{
    let gameNameTextbox   = document.getElementById('game-name');
    let gameSupplyTextbox = document.getElementById('game-supply');

    let params = new URLSearchParams({
        name:     gameNameTextbox.value,
        duration: gameSupplyTextbox.value,
        owner:    userData.userId
    });

    fetch(postGame + '?' + params.toString(), {
        method: 'POST',
        headers: {
            'Authorization': 'Bearer ' + jwtToken,
            'Content-Type':  'application/json',
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data['message'] === 'Transaction Pending!'){
                displayGames(data);
            }
        })
        .catch(error => console.error('Unable to add a game', error));
}

function displayGames(gameData)
{
    let gameRow = document.getElementById('games');

    gameData.forEach(item => {
        let addDiv  = '<div class="col-lg-6 col-md-6 col-sm-6">'
                        +'<div class="card card-stats">'
                        +'<div class="card-body ">'
                            +'<div class="row">'
                            +'<div class=" col-md-12" >'
                                +'<div class="text-center" style="height:100px">'
                                +'<img src="../assets/img/dane-icon.png" style="height:100px" class="rounded" alt="aaa">'
                                +'</div>'
                            +'</div>'
                            +'</div>'
                            +'<div class="row">'
                            +'<div class="col-12 col-md-12">'
                                +'<div class="numbers text-center">'
                                +'<p class="card-category">Uses DANE COIN</p>'
                                +'<p class="card-title">Flappy Dog Game<p>'
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

        $(gameRow).append(addDiv);

    });
}