const url = "http://localhost:5299";

async function loadBourbonExperience() {
    const params = new URLSearchParams(window.location.search);
    const bourbonId = params.get("bourbonID");

    if (!bourbonId) {
        console.error("No bourbonID provided in URL.");
        return;
    }

    try {
        const response = await fetch(`${url}/api/BourbonMaster/${bourbonId}`, {
            method: "GET",
            credentials: "include"
        });
        if (!response.ok) {
            console.error("Failed to fetch bourbon data:", response.status);
            return;
        }

        const bourbon = await response.json();
        console.log("Fetched bourbon data:", bourbon);

        // Populate details
        document.getElementById("bourbonName").textContent = bourbon.name;
        document.getElementById("bourbonProof").textContent = bourbon.proof;
        document.getElementById("bourbonNotes").textContent = bourbon.flavorNotes;
        document.getElementById("cornPercentage").textContent = bourbon.cornPercentage;
        document.getElementById("ryePercentage").textContent = bourbon.ryePercentage;
        document.getElementById("barleyPercentage").textContent = bourbon.barleyPercentage;
        document.getElementById("bourbonImage").src = bourbon.photoUrl || "https://via.placeholder.com/150";

    } catch (error) {
        console.error("Error loading bourbon data:", error);
    }
}

document.addEventListener("DOMContentLoaded", loadBourbonExperience);
