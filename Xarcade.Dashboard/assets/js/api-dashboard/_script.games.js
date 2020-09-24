const getGame  = 'http://localhost:5000/token/game';
const postGame = 'http://localhost:5000/token/generate/game';

function getGames()
{
    fetch(getGame)
        .then(response => response.json())
        .then(data => displayGames(data))
        .error(error => console.error('Unable to get games', error));
}

function addGame()
{
    const gameNameTextbox   = document.getElementById('game-name');
    const gameSupplyTextbok = document.getElementById('game-supply');

    fetch(postGame, {
        method: 'POST',
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            
        })
        .error(error => console.error('Unable to add a game', error));
}

function displayGames(data)
{
    console.log(data)
}