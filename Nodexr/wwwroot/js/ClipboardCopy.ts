//Handle copying text to the user's clipboard.
//From https://chrissainty.com/copy-to-clipboard-in-blazor/

export async function copyText(text, message) {
    await navigator.clipboard.writeText(text);
    if (message) {
        alert(message);
    }
}
