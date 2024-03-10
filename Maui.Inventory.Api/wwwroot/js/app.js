var mainContent;

var onReady = setInterval(() => {
    if (document.readyState == 'complete') {
        clearInterval(onReady);

        mainContent = document.getElementById('main-content');

        networkRequest("api/admin/checkAuth", "POST")
            .then((response) => {
                if (response != null && response != undefined) {
                    if (response.success == true && response.message == 'validated') {
                        var isDev = localStorage.getItem('isDev');
                        if (isDev == 'yes') {
                            devDashboard();
                        } else {
                            logout();
                        }
                    }
                }
            });
    }
}, 250);

/**
 * Basic wrapper to avoid auth header boiler plate everywhere.
 * @param {string} endpoint - i.e api/my/endpoint
 * @param {string} method   - i.e POST
 * @param {any} data        - object to send along with request
 * @returns
 */
async function networkRequest(endpoint = "", method = "", data = {}) {
    try {
        const response = await fetch(`https://${window.location.host}/${endpoint}`, {
            method: method,
            body: JSON.stringify(data),
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (response.status == 401) {
            mainContent.innerHTML = loginHTML();
        }

        if (!response.ok) {
            return null;
        }

        return response.json();
    } catch (error) {
        return null;
    }
}

/**
 * Creates login template for user to login into
 * @returns {string}
 */
function loginHTML() {
    return `
<div class="container" style="height: 100vh; display: flex;">
    <div class="main-card p-3 rounded" style="width: 75%; margin: auto;">
        <div class="container p-0 mb-3">
            <div class="row">
                <div class="col-md-auto">
                    <img src="/img/favicon.svg" width="50" height="50"/>
                </div>
                <div class="col" style="margin-top: auto; margin-bottom: auto;">
                    <h3 class="text-white p-0 m-0">Owner Login</h3>
                </div>
            </div>
            </div>
            <div class="mb-3">
                <label for="username-input" class="form-label text-white">Username</label>
                <input type="text" class="form-control" id="username-input" aria-describedby="emailHelp">
            </div>
            <div class="mb-3">
                <label for="password-input" class="form-label text-white">Password</label>
                <input type="password" class="form-control" id="password-input">
            </div>
            <div class="container p-0 m-0">
            <div class="row">
                <div class="col">
                    <div id="status-placeholder"></div>
                </div>
                <div class="col-md-auto">
                    <button type="submit" class="btn btn-primary" style="float: right;" onclick="login()">Login</button>
                </div>
            </div>
        </div>
    </div>
</div>`;
}

/**
 * Display a bootstrap alert on the specified element if it exists
 * @param {string} id - the id of the element that serves as placeholder for alert
 * @param {string} message - what the alert should display
 * @param {string} type - primary | secondary | success | danger | warning | info | light | dark
 */
function alertOn(id = "", message = "", type = "primary") {
    const element = document.getElementById(id);
    if (element != null && element != undefined) {
        element.innerHTML = `
        <div>
            <div class="alert alert-${type} alert-dismissible" role="alert">
                <div>${message}</div>
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        </div>`;
    }
}

/**
 * Display a bootstrap spinner on the specified element if it exists
 * @param {string} id - the id of the element that serves as placeholder for spinner
 * @param {string} type - primary | secondary | success | danger | warning | info | light | dark
 */
function spinner(id = "", type = "primary") {
    const element = document.getElementById(id);
    if (element != null && element != undefined) {
        element.innerHTML = `
        <div class="spinner-border text-${type}" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>`;
    }
}

function clear(id = "") {
    try {
        document.getElementById(id).innerHTML = '';
    } catch { }
}

function login() {
    spinner('status-placeholder', 'warning');

    var unInput = document.getElementById('username-input');
    var pwdInput = document.getElementById('password-input');

    var userName = unInput.value;
    var password = pwdInput.value;

    var haveUsername = userName != null && userName != undefined && userName.length > 0;
    var havePassword = password != null && password != undefined && password.length > 0;
    if (!haveUsername || !havePassword) {
        alertOn('status-placeholder', 'Missing required information', 'info');
        return;
    }

    networkRequest("api/admin/login", "POST", { userName: userName, password: password })
        .then((response) => {
            clear('status-placeholder');

            if (response != null || response != undefined) {
                if (response.success == true && response.data.accessToken.length > 0) {
                    if (response.data.isDeveloper == true) {
                        document.cookie = `auth=${response.data.accessToken};`;
                        localStorage.setItem('isDev', 'yes');
                        devDashboard();
                    } else {
                        alertOn('status-placeholder', 'This tool is for developers of FTIM only.', 'warning');
                        unInput.value = '';
                        pwdInput.value = '';
                    }
                } else {
                    alertOn('status-placeholder', 'Invalid sign in.', 'danger');
                    unInput.value = '';
                    pwdInput.value = '';
                }
            } else {
                alertOn('status-placeholder', 'Network error occurred.', 'danger');
            }
        });
}

function logout() {
    document.cookie = 'auth=;';
    localStorage.setItem('isDev', '');
    mainContent.innerHTML = loginHTML();
}

function devDashboard() {
    mainContent.innerHTML = dashboard();
    htmx.process(htmx.find('#main-content'));
    // here in case there is more to do.
}

function dashboard() {
    return `
    <nav class="navbar navbar-expand-sm navbar-dark flex-sm-nowrap flex-wrap">
      <div class="container-fluid justify-content-start">
        <button class="navbar-toggler flex-grow-sm-1 flex-grow-0 me-2" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="navbar-brand">
            <img src="/img/favicon.svg" width="30" height="30"/>
            Inventory Management
        </div>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav">
            <li class="nav-item">
              <button class="nav-link active"
                      hx-get="/api/admin/testHtmx"
                      hx-trigger="load delay:250ms"
                      hx-target="#dashboard-content">
                      Feedback
              </button>
            </li>
          </ul>
        </div>
      </div>
    </nav>
    <div id="dashboard-content"></div>`;
}