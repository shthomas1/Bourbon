const url = "http://localhost:5299";

document.addEventListener("DOMContentLoaded", () => {
  loadAllBourbons();
  attachButtonHandlers();
  checkUserSession();
});

document.querySelector(".navbar-brand").addEventListener("click", (event) => {
  event.preventDefault(); // Prevent default link behavior
  loadDashboard(); // Loads the bourbon dashboard view
});


// Fetches all bourbons from the server and populates the carousel
async function loadAllBourbons() {
  try {
    const response = await fetch(`${url}/api/BourbonMaster/all`);
    const bourbons = await response.json();
    populateCarousel(bourbons);
  } catch (error) {
    console.error("Error fetching bourbons:", error);
  }
}

// Populates the carousel with fetched bourbons
function populateCarousel(bourbons) {
    const carouselInner = document.getElementById("carousel-inner");
    if (!carouselInner) return;

    carouselInner.innerHTML = "";

    bourbons.forEach((bourbon, index) => {
        const item = document.createElement("div");
        item.className = `carousel-item ${index === 0 ? "active" : ""}`;
        item.innerHTML = `
            <div class="d-flex flex-column justify-content-center align-items-center bg-dark text-white p-4" style="height: 350px;">
                <h3 class="mb-3">${bourbon.name}</h3>
                <img src="${bourbon.photoUrl || 'https://via.placeholder.com/150'}" alt="${bourbon.name}" 
                     class="img-fluid mb-2" style="max-height: 150px; border-radius: 8px;">
                <p class="mb-2"><strong>Proof:</strong> ${bourbon.proof}</p>
                <p><strong>Notes:</strong> ${bourbon.flavorNotes}</p>
            </div>
        `;

        carouselInner.appendChild(item);
    });
}


// Attach event handlers for Register and Login buttons
function attachButtonHandlers() {
  document.getElementById("btnRegister").addEventListener("click", showRegistrationForm);
  document.getElementById("btnLogin").addEventListener("click", showLoginForm);
  document.getElementById("btnBlog").addEventListener("click", showBlog);
}

function showBlog() {
  const mainContent = document.getElementById("mainContent");
  mainContent.innerHTML = `
    <div class="row">
      <div class="col-md-6 offset-md-3">
        <h2 class="mb-4">Blog</h2>
        <p>Coming soon...</p>
    `
}

// Check if user is already logged in (stored in localStorage)
function checkUserSession() {
  const userName = localStorage.getItem("loggedInUser");
  const navbar = document.getElementById("userNavbar");

  if (!navbar) {
    console.error("Navbar element not found.");
    return;
  }

  if (userName) {
    // User is logged in: Show "Hello, {FirstName}" + Logout button
    displayUserGreeting(userName);
  } else {
    // User is NOT logged in: Show Register/Login buttons
    navbar.innerHTML = `
      <button class="btn btn-primary me-2" id="btnRegister">Register</button>
      <button class="btn btn-success me-2" id="btnLogin">Login</button>
      <button class="btn btn-primary me-2" id="btnBlog">Blog</button>
    `;

    attachButtonHandlers();
  }
}
 
  
// Updates the navbar to show "Hello, {FirstName}" instead of Login/Register buttons
function displayUserGreeting(firstName) {
  const navbar = document.getElementById("userNavbar");

  if (!navbar) {
    console.error("Navbar element not found.");
    return;
  }

  navbar.innerHTML = `
    <span class="navbar-text me-3">Hello, ${firstName}!</span>
    <button class="btn btn-outline-danger btn-sm" id="btnLogout">Logout</button>
    <button class="btn btn-primary" id="btnBlog">Blog</button>
  `;

  // Attach event listeners to buttons
  document.getElementById("btnLogout").addEventListener("click", handleLogout);
  document.getElementById("btnBlog").addEventListener("click", showBlog);
}


// Clears main content and loads the registration form
function showRegistrationForm() {
  const mainContent = document.getElementById("mainContent");

  // Insert the registration form HTML
  mainContent.innerHTML = `
    <div class="row">
      <div class="col-md-6 offset-md-3">
        <h2 class="mb-4">Register</h2>
        <form id="registrationForm">
          <div class="mb-3">
            <label for="emailInput" class="form-label">Email</label>
            <input type="email" class="form-control" id="emailInput" required />
          </div>
          <div class="mb-3">
            <label for="firstNameInput" class="form-label">First Name</label>
            <input type="text" class="form-control" id="firstNameInput" required />
          </div>
          <div class="mb-3">
            <label for="lastNameInput" class="form-label">Last Name</label>
            <input type="text" class="form-control" id="lastNameInput" required />
          </div>
          <div class="mb-3">
            <label for="passwordInput" class="form-label">Password</label>
            <input type="password" class="form-control" id="passwordInput" required />
          </div>
          <div class="mb-3">
            <label for="repeatPasswordInput" class="form-label">Repeat Password</label>
            <input type="password" class="form-control" id="repeatPasswordInput" required />
          </div>
          <button type="submit" class="btn btn-primary">Submit</button>
        </form>
      </div>
    </div>
  `;

  // Attach event listener to the newly inserted form
  document.getElementById("registrationForm").addEventListener("submit", handleRegistrationSubmit);
}

