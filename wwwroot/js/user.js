const uri = '/User';
let users = [];

const token = localStorage.getItem('tasksToken')

function getUsers() {
    fetch(uri, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify()
    })
        .then(response => response.json())
        .then(data => _displayUsers(data))

        .catch(error => {
            console.error('Unable to get users.', error)
        }
        );
}

function addUser() {
    const addNameTextbox = document.getElementById('add-name');
    const user = {
        name: addNameTextbox.value.trim(),
        password: ""
    };
    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`

        },
        body: JSON.stringify(user)
    })
        .then(response => response.json())
        .then(() => {
            getUsers();
            addNameTextbox.value = '';
        })
        .catch(error => console.error('Unable to add user.', error));

}

function deleteUser(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
    })
        .then(() => getUsers())
        .catch(error => console.error('Unable to delete user.', error));
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'user' : 'users';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayUsers(data) {
    const tBody = document.getElementById('Users');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(user => {

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteUser(${user.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode1 = document.createTextNode(user.name);
        td1.appendChild(textNode1);

        let td2 = tr.insertCell(1);
        let textNode2 = document.createTextNode(user.password);
        td2.appendChild(textNode2);

        let td4 = tr.insertCell(2);
        td4.appendChild(deleteButton);
    });

    users = data;
}
getUsers();