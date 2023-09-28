document.getElementById("sendQuery").addEventListener("click", Click);
var searchElement = document.getElementById("queryString");
var img = document.createElement("img");

async function Click() {
    const request = await fetch("https://localhost:7135/search",
        {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                QueryString: searchElement.value
            })
        }
    );

    if (request.status == 404) {
        const errorResponse = await request.json();
        document.getElementById("testP").innerText = String(errorResponse.message);
        return;
    }

    const file = await request.json();

    img.src = "data:image/png;base64," + String(file.message);

    img.width = 600;
    img.height = 400;

    document.body.getElementsByClassName("content")[0].appendChild(img);

    document.getElementById("testP").innerText = String(request.status);
}