// Submit handler for the registration form
async function handleRegistrationSubmit(e) {
  e.preventDefault();

  // Gather input values
  const email = document.getElementById("emailInput").value.trim();
  const firstName = document.getElementById("firstNameInput").value.trim();
  const lastName = document.getElementById("lastNameInput").value.trim();
  const password = document.getElementById("passwordInput").value;
  const repeatPassword = document.getElementById("repeatPasswordInput").value;

  if (password !== repeatPassword) {
    alert("Passwords do not match!");
    return;
  }

  // Post the user to the server
  try {
    const response = await fetch(`${url}/api/Users/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        email: email,
        firstName: firstName,
        lastName: lastName,
        password: password
      })
    });

    if (!response.ok) {
      const errText = await response.text();
      alert("Registration failed: " + errText);
      return;
    }

    alert("Registration successful!");
  } catch (err) {
    console.error("Registration error:", err);
    alert("An error occurred. Please try again.");
    return;
  }

  // After a successful registration, show the login form
  showLoginForm();
}

// Clears main content and loads the login form
function showLoginForm() {
  const mainContent = document.getElementById("mainContent");
  mainContent.innerHTML = `
    <div class="row">
      <div class="col-md-6 offset-md-3">
        <h2 class="mb-4">Login</h2>
        <form id="loginForm">
          <div class="mb-3">
            <label for="emailLoginInput" class="form-label">Email</label>
            <input type="email" class="form-control" id="emailLoginInput" required />
          </div>
          <div class="mb-3">
            <label for="passwordLoginInput" class="form-label">Password</label>
            <input type="password" class="form-control" id="passwordLoginInput" required />
          </div>
          <button type="submit" class="btn btn-success">Login</button>
        </form>
      </div>
    </div>
  `;

  // Attach event listener to the login form
  document.getElementById("loginForm").addEventListener("submit", handleLoginSubmit);
}


// Handles user login and updates UI
async function handleLoginSubmit(e) {
  e.preventDefault();

  const email = document.getElementById("emailLoginInput").value.trim();
  const password = document.getElementById("passwordLoginInput").value;

  try {
    const response = await fetch(`${url}/api/Users/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      throw new Error("Invalid email or password.");
    }

    const data = await response.json();
    const firstName = data.message.replace("Hello, ", "");

    // Store user name in localStorage
    localStorage.setItem("loggedInUser", firstName);

    // Update navbar immediately
    displayUserGreeting(firstName);

    // Redirect back to the dashboard
    loadDashboard();

  } catch (error) {
    alert(error.message);
  }
}
  
// Loads the Bourbon dashboard (after login)
function loadDashboard() {
    const mainContent = document.getElementById("mainContent");
    if (!mainContent) return;
  
    // Update navbar to show "Hello, {FirstName}"
    const firstName = localStorage.getItem("loggedInUser");
    if (firstName) {
      displayUserGreeting(firstName);
    }
  
    // Restore the bourbon dashboard view
    mainContent.innerHTML = `
      <div class="row mb-4">
        <div class="col-md-6 offset-md-3">
          <div id="bourbonCarousel" class="carousel slide shadow-sm" data-bs-ride="carousel">
            <div class="carousel-inner" id="carousel-inner">
              <!-- Carousel items populated by JavaScript -->
            </div>
            <button class="carousel-control-prev" type="button" data-bs-target="#bourbonCarousel" data-bs-slide="prev">
              <span class="carousel-control-prev-icon"></span>
            </button>
            <button class="carousel-control-next" type="button" data-bs-target="#bourbonCarousel" data-bs-slide="next">
              <span class="carousel-control-next-icon"></span>
            </button>
          </div>
        </div>
      </div>
    `;
  
    // Reload the bourbon data
    loadAllBourbons();
  }

function handleLogout() {
  // Remove the stored user
  localStorage.removeItem("loggedInUser");

  // Restore the Nav bar to Buttons
  const navbar = document.getElementById("userNavbar");
  navbar.innerHTML = `
    <button class="btn btn-primary me-2" id="btnRegister">Register</button>
    <button class="btn btn-success me-2" id="btnLogin">Login</button>
    <button class="btn btn-primary me-2" id="btnBlog">Blog</button>
  `;

  // Reattach event listeners to buttons
  attachButtonHandlers();

  // Reload the page to reset the state (Optional)
  location.reload();
}
