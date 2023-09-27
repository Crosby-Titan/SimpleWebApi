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


    const file = await request.json();

    img.src = "data:image/png;base64," + String(file.imgStr);

    document.body.appendChild(img);

    document.getElementById("testP").innerText = String(request.status);
}