document.getElementById("sendQuery").addEventListener("click", Click);
var searchElement = document.getElementById("queryString");
var img = document.querySelector("img");

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
    const file = await request.blob();

    document.getElementById("testP").innerText = file.type;

}