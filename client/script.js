const url = "http://localhost:5299";

document.addEventListener("DOMContentLoaded", () => {
    loadAllBourbons();
});

// Fetches all bourbons from the server
async function loadAllBourbons(){
    try {
        const response = await fetch(`${url}/api/BourbonMaster/all`);
        const bourbons = await response.json();
        populateCarousel(bourbons);
    } catch (error) {
        console.error("Error fetching bourbons:", error);
    }
}

// Populates the carousel with all fetched bourbons
function populateCarousel(bourbons){
    const carouselInner = document.getElementById("carousel-inner");
    carouselInner.innerHTML = "";

    bourbons.forEach((bourbon, index) => {
        const item = document.createElement("div");
        item.className = `carousel-item ${index === 0 ? 'active' : ''}`;
        item.innerHTML = `
          <div class="d-flex flex-column justify-content-center align-items-center bg-dark text-white p-4" style="height: 250px;">
              <h3 class="mb-3">${bourbon.name}</h3>
              <p class="mb-2"><strong>Proof:</strong> ${bourbon.proof}</p>
              <p><strong>Notes:</strong> ${bourbon.flavorNotes}</p>
          </div>
        `;

        carouselInner.appendChild(item);
    });
}
