﻿document.getElementById("sendQuery").addEventListener("click", Click);
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

    if(await CheckResponseStatus(request) == 404)
        return;

    const response = await request.json();
    const message = response.message;

    for (let i = 0; i < message.length; i++) {
        const element = message.at(i);

        if (element == null || element == "END")
            break;

        let img = document.createElement("img");

        img.style.display = "flex";

        img.src = "data:image/png;base64," + String(element);

        img.width = 600;
        img.height = 400;

        document.body.getElementsByClassName("content")[0].appendChild(img);
    }

    document.getElementById("testP").innerText = String(request.status);

}

async function CheckResponseStatus(response)
{
    if (response.status != 404){
        return 200;
    }
    
    const errorResponse = await response.json();
    const errorMessage = errorResponse.message[0];
    document.getElementById("testP").innerText = String(errorMessage);
    response.status = 200;

    return 404;
    
}