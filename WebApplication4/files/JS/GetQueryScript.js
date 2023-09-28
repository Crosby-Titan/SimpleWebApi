document.getElementById("sendQuery").addEventListener("click", Click);
var searchElement = document.getElementById("queryString");

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
        const errorMessage = Array(errorResponse.message)[0];
        document.getElementById("testP").innerText = String(errorMessage);
        return;
    }

    const response = await request.json();
    const message = Array(response.message);

    for (let i = 0; i < message.length; i++) {
        const element = message[i];

        if (element == null || element == "END")
            break;

        let img = document.createElement("img");
        img.src = "data:image/png;base64," + String(element);

        img.width = 600;
        img.height = 400;

        document.body.getElementsByClassName("content")[0].appendChild(img);
    }

    document.getElementById("testP").innerText = String(request.status);

}