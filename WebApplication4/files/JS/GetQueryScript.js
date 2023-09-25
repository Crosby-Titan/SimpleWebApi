const { get } = require("http");

document.getElementById("sendQuery").addEventListener("click", Click);
var searchElement = document.getElementById("queryString");
var img = document.querySelector("img");

async function Click() {
    const request = await fetch("https://localhost:7135/",
        {
            method: "GET",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                QueryString: searchElement.value
            })
        }
    );

    while (true) {
        const file = await request.blob();

        if (file.size <= 0)
            break;

        img.src = URL.createObjectURL(file);

    }
    
}