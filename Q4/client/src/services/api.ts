function getData(
    url: string,
    method:
        string,
    headers:
        any,
    fn: (data: any) => any
) {
    fetch(url, {
        method: method,
        headers: headers
    }).then(response => {
        if (!response.ok) {
            throw new Error("HTTP Error: " + response.status);
        }
        return response.json();
    }).then(json => fn(json));
}

export default getData;